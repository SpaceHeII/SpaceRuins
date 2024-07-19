using UnityEngine;
using DG.Tweening;

public class MovimientoBloque : MonoBehaviour
{
    public Transform bloque;  // El bloque que se va a mover.
    public Vector3 offsetPosicion;  // El offset de posición al que se va a mover el bloque.
    public bool rotar;  // Si el bloque debe rotar.
    public Vector3 rotacionObjetivo;  // La rotación objetivo si el bloque debe rotar.
    public float velocidadMovimiento = 1.0f;  // Velocidad ajustable del movimiento del bloque.

    public void MoverBloque()
    {
        Vector3 posicionObjetivo = bloque.position + offsetPosicion;  // Calcula la nueva posición
        bloque.DOMove(posicionObjetivo, velocidadMovimiento).SetEase(Ease.OutBack);  // Mueve el bloque a la nueva posición con la velocidad ajustable

        if (rotar)
        {
            bloque.DORotate(rotacionObjetivo, velocidadMovimiento).SetEase(Ease.OutBack);  // Rota el bloque si es necesario con la velocidad ajustable
        }
    }
}