using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimacionJugador : MonoBehaviour
{
    public Animator animador;  // Referencia al componente Animator del jugador.
    public ControladorJugador jugador;  // Referencia al ControladorJugador.

    void Start()
    {
        if (jugador == null)
        {
            jugador = GetComponent<ControladorJugador>();
        }
    }

    void Update()
    {
        if (animador != null && jugador != null)
        {
            animador.SetBool("caminando", jugador.caminando);  // Actualiza el estado de la animación "caminando".
            animador.SetFloat("mezcla", jugador.mezcla);  // Actualiza el valor de mezcla.
        }
    }
}
