using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Velocidad de rotaci�n en grados por segundo
    public Vector3 rotationSpeed = new Vector3(10, 10, 10);

    // Rotaci�n inicial aleatoria
    private Quaternion initialRotation;

    void Start()
    {
        // Asignar una rotaci�n inicial aleatoria sin afectar la posici�n
        initialRotation = Random.rotation;
        transform.localRotation = initialRotation;
    }

    void Update()
    {
        // Rotar el objeto alrededor de sus ejes locales
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
    }
}