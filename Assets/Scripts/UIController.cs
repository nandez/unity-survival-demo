using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private List<GameObject> healthIcons;
    [SerializeField] private Text spearsText;
    [SerializeField] private Text rocksText;
    [SerializeField] private GameObject pauseMenu;

    [Header("Timer Settings")]
    [SerializeField] private Text timeText;
    [SerializeField] private float timeRemaining = 10;

    [Header("Events")]
    //Evento que se dispara cuando el timer expira.
    public UnityEvent OnTimerExpired;

    private bool timerIsRunning = false;

    void Start()
    {
        Time.timeScale = 1;
        timerIsRunning = true;
    }


    private void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                OnTimerExpired?.Invoke();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Cuando el jugador pulsa la tecla escape, activamos / ocultamos el menu de pausa
            // y seteamos el estado del juego en pausa / ejecucion utilizando la escala de tiempo.
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
        }
    }

    public void OnUpdateGameResourceEventHandler(GameResource resource)
    {
        if (resource == null)
            return;

        resource.resourceUiText.text = $"{resource.GetCurrentAmount()} / {resource.GetMaxAmount()}";
    }

    public void OnPlayerDeathHandler(int remainingLives)
    {
        // Desactivamos los iconos de vida.
        for (int i = healthIcons.Count; i > remainingLives; i--)
            healthIcons[i - 1].SetActive(false);
    }

    public void OnAmmoReplenished(int spearsCount, int rockCount)
    {
        spearsText.text = spearsCount.ToString();
        rocksText.text = rockCount.ToString();
    }

    public void OnResumeGameHandler()
    {
        // Cuando el jugador pulsa el boton de reanudar, ocultamos el menu de pausa
        // y seteamos el estado del juego en ejecucion utilizando la escala de tiempo.
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnBackToMainMenuHandler()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
