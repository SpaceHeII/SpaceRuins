using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

[SelectionBase]
public class ControladorJugador : MonoBehaviour
{
    public bool caminando = false;
    public float velocidad = 1.0f;
    public float cooldownTiempo = 0.5f;
    private float tiempoUltimoClick = 0f;
    public float mezcla;

    [Space]
    public Transform cuboActual;
    public Transform cuboSeleccionado;
    public Transform indicador;

    public GestorJuego gestorJuego;

    [Space]
    public List<Transform> caminoFinal = new List<Transform>();

    void Start()
    {
        if (cuboActual == null)
        {
            Debug.LogError("cuboActual no está asignado en el inspector.");
        }

        RayCastAbajo();

        if (cuboActual != null && !cuboActual.GetComponent<Caminable>().noRotar)
        {
            Quaternion rotacionDeseada = cuboActual.GetComponent<Caminable>().ObtenerRotacionDeseada();
            transform.rotation = rotacionDeseada;
        }
    }

    public void PresionarBoton(string nombreBoton)
    {
        if (GestorJuego.instancia != null)
        {
            GestorJuego.instancia.EjecutarMovimientos(nombreBoton, velocidad);
        }
        else
        {
            Debug.LogError("Instancia de GestorJuego no encontrada.");
        }
    }

    public void LlamarEjecutarMovimientos(int indiceBoton)
    {
        Sequence s = DOTween.Sequence();
        s.Append(indicador.GetComponent<Renderer>().material.DOColor(Color.white, "_Color", .1f));
        s.AppendCallback(() => PresionarBoton(indiceBoton.ToString()));
    }

    void Update()
    {
        if (caminando) return;

        RayCastAbajo();

        if (cuboActual != null && cuboActual.GetComponent<Caminable>().sueloMovil)
        {
            transform.parent = cuboActual;
        }
        else
        {
            transform.parent = null;
        }

        if (Input.GetMouseButtonDown(0) && Time.time - tiempoUltimoClick >= cooldownTiempo)
        {
            tiempoUltimoClick = Time.time;
            Ray rayoMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit golpeMouse;

            if (Physics.Raycast(rayoMouse, out golpeMouse))
            {
                if (golpeMouse.transform.GetComponent<Caminable>() != null)
                {
                    cuboSeleccionado = golpeMouse.transform;
                    DOTween.Kill(gameObject.transform);
                    caminoFinal.Clear();
                    EncontrarCamino();

                    indicador.position = golpeMouse.transform.GetComponent<Caminable>().ObtenerPuntoCaminable();
                    Sequence s = DOTween.Sequence();
                    s.AppendCallback(() => indicador.GetComponentInChildren<ParticleSystem>().Play());
                    s.Append(indicador.GetComponent<Renderer>().material.DOColor(Color.white, "_Color", .1f));
                    s.Append(indicador.GetComponent<Renderer>().material.DOColor(Color.black, .3f).SetDelay(.2f));
                    s.Append(indicador.GetComponent<Renderer>().material.DOColor(Color.clear, .3f));
                }
            }
        }
    }

    void EncontrarCamino()
    {
        List<Transform> proximosCubos = new List<Transform>();
        List<Transform> cubosPasados = new List<Transform>();

        if (cuboActual != null)
        {
            foreach (Camino camino in cuboActual.GetComponent<Caminable>().caminosPosibles)
            {
                if (camino.activo)
                {
                    proximosCubos.Add(camino.objetivo);
                    camino.objetivo.GetComponent<Caminable>().bloqueAnterior = cuboActual;
                }
            }

            cubosPasados.Add(cuboActual);

            ExplorarCubo(proximosCubos, cubosPasados);
            ConstruirCamino();

            cuboActual = caminoFinal[0];
        }
    }

    void ExplorarCubo(List<Transform> proximosCubos, List<Transform> cubosVisitados)
    {
        Transform actual = proximosCubos.First();
        proximosCubos.Remove(actual);

        if (actual == cuboSeleccionado)
        {
            return;
        }

        foreach (Camino camino in actual.GetComponent<Caminable>().caminosPosibles)
        {
            if (!cubosVisitados.Contains(camino.objetivo) && camino.activo)
            {
                proximosCubos.Add(camino.objetivo);
                camino.objetivo.GetComponent<Caminable>().bloqueAnterior = actual;
            }
        }

        cubosVisitados.Add(actual);

        if (proximosCubos.Any())
        {
            ExplorarCubo(proximosCubos, cubosVisitados);
        }
    }

    void ConstruirCamino()
    {
        Transform cubo = cuboSeleccionado;

        while (cubo != cuboActual)
        {
            caminoFinal.Add(cubo);

            if (cubo.GetComponent<Caminable>().bloqueAnterior != null)
                cubo = cubo.GetComponent<Caminable>().bloqueAnterior;
            else
                return;
        }

        caminoFinal.Insert(0, cuboSeleccionado);
        SeguirCamino();
    }

    void SeguirCamino()
    {
        Sequence s = DOTween.Sequence();
        caminando = true;

        for (int i = caminoFinal.Count - 1; i > 0; i--)
        {
            float tiempo = caminoFinal[i].GetComponent<Caminable>().esEscalera ? 1.5f : 1;
            Vector3 posicionObjetivo = caminoFinal[i].GetComponent<Caminable>().ObtenerPuntoCaminable();
            Quaternion rotacionObjetivo = caminoFinal[i].GetComponent<Caminable>().ObtenerRotacionDeseada();
            s.Append(transform.DOMove(posicionObjetivo, .2f * tiempo / velocidad).SetEase(Ease.Linear));
            s.Join(transform.DORotateQuaternion(rotacionObjetivo, .2f * tiempo / velocidad)); // Ajusta la rotación del jugador
        }

        if (cuboSeleccionado.GetComponent<Caminable>().esBoton)
        {
            string nombreBoton = cuboSeleccionado.GetComponent<Caminable>().nombreBoton;
            s.AppendCallback(() => PresionarBoton(nombreBoton));
        }

        s.AppendCallback(() => Limpiar());
    }

    void Limpiar()
    {
        foreach (Transform t in caminoFinal)
        {
            t.GetComponent<Caminable>().bloqueAnterior = null;
        }
        caminoFinal.Clear();
        caminando = false;
    }

    public void RayCastAbajo()
    {
        Ray rayoJugador = new Ray(transform.GetChild(0).position, -transform.up);

        RaycastHit[] colliders = Physics.RaycastAll(rayoJugador);
        foreach (var collider in colliders)
        {
            if (collider.transform.TryGetComponent(out Caminable caminable))
            {
                if (caminable.esEscalera)
                {
                    DOVirtual.Float(ObtenerMezcla(), -1f, .1f, EstablecerMezcla);
                }
                else
                {
                    DOVirtual.Float(ObtenerMezcla(), 0, .1f, EstablecerMezcla);
                }

                return;
            }
        }
    }

    float ObtenerMezcla()
    {
        Animator animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator no encontrado en el objeto o en sus hijos.");
            return 0;
        }
        return animator.GetFloat("mezcla");
    }

    void EstablecerMezcla(float x)
    {
        Animator animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator no encontrado en el objeto o en sus hijos.");
            return;
        }
        animator.SetFloat("mezcla", x);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Ray rayo = new Ray(transform.GetChild(0).position, -transform.up);
        Gizmos.DrawRay(rayo);
    }
}
