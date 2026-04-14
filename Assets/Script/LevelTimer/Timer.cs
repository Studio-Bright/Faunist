using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timer;

    private float lifeTime;
    private float gameTime;
    private bool isRunning;

    public Action OnTimerFinished;

    public void StartTimer(float duration)
    {
        lifeTime = duration;
        gameTime = 0f;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    private void Update()
    {
        if (!isRunning) return;

        gameTime += Time.deltaTime;

        if (gameTime >= 1f)
        {
            lifeTime -= 1f;
            gameTime = 0f;
        }

        if (lifeTime <= 0)
        {
            lifeTime = 0;
            isRunning = false;
            OnTimerFinished?.Invoke();
        }

        int minutes = Mathf.FloorToInt(lifeTime / 60);
        int seconds = Mathf.FloorToInt(lifeTime % 60);

        timer.text = $"Time left: {minutes:00}:{seconds:00}";

        
    }
}