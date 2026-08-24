using UnityEngine;
using System.Collections;

public class FallingObject : MonoBehaviour
{
    [Header("Caída")]
    public float fallSpeed = 3f;

    [Header("Rotación")]
    public float minRotationSpeed = 30f;
    public float maxRotationSpeed = 70f;

    [Header("Puntuación")]
    public int points = 10;

    [Header("Tiempo en el suelo")]
    public float positiveGroundDelay = 1f;

    private Rigidbody rb;
    private GameManager gameManager;

    private Vector3 rotationDirection;
    private float rotationSpeed;

    private bool collected = false;

    // Evita ejecutar varias veces la lógica
    // cuando el objeto ya tocó el suelo
    private bool touchedGround = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        gameManager =
            FindFirstObjectByType<GameManager>();

        rotationDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        rotationSpeed = Random.Range(
            minRotationSpeed,
            maxRotationSpeed
        );
    }


    void FixedUpdate()
    {
        // Si ya tocó el suelo,
        // dejamos de controlar su caída y rotación
        if (touchedGround)
            return;

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            -fallSpeed,
            rb.linearVelocity.z
        );

        transform.Rotate(
            rotationDirection *
            rotationSpeed *
            Time.fixedDeltaTime,
            Space.Self
        );
    }


    private void OnCollisionEnter(Collision collision)
    {
        // ==========================================
        // COLISIÓN CON EL PLAYER
        // ==========================================

        PlayerMovement player =
            collision.gameObject.GetComponentInParent<PlayerMovement>();

        if (player != null)
        {
            CollectObject();
            return;
        }


        // ==========================================
        // COLISIÓN CON EL SUELO
        // ==========================================

        if (collision.gameObject.CompareTag("Ground"))
        {
            if (touchedGround || collected)
                return;

            touchedGround = true;


            // ======================================
            // OBJETO POSITIVO
            // ======================================

            if (points > 0)
            {
                // Detener el objeto
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                // Evitar que vuelva a moverse
                rb.isKinematic = true;

                // Darle al jugador una última
                // oportunidad de recogerlo
                StartCoroutine(
                    PositiveObjectGroundDelay()
                );
            }


            // ======================================
            // OBJETO NEGATIVO
            // ======================================

            else
            {
                // Los negativos desaparecen
                // inmediatamente al tocar el suelo
                Destroy(gameObject);
            }
        }
    }


    private IEnumerator PositiveObjectGroundDelay()
    {
        // Tiempo que permanece disponible
        // sobre el suelo
        yield return new WaitForSeconds(
            positiveGroundDelay
        );


        // Puede haber sido recogido durante
        // este segundo
        if (collected)
            yield break;


        // Si no fue recogido, cuenta como perdido
        if (gameManager != null)
        {
            gameManager.PositiveObjectMissed();
        }

        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Área especial de recolección
        if (other.gameObject.name == "CollectArea")
        {
            CollectObject();
        }
    }

    private void CollectObject()
    {
        if (collected)
            return;

        collected = true;

        if (gameManager != null)
        {
            // Si es un objeto negativo y el escudo está activo,
            // desaparece sin quitar puntos.
            if (points < 0 && gameManager.IsShieldActive())
            {
                Debug.Log("OBJETO NEGATIVO BLOQUEADO POR EL ESCUDO");

                Destroy(gameObject);
                return;
            }

            // Funcionamiento normal
            gameManager.AddScore(points);
            gameManager.AddPowerUpPoints(points);
        }

        Destroy(gameObject);
    }
}