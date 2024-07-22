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

    // Referencia al transform del jugador
    public Transform jugador;

    // Offset vertical entre la c�mara y el jugador
    public float alturaOffset = 5f;

    // Posici�n inicial de la c�mara
    private Vector3 posicionInicial;

    // Velocidad de ajuste de altura con la rueda del rat�n
    public float velocidadAjusteAltura = 1f;

    void Start()
    {
        // Guardar la rotaci�n inicial de la c�mara
        initialRotation = transform.localRotation;

        // Guardar la posici�n inicial de la c�mara
        if (jugador != null)
        {
            posicionInicial = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
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

        // Ajustar la posici�n vertical de la c�mara en funci�n del jugador
        if (jugador != null)
        {
            Vector3 nuevaPosicion = new Vector3(posicionInicial.x, jugador.position.y + alturaOffset, posicionInicial.z);
            transform.position = nuevaPosicion;
        }

        // Ajustar la altura de la c�mara con la rueda del rat�n
        float rueda = Input.GetAxis("Mouse ScrollWheel");
        if (rueda != 0f)
        {
            alturaOffset += rueda * velocidadAjusteAltura;
        }
    }
}
