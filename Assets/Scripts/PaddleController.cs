using UnityEngine;
using UnityEngine.EventSystems;

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
    public float aiErrorRange = 0.5f;    // random offset from ball position
    public float aiReactionDelay = 0.1f; // seconds before AI reacts to direction change
    private float aiTargetY;
    private float aiUpdateTimer;

    // Touch settings
    public float touchSmoothSpeed = 15f;
    private float touchTargetY;
    private bool touchActive;

    void Update()
    {
        if (isPlayer)
        {
            HandleKeyboardInput();
            HandleTouchAndMouseInput();
        }
        else
        {
            HandleAI();
        }

        ClampPosition();
    }

    void HandleKeyboardInput()
    {
        float move = 0f;

        if (Input.GetKey(upKey))
            move = 1f;
        else if (Input.GetKey(downKey))
            move = -1f;

        if (move != 0f)
            transform.Translate(Vector2.up * move * speed * Time.deltaTime);
    }

    void HandleTouchAndMouseInput()
    {
        // Works for both touch (mapped to mouse on mobile) and actual mouse
        bool pressing = Input.GetMouseButton(0);
        bool justPressed = Input.GetMouseButtonDown(0);

        if (!pressing)
        {
            touchActive = false;
            return;
        }

        // Skip if touching a UI element
        if (justPressed && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // Also check for touch over UI on mobile
        if (justPressed && Input.touchCount > 0 && EventSystem.current != null
            && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            return;

        Vector3 screenPos = Input.mousePosition;

        // Only respond to touches/clicks on the left half of the screen
        if (screenPos.x > Screen.width * 0.5f)
            return;

        touchActive = true;

        // Convert screen position to world position
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        touchTargetY = worldPos.y;

        // Smoothly move paddle toward touch position
        float currentY = transform.position.y;
        float newY = Mathf.MoveTowards(currentY, touchTargetY, touchSmoothSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, 0f);
    }

    void HandleAI()
    {
        if (ball == null) return;

        // Update target periodically based on reaction delay
        aiUpdateTimer -= Time.deltaTime;
        if (aiUpdateTimer <= 0f)
        {
            aiUpdateTimer = aiReactionDelay;
            aiTargetY = ball.position.y + Random.Range(-aiErrorRange, aiErrorRange);
        }

        float currentY = transform.position.y;
        float move = Mathf.MoveTowards(currentY, aiTargetY, aiReactionSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, move, 0f);
    }

    public void SetDifficulty(float reactionSpeed, float errorRange, float reactionDelay)
    {
        aiReactionSpeed = reactionSpeed;
        aiErrorRange = errorRange;
        aiReactionDelay = reactionDelay;
    }

    void ClampPosition()
    {
        float clampedY = Mathf.Clamp(transform.position.y, -boundaryY, boundaryY);
        transform.position = new Vector3(transform.position.x, clampedY, 0f);
    }
}
