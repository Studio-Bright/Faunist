using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Если используешь UI Text
using TMPro; // Если используешь TextMeshPro

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timer;

    public float lifeTime = 600f; // 10 минут
    private float gameTime;

    private void Update()
    {
        gameTime += Time.deltaTime;

        if (gameTime >= 1f)
        {
            lifeTime -= 1f;
            gameTime = 0f;
        }

        if (lifeTime < 0)
            lifeTime = 0;

        // ⏱ перевод в формат 10:00
        int minutes = Mathf.FloorToInt(lifeTime / 60);
        int seconds = Mathf.FloorToInt(lifeTime % 60);

        timer.text = $"Time left until the end of the level: {minutes:00}:{seconds:00}";

        // 🎨 цвет
        if (lifeTime <= 3)
        {
            timer.color = Color.red;
        }
        else if (lifeTime <= 5)
        {
            timer.color = Color.yellow;
        }
    }
}