using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CargarEscenaConDelay : MonoBehaviour
{
    // Tiempo de espera antes de cargar la nueva escena (en segundos)
    public float tiempoEspera = 5f;

    // Nombre de la escena a cargar después del tiempo de espera
    public string escenaParaCargar;

    void Start()
    {
        // Iniciar la corutina para cargar la escena después del tiempo de espera
        StartCoroutine(CargarEscenaConDelayCoroutine());
    }

    private IEnumerator CargarEscenaConDelayCoroutine()
    {
        // Esperar el tiempo especificado
        yield return new WaitForSeconds(tiempoEspera);

        // Cargar la nueva escena
        SceneManager.LoadScene(escenaParaCargar);
    }
}
