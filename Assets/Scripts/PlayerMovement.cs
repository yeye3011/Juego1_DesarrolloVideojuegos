using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;

    [Header("Límites laterales")]
    public float minX = -4f;
    public float maxX = 4f;

    [Header("Salto")]
    public float jumpForce = 3f;
    public float fallMultiplier = 2f;

    private Rigidbody rb;
    private Animator animator;

    private float moveDirection = 0f;
    private bool isGrounded = true;

    private string currentAnimation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>(true);

        PlayAnimation("idle");
    }

    void Update()
    {
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        // Movimiento horizontal
        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveDirection * moveSpeed;
        rb.linearVelocity = velocity;

        // Caída
        if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(
                Vector3.up * Physics.gravity.y * fallMultiplier,
                ForceMode.Acceleration
            );
        }

        // Límites laterales
        if (rb.position.x < minX)
        {
            rb.MovePosition(new Vector3(
                minX,
                rb.position.y,
                rb.position.z
            ));
        }
        else if (rb.position.x > maxX)
        {
            rb.MovePosition(new Vector3(
                maxX,
                rb.position.y,
                rb.position.z
            ));
        }
    }

    private void UpdateAnimation()
    {
        if (!isGrounded)
        {
            PlayAnimation("jump");
        }
        else if (moveDirection != 0)
        {
            PlayAnimation("walk");
        }
        else
        {
            PlayAnimation("idle");
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (currentAnimation == animationName)
            return;

        currentAnimation = animationName;
        animator.Play(animationName);
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
            rb.AddForce(
                Vector3.up * jumpForce,
                ForceMode.Impulse
            );

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