using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

[SelectionBase]
public class ControladorJugador : MonoBehaviour
{
    public bool caminando = false;  // Indica si el jugador está caminando.

    [Space]
    public Transform cuboActual;  // Bloque actual bajo el jugador.
    public Transform cuboSeleccionado;  // Bloque seleccionado por el jugador.
    public Transform indicador;  // Indicador visual de la selección.

    [Space]
    public List<Transform> caminoFinal = new List<Transform>();  // Lista de bloques que forman el camino a seguir.

    private float mezcla;  // Parámetro para la animación de movimiento.

    void Start()
    {
        RayCastAbajo();  // Inicializa el cuboActual.
    }

    void Update()
    {
        RayCastAbajo();  // Actualiza el cuboActual bajo el jugador.

        // Si el cubo actual es un suelo móvil, se establece como padre del jugador.
        if (cuboActual.GetComponent<Caminable>().sueloMovil)
        {
            transform.parent = cuboActual.parent;
        }
        else
        {
            transform.parent = null;
        }

        // Detecta clic en un cubo.
        if (Input.GetMouseButtonDown(0))
        {
            Ray rayoMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit golpeMouse;

            if (Physics.Raycast(rayoMouse, out golpeMouse))
            {
                if (golpeMouse.transform.GetComponent<Caminable>() != null)
                {
                    cuboSeleccionado = golpeMouse.transform;
                    DOTween.Kill(gameObject.transform);  // Detiene cualquier movimiento previo.
                    caminoFinal.Clear();  // Limpia el camino previo.
                    EncontrarCamino();  // Encuentra el nuevo camino.

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
            s.Append(transform.DOMove(caminoFinal[i].GetComponent<Caminable>().ObtenerPuntoCaminable(), .2f * tiempo).SetEase(Ease.Linear));

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
            if (golpeJugador.transform.GetComponent<Caminable>() != null)
            {
                cuboActual = golpeJugador.transform;

                if (golpeJugador.transform.GetComponent<Caminable>().esEscalera)
                {
                    DOVirtual.Float(ObtenerMezcla(), mezcla, .1f, EstablecerMezcla);
                }
                else
                {
                    DOVirtual.Float(ObtenerMezcla(), 0, .1f, EstablecerMezcla);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Ray rayo = new Ray(transform.GetChild(0).position, -transform.up);
        Gizmos.DrawRay(rayo);  // Dibuja el rayo en la vista del editor.
    }

    float ObtenerMezcla()
    {
        return GetComponentInChildren<Animator>().GetFloat("Mezcla");  // Obtiene el valor actual de la mezcla de animación.
    }

    void EstablecerMezcla(float x)
    {
        GetComponentInChildren<Animator>().SetFloat("Mezcla", x);  // Establece el valor de la mezcla de animación.
    }
}
