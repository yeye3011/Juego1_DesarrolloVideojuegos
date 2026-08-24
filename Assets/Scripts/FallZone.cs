using UnityEngine;

public class FallZone : MonoBehaviour
{
    public Transform respawnPoint;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // =========================
        // PLAYER
        // =========================

        PlayerMovement player =
            other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            if (gameManager != null)
            {
                bool canContinue = gameManager.LoseLife();

                if (canContinue)
                {
                    RespawnPlayer(player);
                }
            }

            return;
        }

        // =========================
        // OBJETOS QUE CAEN
        // =========================

        FallingObject fallingObject =
            other.GetComponent<FallingObject>();

        if (fallingObject != null)
        {
            // Si era positivo, cuenta como objeto perdido
            if (fallingObject.points > 0 && gameManager != null)
            {
                gameManager.PositiveObjectMissed();
            }

            Destroy(fallingObject.gameObject);
        }
    }

    private void RespawnPlayer(PlayerMovement player)
    {
        Rigidbody playerRb =
            player.GetComponent<Rigidbody>();

        if (playerRb == null)
            return;

        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        playerRb.position = respawnPoint.position;
    }
}