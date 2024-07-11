using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Velocidad de rotación en grados por segundo
    public Vector3 rotationSpeed = new Vector3(10, 10, 10);

    // Rotación inicial aleatoria
    private Quaternion initialRotation;

    void Start()
    {
        // Asignar una rotación inicial aleatoria sin afectar la posición
        initialRotation = Random.rotation;
        transform.localRotation = initialRotation;
    }

    void Update()
    {
        // Rotar el objeto alrededor de sus ejes locales
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
    }
}