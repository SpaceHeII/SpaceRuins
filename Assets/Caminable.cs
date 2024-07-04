using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caminable : MonoBehaviour
{
    public List<Camino> caminosPosibles = new List<Camino>();  // Lista de caminos posibles desde este bloque.

    [Space]
    public Transform bloqueAnterior;  // Referencia al bloque previo en el camino.

    [Space]
    [Header("Booleanos")]
    public bool esEscalera = false;  // Indica si este bloque es una escalera.
    public bool sueloMovil = false;  // Indica si este bloque es un suelo móvil.
    public bool esBoton;  // Indica si este bloque es un botón.
    public bool noRotar;  // Indica si este bloque no debe rotar.

    [Space]
    [Header("Offsets")]
    public float offsetPuntoCaminable = .5f;  // Desplazamiento para calcular el punto exacto donde el jugador puede caminar.
    public float offsetEscalera = .4f;  // Desplazamiento adicional si el bloque es una escalera.

    public Vector3 ObtenerPuntoCaminable()
    {
        float escalera = esEscalera ? offsetEscalera : 0;
        return transform.position + transform.up * offsetPuntoCaminable - transform.up * escalera;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        float escalera = esEscalera ? .4f : 0;
        Gizmos.DrawSphere(ObtenerPuntoCaminable(), .1f);

        if (caminosPosibles == null)
            return;

        // Dibuja líneas entre este bloque y los bloques a los que está conectado.
        foreach (Camino c in caminosPosibles)
        {
            if (c.objetivo == null)
                return;
            Gizmos.color = c.activo ? Color.black : Color.clear;
            Gizmos.DrawLine(ObtenerPuntoCaminable(), c.objetivo.GetComponent<Caminable>().ObtenerPuntoCaminable());
        }
    }
}

[System.Serializable]
public class Camino
{
    public Transform objetivo;  // Bloque objetivo de este camino.
    public bool activo = true;  // Indica si este camino está activo.
}
