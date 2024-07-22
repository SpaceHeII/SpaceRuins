using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AjustarAlturaCamara : MonoBehaviour
{
    // Referencia al bot�n que activa el ajuste de altura
    public GameObject botonActivador;

    // La cantidad que se sumar� al offset de la c�mara
    public float cantidadAjuste = 10f;

    // Referencia al script de la c�mara
    public RotateCamera camaraScript;

    private bool botonActivado = false;

    void Update()
    {
        // Verificar si el bot�n ha sido activado
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
            botonActivado = false; // Restablecer la activaci�n del bot�n si no est� activo
        }
    }

    void AjustarAltura()
    {
        if (camaraScript != null)
        {
            // Ajustar el offset de la c�mara sumando la cantidad especificada
            camaraScript.alturaOffset += cantidadAjuste;
        }
        else
        {
            Debug.LogWarning("El script de la c�mara no est� asignado.");
        }
    }
}
