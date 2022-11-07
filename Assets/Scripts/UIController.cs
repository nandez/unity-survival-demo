using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private List<GameObject> healthIcons;

    [Header("Timer Settings")]
    [SerializeField] private Text timeText;
    [SerializeField] private float timeRemaining = 10;

    [Header("Events")]
    [Tooltip("Evento que se dispara cuando el timer expira.")]
    public UnityEvent OnTimerExpired;

    private bool timerIsRunning = false;

    void Start()
    {
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

    private void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
