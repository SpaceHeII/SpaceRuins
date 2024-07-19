using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravedadPersonalizada : MonoBehaviour
{
    private Vector3 gravedadActual = Vector3.down;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No se encontró un Rigidbody en el objeto.");
        }
        else
        {
            rb.useGravity = false;
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.AddForce(gravedadActual * 9.81f, ForceMode.Acceleration);
        }
    }

    public void ActualizarGravedad(Vector3 nuevaGravedad)
    {
        gravedadActual = -nuevaGravedad;
        OrientarJugador(nuevaGravedad);
    }

    private void OrientarJugador(Vector3 normalSuperficie)
    {
        Quaternion rotacionDeseada = Quaternion.FromToRotation(transform.up, normalSuperficie) * transform.rotation;
        transform.rotation = rotacionDeseada;
    }
}
