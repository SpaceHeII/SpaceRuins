using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

[SelectionBase]
public class ControladorJugador : MonoBehaviour
{
    public bool caminando = false;
    public float velocidad = 1.0f; // Velocidad ajustable del jugador.
    public float cooldownTiempo = 0.5f; // Cooldown ajustable para el click izquierdo.
    private float tiempoUltimoClick = 0f;

    [Space]
    public Transform cuboActual;  // Asegúrate de asignar este Transform en el inspector.
    public Transform cuboSeleccionado;
    public Transform indicador;

    [Space]
    public List<Transform> caminoFinal = new List<Transform>();

    private float mezcla;

    void Start()
    {
        // Verificar si cuboActual está asignado
        if (cuboActual == null)
        {
            Debug.LogError("cuboActual no está asignado en el inspector.");
        }

        RayCastAbajo();
    }

    void Update()
    {
        // OBTENER CUBO ACTUAL (DEBAJO DEL JUGADOR)
        RayCastAbajo();

        if (cuboActual != null && cuboActual.GetComponent<Caminable>().sueloMovil)
        {
            transform.parent = cuboActual.parent;
        }
        else
        {
            transform.parent = null;
        }

        // CLIC EN CUBO
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
                    DOTween.Kill(gameObject.transform); // Cancela cualquier animación en curso
                    caminoFinal.Clear();
                    EncontrarCamino();

                    mezcla = transform.position.y - cuboSeleccionado.position.y > 0 ? -1 : 1;

                    indicador.position = golpeMouse.transform.GetComponent<Caminable>().ObtenerPuntoCaminable();
                    Sequence s = DOTween.Sequence();
                    s.AppendCallback(() => indicador.GetComponentInChildren<ParticleSystem>().Play());
                    s.Append(indicador.GetComponent<Renderer>().material.DOColor(Color.white, .1f));
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

        // Añade los caminos posibles desde el cubo actual.
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

            ExplorarCubo(proximosCubos, cubosPasados);  // Explora los cubos siguientes.
            ConstruirCamino();  // Construye el camino final.
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

        // Añade caminos posibles desde el cubo actual.
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
            caminoFinal.Add(cubo);  // Añade el cubo al camino final.
            if (cubo.GetComponent<Caminable>().bloqueAnterior != null)
                cubo = cubo.GetComponent<Caminable>().bloqueAnterior;
            else
                return;
        }

        caminoFinal.Insert(0, cuboSeleccionado);
        SeguirCamino();  // Sigue el camino final.
    }

    void SeguirCamino()
    {
        Sequence s = DOTween.Sequence();
        caminando = true;

        // Anima el movimiento del jugador a través del camino final.
        for (int i = caminoFinal.Count - 1; i > 0; i--)
        {
            float tiempo = caminoFinal[i].GetComponent<Caminable>().esEscalera ? 1.5f : 1;
            s.Append(transform.DOMove(caminoFinal[i].GetComponent<Caminable>().ObtenerPuntoCaminable(), .2f * tiempo / velocidad).SetEase(Ease.Linear));

            if (!caminoFinal[i].GetComponent<Caminable>().noRotar)
                s.Join(transform.DOLookAt(caminoFinal[i].position, .1f, AxisConstraint.Y, Vector3.up));
        }

        // Si el cubo seleccionado es un botón, ejecuta una acción.
        if (cuboSeleccionado.GetComponent<Caminable>().esBoton)
        {
            s.AppendCallback(() => GestorJuego.instancia.RotarPivoteDerecho());
        }

        s.AppendCallback(() => Limpiar());  // Limpia las referencias después de completar el movimiento.
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
        RaycastHit golpeJugador;

        // Raycast para detectar el cubo actual bajo el jugador.
        if (Physics.Raycast(rayoJugador, out golpeJugador))
        {
            Caminable caminable = golpeJugador.transform.GetComponent<Caminable>();
            if (caminable != null)
            {
                cuboActual = golpeJugador.transform;

                if (caminable.esEscalera)
                {
                    DOVirtual.Float(ObtenerMezcla(), mezcla, .1f, EstablecerMezcla);
                }
                else
                {
                    DOVirtual.Float(ObtenerMezcla(), 0, .1f, EstablecerMezcla);
                }
            }
            else
            {
                Debug.LogError("El objeto golpeado no tiene un componente Caminable.");
            }
        }
    }

    float ObtenerMezcla()
    {
        // Verificar si el Animator está asignado
        Animator animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator no encontrado en el objeto o en sus hijos.");
            return 0; // Devuelve un valor predeterminado si el Animator no se encuentra
        }

        return animator.GetFloat("Mezcla");
    }

    void EstablecerMezcla(float x)
    {
        // Verificar si el Animator está asignado
        Animator animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator no encontrado en el objeto o en sus hijos.");
            return; // Salir de la función si el Animator no se encuentra
        }

        animator.SetFloat("Mezcla", x);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Ray rayo = new Ray(transform.GetChild(0).position, -transform.up);
        Gizmos.DrawRay(rayo);  // Dibuja el rayo en la vista del editor.
    }
}
