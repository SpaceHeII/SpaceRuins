using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirarHaciaDondeAvanza : MonoBehaviour
{
    Vector3 lastPosition;
    public float rotacionSuave = 10f; // Ajusta este valor para suavizar la rotación

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 direccion = transform.position - lastPosition;
        if (direccion.magnitude > 0.01f) // Umbral para evitar movimientos menores
        {
            Quaternion targetRotation = Quaternion.LookRotation(direccion.normalized); // Asegura que la dirección esté normalizada
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotacionSuave);
        }
        lastPosition = transform.position;
    }
}
