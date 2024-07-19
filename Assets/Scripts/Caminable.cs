using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caminable : MonoBehaviour
{
    public string nombreBoton;
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
        // Obtener la posición del punto caminable sin aplicar la rotación del cubo
        Vector3 puntoCaminable = transform.position + Vector3.up * (transform.localScale.y / 2 + offsetPuntoCaminable) - Vector3.up * escalera;

        if (punto)
        {
            puntoCaminable = punto.position;
        }
        return puntoCaminable;
    }

    public Quaternion ObtenerRotacionDeseada()
    {
        if (noRotar)
        {
            return Quaternion.identity; // No hay rotación deseada
        }
        else
        {
            // Obtener la rotación del cubo
            Quaternion rotacionCubo = transform.rotation;
            return rotacionCubo;
        }
    }

    public void ActivarCamino(int indice)
    {
        if (indice >= 0 && indice < caminosPosibles.Count)
        {
            caminosPosibles[indice].activo = true;
            // Lógica para habilitar el camino (puede ser hacer visible el camino, habilitar colisión, etc.)
            caminosPosibles[indice].objetivo.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Índice de camino fuera de rango: " + indice);
        }
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
