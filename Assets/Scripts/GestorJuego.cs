using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GestorJuego : MonoBehaviour
{
    public static GestorJuego instancia;  // Singleton de GestorJuego.

    public ControladorJugador jugador;  // Referencia al ControladorJugador.
    public List<CondicionCamino> condicionesCaminos = new List<CondicionCamino>();  // Lista de condiciones de caminos.
    public List<Transform> pivotes;  // Lista de pivotes para rotar elementos del nivel.

    public Transform[] objetosParaOcultar;  // Array de objetos que se ocultan al rotar los pivotes.

    private void Awake()
    {
        instancia = this;  // Inicializa la instancia singleton.
    }

    void Update()
    {
        // Comprueba condiciones de caminos y activa/desactiva caminos según corresponda.
        foreach (CondicionCamino cc in condicionesCaminos)
        {
            int contador = 0;
            for (int i = 0; i < cc.condiciones.Count; i++)
            {
                if (cc.condiciones[i].objetoCondicion.eulerAngles == cc.condiciones[i].anguloEuler)
                {
                    contador++;
                }
            }
            //foreach (CaminoUnico cu in cc.caminos)
            //    cu.bloque.caminosPosibles[cu.indice].activo = (contador == cc.condiciones.Count);
        }

        if (jugador.caminando)
            return;

        // Gestiona la rotación de pivotes con las teclas de flecha.
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            int multiplicador = Input.GetKey(KeyCode.RightArrow) ? 1 : -1;
            pivotes[0].DOComplete();
            pivotes[0].DORotate(new Vector3(0, 90 * multiplicador, 0), .6f, RotateMode.WorldAxisAdd).SetEase(Ease.OutBack);
        }

        // Oculta o muestra objetos en función del ángulo del pivote.
        foreach (Transform t in objetosParaOcultar)
        {
            t.gameObject.SetActive(pivotes[0].eulerAngles.y > 45 && pivotes[0].eulerAngles.y < 90 + 45);
        }

        // Reinicia la escena al presionar "R".
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }

    public void RotarPivoteDerecho()
    {
        pivotes[1].DOComplete();
        pivotes[1].DORotate(new Vector3(0, 0, 90), .6f).SetEase(Ease.OutBack);  // Rota el pivote derecho.
    }
}

[System.Serializable]
public class CondicionCamino
{
    public string nombreCondicionCamino;  // Nombre de la condición de camino.
    public List<Condicion> condiciones;  // Lista de condiciones.
    public List<CaminoUnico> caminos;  // Lista de caminos únicos.
}

[System.Serializable]
public class Condicion
{
    public Transform objetoCondicion;  // Objeto que debe cumplir la condición.
    public Vector3 anguloEuler;  // Ángulo Euler que debe alcanzar el objeto.
}

[System.Serializable]
public class CaminoUnico
{
    public Caminable bloque;  // Bloque caminable asociado.
    public int indice;  // Índice del camino en la lista de caminos del bloque.
}
