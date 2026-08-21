using UnityEngine;

public class FallingObject : MonoBehaviour
{
    [Header("Caída")]
    public float fallSpeed = 3f;

    [Header("Puntuación")]
    public int points = 10;

    private Rigidbody rb;
    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        gameManager = FindFirstObjectByType<GameManager>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            -fallSpeed,
            rb.linearVelocity.z
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Si toca al Player
        if (collision.gameObject.CompareTag("Player"))
        {
            if (gameManager != null)
            {
                gameManager.AddScore(points);
            }

            Destroy(gameObject);
        }

        // Si llega al suelo sin ser recogido
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}