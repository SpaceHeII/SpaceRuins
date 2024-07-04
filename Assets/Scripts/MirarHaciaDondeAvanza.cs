using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirarHaciaDondeAvanza : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 lastPosition;
    void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direccion = transform.position - lastPosition;
        direccion.Normalize();
        if(direccion != Vector3.zero)
        {
            transform.forward = direccion;
        }
       
        lastPosition = transform.position;
    }
}
