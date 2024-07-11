using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    // Velocidad de rotaci�n en grados por segundo
    public Vector3 rotationSpeed = new Vector3(3, 0, 0);

    // Amplitud de la oscilaci�n en cada eje
    public Vector3 rotationAmplitude = new Vector3(10, 0, 10);

    // Tiempo inicial
    private float time;

    // Rotaci�n inicial de la c�mara
    private Quaternion initialRotation;

    void Start()
    {
        // Guardar la rotaci�n inicial de la c�mara
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Incrementar el tiempo
        time += Time.deltaTime;

        // Calcular la rotaci�n en cada eje utilizando una funci�n sinusoidal
        float xRotation = Mathf.Sin(time * rotationSpeed.x) * rotationAmplitude.x;
        float yRotation = Mathf.Sin(time * rotationSpeed.y) * rotationAmplitude.y;
        float zRotation = Mathf.Sin(time * rotationSpeed.z) * rotationAmplitude.z;

        // Aplicar la rotaci�n calculada desde la rotaci�n inicial
        transform.localRotation = initialRotation * Quaternion.Euler(new Vector3(xRotation, yRotation, zRotation));
    }
}
