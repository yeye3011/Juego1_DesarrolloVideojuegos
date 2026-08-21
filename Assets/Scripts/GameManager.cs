using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Puntuación")]
    public int score = 0;
    public int scoreToWin = 100;

    [Header("Tiempo")]
    public float gameTime = 0f;

    [Header("HUD")]
    public TMP_Text scoreText;
    public TMP_Text timeText;

    [Header("Ventana de victoria")]
    public GameObject winPanel;
    public TMP_Text finalTimeText;

    private bool gameFinished = false;

    void Start()
    {
        UpdateScoreUI();
        UpdateTimeUI();

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (!gameFinished)
        {
            gameTime += Time.deltaTime;
            UpdateTimeUI();
        }
    }

    public void AddScore(int amount)
    {
        if (gameFinished)
            return;

        score += amount;

        if (score < 0)
        {
            score = 0;
        }

        UpdateScoreUI();

        if (score >= scoreToWin)
        {
            WinLevel();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + score;
        }
    }

    void UpdateTimeUI()
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);

            timeText.text = string.Format(
                "{0:00}:{1:00}",
                minutes,
                seconds
            );
        }
    }

    void WinLevel()
    {
        gameFinished = true;

        // Mostrar tiempo final
        if (finalTimeText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);

            finalTimeText.text = string.Format(
                "Tiempo: {0:00}:{1:00}",
                minutes,
                seconds
            );
        }

        // Mostrar ventana
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        // Pausar el juego
        Time.timeScale = 0f;
    }
}