using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel; // Referencia al panel de pausa
    public Button pauseButton; // Referencia al bot�n de pausa en la UI
    public string levelToLoad; // Nombre de la escena a cargar (asignable desde el Inspector)

    private bool isPaused = false; // Variable para rastrear si el juego est� pausado

    void Start()
    {
        // Asegurarse de que el juego no est� inicialmente pausado
        ResumeGame();

        // Aseg�rate de que el bot�n est� configurado para llamar a TogglePause cuando se haga clic
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
        }
        else
        {
            Debug.LogWarning("El bot�n de pausa no est� asignado.");
        }
    }

    void Update()
    {
        // Detectar la entrada de teclado para activar/desactivar la pausa con Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame(); // Si est� pausado, reanudar el juego
        }
        else
        {
            PauseGame(); // Si no est� pausado, pausar el juego
        }
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Congelar el tiempo del juego
        Cursor.visible = true; // Mostrar el cursor del mouse
        Cursor.lockState = CursorLockMode.None; // Desbloquear el cursor
        pausePanel.SetActive(true); // Activar el panel de pausa
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Reanudar el tiempo del juego normalmente
        Cursor.visible = true; // Asegurarse de que el cursor sea visible
        Cursor.lockState = CursorLockMode.None; // Asegurarse de que el cursor est� desbloqueado
        pausePanel.SetActive(false); // Desactivar el panel de pausa
    }

    public void LoadLevel(string levelName)
    {
        // Cargar la escena especificada por levelName
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame()
    {
        // Salir del juego sin confirmaci�n
        Debug.Log("Sali del juego");
        Application.Quit();
    }
}
