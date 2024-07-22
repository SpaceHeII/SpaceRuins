using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    // Velocidad de rotación en grados por segundo
    public Vector3 rotationSpeed = new Vector3(3, 0, 0);

    // Amplitud de la oscilación en cada eje
    public Vector3 rotationAmplitude = new Vector3(10, 0, 10);

    // Tiempo inicial
    private float time;

    // Rotación inicial de la cámara
    private Quaternion initialRotation;

    // Referencia al transform del jugador
    public Transform jugador;

    // Offset vertical entre la cámara y el jugador
    public float alturaOffset = 5f;

    // Posición inicial de la cámara
    private Vector3 posicionInicial;

    // Velocidad de ajuste de altura con la rueda del ratón
    public float velocidadAjusteAltura = 1f;

    void Start()
    {
        // Guardar la rotación inicial de la cámara
        initialRotation = transform.localRotation;

        // Guardar la posición inicial de la cámara
        if (jugador != null)
        {
            posicionInicial = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
    }

    void Update()
    {
        // Incrementar el tiempo
        time += Time.deltaTime;

        // Calcular la rotación en cada eje utilizando una función sinusoidal
        float xRotation = Mathf.Sin(time * rotationSpeed.x) * rotationAmplitude.x;
        float yRotation = Mathf.Sin(time * rotationSpeed.y) * rotationAmplitude.y;
        float zRotation = Mathf.Sin(time * rotationSpeed.z) * rotationAmplitude.z;

        // Aplicar la rotación calculada desde la rotación inicial
        transform.localRotation = initialRotation * Quaternion.Euler(new Vector3(xRotation, yRotation, zRotation));

        // Ajustar la posición vertical de la cámara en función del jugador
        if (jugador != null)
        {
            Vector3 nuevaPosicion = new Vector3(posicionInicial.x, jugador.position.y + alturaOffset, posicionInicial.z);
            transform.position = nuevaPosicion;
        }

        // Ajustar la altura de la cámara con la rueda del ratón
        float rueda = Input.GetAxis("Mouse ScrollWheel");
        if (rueda != 0f)
        {
            alturaOffset += rueda * velocidadAjusteAltura;
        }
    }
}
