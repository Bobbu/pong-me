using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public float speed = 10f;
    public float boundaryY = 4f;
    public bool isPlayer = true;
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;

    // AI settings
    public Transform ball;
    public float aiReactionSpeed = 6f;

    void Update()
    {
        if (isPlayer)
        {
            HandlePlayerInput();
        }
        else
        {
            HandleAI();
        }

        ClampPosition();
    }

    void HandlePlayerInput()
    {
        float move = 0f;

        if (Input.GetKey(upKey))
            move = 1f;
        else if (Input.GetKey(downKey))
            move = -1f;

        // Also support arrow keys for right paddle
        if (upKey == KeyCode.UpArrow && Input.GetKey(KeyCode.UpArrow))
            move = 1f;
        else if (downKey == KeyCode.DownArrow && Input.GetKey(KeyCode.DownArrow))
            move = -1f;

        transform.Translate(Vector2.up * move * speed * Time.deltaTime);
    }

    void HandleAI()
    {
        if (ball == null) return;

        float targetY = ball.position.y;
        float currentY = transform.position.y;
        float move = Mathf.MoveTowards(currentY, targetY, aiReactionSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, move, 0f);
    }

    void ClampPosition()
    {
        float clampedY = Mathf.Clamp(transform.position.y, -boundaryY, boundaryY);
        transform.position = new Vector3(transform.position.x, clampedY, 0f);
    }
}
