using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // =========================================================
    // TIPO DE POWER-UP
    // =========================================================

    public enum PowerUpType
    {
        None,
        GroundFill,
        Shield
    }


    // =========================================================
    // PUNTUACIÓN
    // =========================================================

    [Header("Puntuación")]
    public int score = 0;
    public int scoreToWin = 100;


    // =========================================================
    // TIEMPO
    // =========================================================

    [Header("Tiempo")]
    public float gameTime = 0f;


    // =========================================================
    // HUD
    // =========================================================

    [Header("HUD")]
    public TMP_Text scoreText;
    public TMP_Text timeText;


    // =========================================================
    // VENTANA DE VICTORIA
    // =========================================================

    [Header("Ventana de victoria")]
    public GameObject winPanel;
    public TMP_Text finalTimeText;


    // =========================================================
    // VENTANA DE DERROTA
    // =========================================================

    [Header("Ventana de derrota")]
    public GameObject gameOverPanel;


    // =========================================================
    // VIDAS
    // =========================================================

    [Header("Vidas")]
    public int lives = 3;

    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;


    // =========================================================
    // OBJETOS POSITIVOS PERDIDOS
    // =========================================================

    [Header("Objetos positivos perdidos")]
    public int missedPositiveObjects = 0;
    public int missedObjectsToLoseLife = 5;

    private bool processingMissedPenalty = false;

    public Image missedObject1;
    public Image missedObject2;
    public Image missedObject3;
    public Image missedObject4;
    public Image missedObject5;


    // =========================================================
    // POWER-UP GENERAL
    // =========================================================

    [Header("Power-Up General")]

    public PowerUpType powerUpType = PowerUpType.None;

    public Image powerUpBar;

    public int powerUpPoints = 0;
    public int powerUpPointsNeeded = 60;

    public Button powerUpButton;

    [Header("Audio Power-Up")]
    public AudioSource powerUpAudioSource;
    public AudioClip powerUpSound;

    [Tooltip("Duración total del Power-Up")]
    public float powerUpDuration = 5f;

    private bool powerUpReady = false;


    // =========================================================
    // POWER-UP NIVEL 2 - SUELO
    // =========================================================

    [Header("Power-Up Nivel 2 - Suelo")]

    public GameObject powerUpGround;

    [Tooltip("Cuánto tiempo antes de desaparecer comienza la advertencia")]
    public float warningDuration = 2f;

    [Tooltip("Velocidad del cambio de color durante la advertencia")]
    public float blinkSpeed = 0.2f;

    [Tooltip("Color de advertencia antes de desaparecer")]
    public Color warningColor = Color.red;


    // =========================================================
    // POWER-UP NIVEL 3 - ESCUDO
    // =========================================================

    [Header("Power-Up Nivel 3 - Escudo")]

    public GameObject shieldVisual;

    private bool shieldActive = false;


    // =========================================================
    // ESTADO GENERAL
    // =========================================================

    private bool gameFinished = false;


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        Time.timeScale = 1f;

        UpdateScoreUI();
        UpdateTimeUI();
        UpdateLivesUI();
        UpdateMissedObjectsUI();

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        SetupPowerUp();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        if (!gameFinished)
        {
            gameTime += Time.deltaTime;
            UpdateTimeUI();
        }
    }


    // =========================================================
    // PUNTUACIÓN
    // =========================================================

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


    // =========================================================
    // TIEMPO
    // =========================================================

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


    // =========================================================
    // VICTORIA
    // =========================================================

    void WinLevel()
    {
        gameFinished = true;

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

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }


    public void NextLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex + 1
        );
    }


    // =========================================================
    // VIDAS
    // =========================================================

    public bool LoseLife()
    {
        if (gameFinished)
            return false;

        lives--;

        if (lives < 0)
        {
            lives = 0;
        }

        UpdateLivesUI();

        if (lives <= 0)
        {
            GameOver();
            return false;
        }

        return true;
    }


    void UpdateLivesUI()
    {
        if (heart1 != null)
            heart1.SetActive(lives >= 1);

        if (heart2 != null)
            heart2.SetActive(lives >= 2);

        if (heart3 != null)
            heart3.SetActive(lives >= 3);
    }


    void GameOver()
    {
        gameFinished = true;

        Debug.Log("GAME OVER");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }


    // =========================================================
    // OBJETOS POSITIVOS PERDIDOS
    // =========================================================

    public void PositiveObjectMissed()
    {
        if (gameFinished || processingMissedPenalty)
            return;

        missedPositiveObjects++;

        Debug.Log(
            "Objetos positivos perdidos: " +
            missedPositiveObjects +
            "/" +
            missedObjectsToLoseLife
        );

        UpdateMissedObjectsUI();

        if (missedPositiveObjects >= missedObjectsToLoseLife)
        {
            StartCoroutine(MissedObjectsPenaltyRoutine());
        }
    }


    private IEnumerator MissedObjectsPenaltyRoutine()
    {
        processingMissedPenalty = true;

        // Mantener los 5 indicadores visibles
        yield return new WaitForSeconds(1f);

        // Quitar una vida
        LoseLife();

        // Mostrar el cambio de vida durante un momento
        yield return new WaitForSeconds(0.5f);

        // Reiniciar indicadores
        missedPositiveObjects = 0;
        UpdateMissedObjectsUI();

        processingMissedPenalty = false;
    }


    void UpdateMissedObjectsUI()
    {
        UpdateMissedIcon(
            missedObject1,
            missedPositiveObjects >= 1
        );

        UpdateMissedIcon(
            missedObject2,
            missedPositiveObjects >= 2
        );

        UpdateMissedIcon(
            missedObject3,
            missedPositiveObjects >= 3
        );

        UpdateMissedIcon(
            missedObject4,
            missedPositiveObjects >= 4
        );

        UpdateMissedIcon(
            missedObject5,
            missedPositiveObjects >= 5
        );
    }


    void UpdateMissedIcon(Image icon, bool active)
    {
        if (icon == null)
            return;

        Color color = icon.color;

        if (active)
        {
            color.a = 1f;
        }
        else
        {
            color.a = 0.25f;
        }

        icon.color = color;
    }


    // =========================================================
    // CONFIGURACIÓN DEL POWER-UP
    // =========================================================

    void SetupPowerUp()
    {
        powerUpPoints = 0;
        powerUpReady = false;
        shieldActive = false;

        // Barra vacía
        if (powerUpBar != null)
        {
            powerUpBar.fillAmount = 0f;
        }

        // Botón inicialmente desactivado
        if (powerUpButton != null)
        {
            powerUpButton.interactable = false;
        }

        // Suelo temporal oculto
        if (powerUpGround != null)
        {
            powerUpGround.SetActive(false);
        }

        // Escudo oculto
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }


    // =========================================================
    // CARGAR POWER-UP
    // =========================================================

    public void AddPowerUpPoints(int amount)
    {
        // Nivel sin Power-Up
        if (powerUpType == PowerUpType.None)
            return;

        if (gameFinished || powerUpReady)
            return;

        // Los negativos no cargan la barra
        if (amount <= 0)
            return;

        powerUpPoints += amount;

        if (powerUpPoints >= powerUpPointsNeeded)
        {
            powerUpPoints = powerUpPointsNeeded;
            powerUpReady = true;

            if (powerUpButton != null)
            {
                powerUpButton.interactable = true;
            }

            Debug.Log("POWER-UP LISTO");
        }

        if (powerUpBar != null)
        {
            powerUpBar.fillAmount =
                (float)powerUpPoints /
                powerUpPointsNeeded;
        }
    }


    // =========================================================
    // BOTÓN POWER-UP
    // =========================================================

    public void ActivatePowerUp()
    {
        Debug.Log("BOTÓN POWER-UP PRESIONADO");

        if (!powerUpReady || gameFinished)
            return;

        // NIVEL 2
        if (powerUpType == PowerUpType.GroundFill)
        {
            StartCoroutine(GroundPowerUpRoutine());
        }

        // NIVEL 3
        else if (powerUpType == PowerUpType.Shield)
        {
            StartCoroutine(ShieldPowerUpRoutine());
        }
    }


    // =========================================================
    // PREPARAR USO DEL POWER-UP
    // =========================================================

    void ConsumePowerUp()
    {
        powerUpReady = false;
        powerUpPoints = 0;

        if (powerUpButton != null)
        {
            powerUpButton.interactable = false;
        }

        if (powerUpBar != null)
        {
            powerUpBar.fillAmount = 0f;
        }

        // Reproducir sonido del Power-Up
        if (powerUpAudioSource != null && powerUpSound != null)
        {
            powerUpAudioSource.PlayOneShot(powerUpSound);
        }
    }


    // =========================================================
    // POWER-UP NIVEL 2
    // RELLENAR HUECO
    // =========================================================

    private IEnumerator GroundPowerUpRoutine()
    {
        ConsumePowerUp();

        if (powerUpGround == null)
            yield break;

        // Activar el suelo completo.
        // El collider permanecerá activo durante TODO el Power-Up.
        powerUpGround.SetActive(true);

        // Obtener solamente la parte visual.
        Renderer groundRenderer =
            powerUpGround.GetComponent<Renderer>();

        Debug.Log("POWER-UP SUELO ACTIVADO");


        // =====================================================
        // TIEMPO NORMAL
        // =====================================================

        float normalDuration =
            Mathf.Max(
                0f,
                powerUpDuration - warningDuration
            );

        yield return new WaitForSeconds(normalDuration);


        // =====================================================
        // ADVERTENCIA
        // SOLO PARPADEA EL RENDERER
        // =====================================================

        Debug.Log(
            "ADVERTENCIA: EL SUELO VA A DESAPARECER"
        );

        float warningTimer = 0f;

        while (warningTimer < warningDuration)
        {
            // Ocultar SOLO la imagen del suelo.
            // El Box Collider sigue activo.
            if (groundRenderer != null)
            {
                groundRenderer.enabled = false;
            }

            yield return new WaitForSeconds(blinkSpeed);


            // Volver a mostrar SOLO la imagen.
            if (groundRenderer != null)
            {
                groundRenderer.enabled = true;
            }

            yield return new WaitForSeconds(blinkSpeed);

            warningTimer += blinkSpeed * 2f;
        }


        // Asegurarnos de dejar visible el Renderer
        // antes de desactivar el objeto completo.
        if (groundRenderer != null)
        {
            groundRenderer.enabled = true;
        }


        // =====================================================
        // TERMINAR POWER-UP
        // =====================================================

        // Ahora sí desaparecen juntos:
        // visual + collider.
        powerUpGround.SetActive(false);

        Debug.Log("POWER-UP SUELO TERMINADO");
    }


    // =========================================================
    // POWER-UP NIVEL 3
    // ESCUDO
    // =========================================================

    private IEnumerator ShieldPowerUpRoutine()
    {
        ConsumePowerUp();

        shieldActive = true;

        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
        }

        Debug.Log("ESCUDO ACTIVADO");

        // ==========================================
        // OBTENER MATERIAL DEL ESCUDO
        // ==========================================

        Renderer shieldRenderer = null;
        Color originalColor = Color.white;

        if (shieldVisual != null)
        {
            shieldRenderer =
                shieldVisual.GetComponent<Renderer>();

            if (shieldRenderer != null)
            {
                originalColor =
                    shieldRenderer.material.color;
            }
        }


        // ==========================================
        // TIEMPO NORMAL
        // ==========================================

        // Ejemplo:
        // Power Up Duration = 8
        // Warning Duration = 2
        //
        // 6 segundos normal
        // 2 segundos de advertencia

        float normalDuration =
            Mathf.Max(
                0f,
                powerUpDuration - warningDuration
            );

        yield return new WaitForSeconds(
            normalDuration
        );


        // ==========================================
        // ADVERTENCIA
        // ==========================================

        Debug.Log(
            "ADVERTENCIA: EL ESCUDO VA A TERMINAR"
        );

        float warningTimer = 0f;

        while (warningTimer < warningDuration)
        {
            if (shieldRenderer != null)
            {
                // Hacer la burbuja más transparente
                Color fadedColor = originalColor;

                fadedColor.a =
                    originalColor.a * 0.2f;

                shieldRenderer.material.color =
                    fadedColor;
            }

            yield return new WaitForSeconds(
                blinkSpeed
            );


            if (shieldRenderer != null)
            {
                // Recuperar apariencia normal
                shieldRenderer.material.color =
                    originalColor;
            }

            yield return new WaitForSeconds(
                blinkSpeed
            );

            warningTimer += blinkSpeed * 2f;
        }


        // ==========================================
        // RESTAURAR APARIENCIA
        // ==========================================

        if (shieldRenderer != null)
        {
            shieldRenderer.material.color =
                originalColor;
        }


        // ==========================================
        // TERMINAR ESCUDO
        // ==========================================

        shieldActive = false;

        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }

        Debug.Log("ESCUDO TERMINADO");
    }


    // =========================================================
    // CONSULTAR SI EL ESCUDO ESTÁ ACTIVO
    // =========================================================

    public bool IsShieldActive()
    {
        return shieldActive;
    }


    // =========================================================
    // REINICIAR NIVEL
    // =========================================================

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}