using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GestorJuego : MonoBehaviour
{
    public static GestorJuego instancia;

    public ControladorJugador jugador;
    public List<CondicionCamino> condicionesCaminos = new List<CondicionCamino>();
    public List<Transform> pivotes;
    public List<BotonMovimiento> botonesMovimiento = new List<BotonMovimiento>();
    private Dictionary<string, BotonMovimiento> botonesDict = new Dictionary<string, BotonMovimiento>();

    public List<Transform> objetosParaRotarHorizontal = new List<Transform>();
    public List<Transform> objetosParaRotarVertical = new List<Transform>();
    public Transform[] objetosParaOcultar;

    private bool rotandoHorizontal = false;
    private bool rotandoVertical = false;

    [Header("Configuraci�n de Audio")]
    public AudioClip sonidoBoton; // Audio del bot�n
    public AudioClip sonidoRotacion; // Audio de la rotaci�n
    private AudioSource audioSource; // AudioSource para reproducir los sonidos

    [Header("Configuraci�n de Rotaci�n")]
    public float velocidadRotacion = 0.6f; // Velocidad de la rotaci�n, en segundos

    private void Awake()
    {
        instancia = this;
        foreach (BotonMovimiento boton in botonesMovimiento)
        {
            botonesDict[boton.nombreBoton] = boton;
        }
    }

    void Start()
    {
        // Inicializa el AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // No reproducir autom�ticamente al iniciar
    }

    void Update()
    {
        VerificarCondicionesCaminos();

        if (jugador.caminando)
            return;

        // Gesti�n de rotaci�n horizontal de los objetos con el mouse.
        if (Input.GetMouseButtonDown(0) && objetosParaRotarHorizontal.Count > 0)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (objetosParaRotarHorizontal.Contains(hit.transform))
                {
                    rotandoHorizontal = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && rotandoHorizontal)
        {
            foreach (Transform objeto in objetosParaRotarHorizontal)
            {
                RealizarRotacion(objeto, Vector3.up, 90f);
            }
            rotandoHorizontal = false;
        }

        // Gesti�n de rotaci�n vertical de los objetos con el mouse.
        if (Input.GetMouseButtonDown(0) && objetosParaRotarVertical.Count > 0)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (objetosParaRotarVertical.Contains(hit.transform))
                {
                    rotandoVertical = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && rotandoVertical)
        {
            foreach (Transform objeto in objetosParaRotarVertical)
            {
                RealizarRotacionVertical(objeto, Vector3.right, 90f);
            }
            rotandoVertical = false;
        }

        foreach (Transform t in objetosParaOcultar)
        {
            t.gameObject.SetActive(pivotes[0].eulerAngles.y > 45 && pivotes[0].eulerAngles.y < 90 + 45);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }

    private void RealizarRotacion(Transform objeto, Vector3 axis, float angle)
    {
        // Obt�n la rotaci�n actual
        Quaternion currentRotation = objeto.rotation;

        // Calcula la rotaci�n objetivo sumando el �ngulo
        Quaternion targetRotation = currentRotation * Quaternion.Euler(axis * angle);

        // Aplica la rotaci�n en el objeto
        StartCoroutine(RotarObjeto(objeto, targetRotation, velocidadRotacion));

        // Reproduce el sonido de rotaci�n
        if (audioSource != null && sonidoRotacion != null)
        {
            audioSource.PlayOneShot(sonidoRotacion);
        }
    }

    private void RealizarRotacionVertical(Transform objeto, Vector3 axis, float angle)
    {
        // Obt�n la rotaci�n actual
        Quaternion currentRotation = objeto.rotation;

        // Calcula la rotaci�n objetivo sumando el �ngulo
        Quaternion targetRotation = currentRotation * Quaternion.Euler(axis * angle);

        // Aplica la rotaci�n en el objeto
        StartCoroutine(RotarObjeto(objeto, targetRotation, velocidadRotacion));

        // Reproduce el sonido de rotaci�n
        if (audioSource != null && sonidoRotacion != null)
        {
            audioSource.PlayOneShot(sonidoRotacion);
        }
    }

    private IEnumerator RotarObjeto(Transform objeto, Quaternion targetRotation, float duration)
    {
        float elapsedTime = 0f;

        Quaternion initialRotation = objeto.rotation;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            objeto.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
            yield return null;
        }

        objeto.rotation = targetRotation;
    }

    public void EjecutarMovimientos(string nombreBoton, float velocidad)
    {
        if (botonesDict.ContainsKey(nombreBoton))
        {
            botonesDict[nombreBoton].EjecutarMovimientos(velocidad);

            // Reproduce el sonido del bot�n
            if (audioSource != null && sonidoBoton != null)
            {
                audioSource.PlayOneShot(sonidoBoton);
            }
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
    public string nombreCondicionCamino;
    public List<Condicion> condiciones;
    public List<CaminoUnico> caminos;
}

[System.Serializable]
public class Condicion
{
    public Transform objetoCondicion;
    public Vector3 anguloEuler;
    public Vector3 posicionObjetivo;
}

[System.Serializable]
public class CaminoUnico
{
    public Caminable bloque;
    public int indice;
}

[System.Serializable]
public class BotonMovimiento
{
    public string nombreBoton;
    public List<MovimientoBloque> movimientos;

    public void EjecutarMovimientos(float velocidad)
    {
        foreach (MovimientoBloque movimiento in movimientos)
        {
            movimiento.velocidadMovimiento = velocidad;
            movimiento.MoverBloque();
        }
    }
}
