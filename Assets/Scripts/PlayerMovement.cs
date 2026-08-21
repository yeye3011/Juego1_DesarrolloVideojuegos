using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;

    [Header("Límites laterales")]
    public float minX = -4f;
    public float maxX = 4f;

    [Header("Salto")]
    public float jumpForce = 6f;
    public float fallMultiplier = 2.5f;

    private Rigidbody rb;
    private float moveDirection = 0f;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Movimiento horizontal
        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveDirection * moveSpeed;
        rb.linearVelocity = velocity;

        // Caída más rápida
        if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(
                Vector3.up * Physics.gravity.y * (fallMultiplier - 1f),
                ForceMode.Acceleration
            );
        }

        // Límites laterales
        Vector3 position = rb.position;
        position.x = Mathf.Clamp(position.x, minX, maxX);
        rb.position = position;
    }

    public void MoveLeft()
    {
        moveDirection = -1f;
    }

    public void MoveRight()
    {
        moveDirection = 1f;
    }

    public void StopMoving()
    {
        moveDirection = 0f;
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}