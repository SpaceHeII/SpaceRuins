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
    public List<BotonMovimiento> botonesMovimiento = new List<BotonMovimiento>();  // Lista de botones y sus movimientos asociados.
    private Dictionary<string, BotonMovimiento> botonesDict = new Dictionary<string, BotonMovimiento>();

    public Transform objetoParaRotar;  // Objeto que se puede rotar con el mouse.
    public bool rotacionVertical = false;  // Determina si la rotaci�n es vertical u horizontal.

    public Transform[] objetosParaOcultar;  // Array de objetos que se ocultan al rotar los pivotes.

    private bool rotando = false;

    private void Awake()
    {
        instancia = this;  // Inicializa la instancia singleton.
        foreach (BotonMovimiento boton in botonesMovimiento)
        {
            botonesDict[boton.nombreBoton] = boton;
        }
    }

    void Update()
    {
        // Comprueba condiciones de caminos y activa/desactiva caminos seg�n corresponda.
        VerificarCondicionesCaminos();

        if (jugador.caminando)
            return;

        // Gesti�n de rotaci�n del objeto con el mouse.
        if (Input.GetMouseButtonDown(0) && objetoParaRotar != null)
        {
            // Inicia la rotaci�n cuando el mouse se presiona sobre el objeto.
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == objetoParaRotar)
                {
                    rotando = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && rotando)
        {
            // Determina el �ngulo de rotaci�n basado en la configuraci�n.
            Vector3 rotationAxis = rotacionVertical ? Vector3.right : Vector3.up;
            float rotationAngle = 90f;

            // Realiza la rotaci�n del objeto.
            objetoParaRotar.DORotate(objetoParaRotar.eulerAngles + rotationAngle * rotationAxis, 0.6f, RotateMode.FastBeyond360).SetEase(Ease.OutBack);

            rotando = false;
        }

        // Oculta o muestra objetos en funci�n del �ngulo del pivote.
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

    public void EjecutarMovimientos(string nombreBoton, float velocidad)
    {
        if (botonesDict.ContainsKey(nombreBoton))
        {
            botonesDict[nombreBoton].EjecutarMovimientos(velocidad);
        }
        else
        {
            Debug.LogWarning("Bot�n '" + nombreBoton + "' no encontrado en la lista de botones.");
        }
    }

    private void VerificarCondicionesCaminos()
    {
        foreach (CondicionCamino cc in condicionesCaminos)
        {
            int contador = 0;
            foreach (Condicion condicion in cc.condiciones)
            {
                if (condicion.objetoCondicion.eulerAngles == condicion.anguloEuler || condicion.objetoCondicion.position == condicion.posicionObjetivo)
                {
                    contador++;
                }
            }

            if (contador == cc.condiciones.Count)
            {
                ActivarCamino(cc);
            }
        }
    }

    private void ActivarCamino(CondicionCamino condicionCamino)
    {
        foreach (CaminoUnico camino in condicionCamino.caminos)
        {
            camino.bloque.ActivarCamino(camino.indice);
        }
    }
}

[System.Serializable]
public class CondicionCamino
{
    public string nombreCondicionCamino;  // Nombre de la condici�n de camino.
    public List<Condicion> condiciones;  // Lista de condiciones.
    public List<CaminoUnico> caminos;  // Lista de caminos �nicos.
}

[System.Serializable]
public class Condicion
{
    public Transform objetoCondicion;  // Objeto que debe cumplir la condici�n.
    public Vector3 anguloEuler;  // �ngulo Euler que debe alcanzar el objeto.
    public Vector3 posicionObjetivo;  // Posici�n que debe alcanzar el objeto.
}

[System.Serializable]
public class CaminoUnico
{
    public Caminable bloque;  // Bloque caminable asociado.
    public int indice;  // �ndice del camino en la lista de caminos del bloque.
}

[System.Serializable]
public class BotonMovimiento
{
    public string nombreBoton;  // Nombre del bot�n.
    public List<MovimientoBloque> movimientos;  // Movimientos de bloques asociados a este bot�n.

    public void EjecutarMovimientos(float velocidad)
    {
        foreach (MovimientoBloque movimiento in movimientos)
        {
            movimiento.velocidadMovimiento = velocidad;  // Asigna la velocidad al movimiento del bloque
            movimiento.MoverBloque();  // Ejecuta el movimiento del bloque con la velocidad especificada
        }
    }
}
