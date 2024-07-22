using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AjustarAlturaCamara : MonoBehaviour
{
    // Referencia al botón que activa el ajuste de altura
    public GameObject botonActivador;

    // La cantidad que se sumará al offset de la cámara
    public float cantidadAjuste = 10f;

    // Referencia al script de la cámara
    public RotateCamera camaraScript;

    private bool botonActivado = false;

    void Update()
    {
        // Verificar si el botón ha sido activado
        if (botonActivador != null && botonActivador.activeSelf)
        {
            if (!botonActivado) // Asegurarse de que solo se ejecute una vez
            {
                botonActivado = true;
                AjustarAltura();
            }
        }
        else
        {
            botonActivado = false; // Restablecer la activación del botón si no está activo
        }
    }

    void AjustarAltura()
    {
        if (camaraScript != null)
        {
            // Ajustar el offset de la cámara sumando la cantidad especificada
            camaraScript.alturaOffset += cantidadAjuste;
        }
        else
        {
            Debug.LogWarning("El script de la cámara no está asignado.");
        }
    }
}
