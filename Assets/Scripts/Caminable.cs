using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caminable : MonoBehaviour
{
    public List<Camino> caminosPosibles = new List<Camino>();

    [Space]
    public Transform bloqueAnterior;

    [Space]
    [Header("Booleanos")]
    public bool esEscalera = false;
    public bool sueloMovil = false;
    public bool esBoton;
    public bool noRotar;

    [Space]
    [Header("Offsets")]
    public float offsetPuntoCaminable = .5f;
    public float offsetEscalera = .4f;
    public Transform punto;
    public Vector3 ObtenerPuntoCaminable()
    {
        float escalera = esEscalera ? offsetEscalera : 0;
        // Colocar el punto en el centro de la cara superior del cubo
        Vector3 puntoCaminable = transform.position + Vector3.up * (transform.localScale.y / 2 + offsetPuntoCaminable) - Vector3.up * escalera;
        if(punto)
        {
            puntoCaminable = punto.position;
        }
        return puntoCaminable;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        float escalera = esEscalera ? .4f : 0;
        Gizmos.DrawSphere(ObtenerPuntoCaminable(), .1f);

        if (caminosPosibles == null)
            return;

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
    public Transform objetivo;
    public bool activo = true;
}
