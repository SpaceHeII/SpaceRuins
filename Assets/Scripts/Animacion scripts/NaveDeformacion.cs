using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaveDeformacion : MonoBehaviour
{
    public int numeroDePartes = 4;  // Número de partes en las que se dividirá el objeto
    public float velocidadRotacion = 20f;  // Velocidad de rotación de las partes
    public float radioRotacion = 1f;  // Radio de rotación alrededor del punto central
    public float deformacionFrecuencia = 1f;  // Frecuencia de la deformación
    public float deformacionMagnitud = 0.5f;  // Magnitud de la deformación

    private List<Transform> partes;  // Lista de las partes en las que se ha dividido el objeto
    private Vector3[] verticesOriginales;

    void Start()
    {
        verticesOriginales = GetComponent<MeshFilter>().mesh.vertices;
        CrearPartes();
    }

    void Update()
    {
        DeformarNave();
        RotarPartes();
    }

    void CrearPartes()
    {
        partes = new List<Transform>();

        // Crear las partes y asignarlas a la lista
        for (int i = 0; i < numeroDePartes; i++)
        {
            GameObject parte = GameObject.CreatePrimitive(PrimitiveType.Cube);
            parte.transform.SetParent(transform);
            parte.transform.localPosition = Vector3.zero;
            parte.transform.localScale = transform.localScale / numeroDePartes;
            partes.Add(parte.transform);
        }
    }

    void DeformarNave()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            float deformacion = Mathf.Sin(Time.time * deformacionFrecuencia + verticesOriginales[i].x) * deformacionMagnitud;
            vertices[i] = verticesOriginales[i] + verticesOriginales[i].normalized * deformacion;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }

    void RotarPartes()
    {
        float angulo = 360f / numeroDePartes;  // Ángulo de separación entre las partes

        // Rotar cada parte alrededor del punto central
        for (int i = 0; i < partes.Count; i++)
        {
            float radianes = Mathf.Deg2Rad * (angulo * i + Time.time * velocidadRotacion);
            Vector3 offset = new Vector3(Mathf.Cos(radianes), Mathf.Sin(radianes), 0) * radioRotacion;
            partes[i].localPosition = offset;
            partes[i].Rotate(Vector3.up * velocidadRotacion * Time.deltaTime, Space.World);
        }
    }
}
