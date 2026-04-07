using UnityEngine;

public class BallController : MonoBehaviour
{
    public float initialSpeed = 8f;
    public float speedIncrease = 0.5f;
    public float maxSpeed = 20f;

    private Rigidbody2D rb;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Ball stays motionless at center until GameManager.StartGame() launches it.
        // The Start overlay (PRESS START) gates the first launch on every platform.
    }

    public void LaunchBall()
    {
        currentSpeed = initialSpeed;
        float xDir = Random.Range(0, 2) == 0 ? -1f : 1f;
        float yDir = Random.Range(-0.5f, 0.5f);
        Vector2 direction = new Vector2(xDir, yDir).normalized;
        rb.linearVelocity = direction * currentSpeed;
    }

    public void ResetBall()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = Vector3.zero;
        Invoke(nameof(LaunchBall), 1f);
    }

    public void ApplySpeedChange()
    {
        if (rb == null) return;
        Vector2 vel = rb.linearVelocity;
        if (vel.sqrMagnitude > 0.01f)
        {
            currentSpeed = initialSpeed;
            rb.linearVelocity = vel.normalized * currentSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayPaddleHit();

            currentSpeed = Mathf.Min(currentSpeed + speedIncrease, maxSpeed);

            float paddleCenterY = collision.transform.position.y;
            float hitPoint = (transform.position.y - paddleCenterY) / collision.collider.bounds.size.y;

            float xDir = transform.position.x < 0 ? 1f : -1f;
            float yDir = hitPoint * 2f;
            Vector2 direction = new Vector2(xDir, yDir).normalized;
            rb.linearVelocity = direction * currentSpeed;
        }
        else
        {
            // Wall bounce
            if (SoundManager.Instance != null) SoundManager.Instance.PlayWallBounce();
        }
    }
}
