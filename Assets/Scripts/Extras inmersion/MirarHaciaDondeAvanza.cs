using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirarHaciaDondeAvanza : MonoBehaviour
{
    public Transform child;
    Vector3 lastPosition;
    public float rotacionSuave = 10f; // Ajusta este valor para suavizar la rotación
    Quaternion targetRotation;

    void Start()
    {
        lastPosition = child.position;
    }

    void Update()
    {
        Vector3 direccion = child.position - lastPosition;
        if (direccion.magnitude > 0.01f) // Umbral para evitar movimientos menores
        {
            targetRotation = Quaternion.LookRotation(direccion.normalized); // Asegura que la dirección esté normalizada
        }

        child.localRotation = Quaternion.Slerp(child.localRotation, targetRotation, Time.deltaTime * rotacionSuave);
        child.localEulerAngles = new Vector3(0, child.localEulerAngles.y, 0);
        lastPosition = child.position;
    }
}
