using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI leftScoreText;
    public TextMeshProUGUI rightScoreText;
    public TextMeshProUGUI winText;
    public GameObject winOverlay;
    public GameObject startOverlay;
    public BallController ball;
    public int winningScore = 3;

    private int leftScore;
    private int rightScore;
    private bool gameOver;
    private bool paused;
    private bool gameStarted;
    private Vector2 savedBallVelocity;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateScoreUI();
    }

    /// <summary>
    /// Called by the Start overlay's button (or Space/Enter) on first launch.
    /// Hides the overlay, primes the audio system (critical for WebGL), and
    /// kicks off the very first ball launch. Subsequent rounds re-launch
    /// automatically via BallController.ResetBall().
    /// </summary>
    public void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;
        if (startOverlay != null) startOverlay.SetActive(false);
        if (SoundManager.Instance != null) SoundManager.Instance.PrimeAudio();
        if (ball != null) ball.LaunchBall();
    }

    public void ScorePoint(bool leftSideGoal)
    {
        if (gameOver || paused || !gameStarted) return;

        if (leftSideGoal)
            rightScore++;
        else
            leftScore++;

        UpdateScoreUI();

        if (leftScore >= winningScore)
        {
            EndGame("PLAYER WINS!");
        }
        else if (rightScore >= winningScore)
        {
            EndGame("AI WINS!\nPlease welcome our new Computer Overlords!");
        }
        else
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayScore();
            ball.ResetBall();
        }
    }

    void EndGame(string message)
    {
        gameOver = true;
        if (SoundManager.Instance != null) SoundManager.Instance.PlayWin();
        if (winText != null)
            winText.text = message;
        if (winOverlay != null)
            winOverlay.SetActive(true);
        ball.ResetBall();
        ball.CancelInvoke();
        ball.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }

    public void ResetGame()
    {
        leftScore = 0;
        rightScore = 0;
        gameOver = false;
        paused = false;
        Time.timeScale = 1f;
        UpdateScoreUI();
        if (winOverlay != null)
            winOverlay.SetActive(false);
        ball.ResetBall();
    }

    public void SetPaused(bool pause)
    {
        if (gameOver) return;
        paused = pause;

        var rb = ball.GetComponent<Rigidbody2D>();
        if (pause)
        {
            savedBallVelocity = rb.linearVelocity;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            // If ball had velocity, restore it; otherwise it's waiting to launch
            if (savedBallVelocity.sqrMagnitude > 0.01f)
                rb.linearVelocity = savedBallVelocity;
            else
                ball.LaunchBall();
        }
    }

    public bool IsPaused() => paused;

    void UpdateScoreUI()
    {
        if (leftScoreText != null)
            leftScoreText.text = leftScore.ToString();
        if (rightScoreText != null)
            rightScoreText.text = rightScore.ToString();
    }

    void Update()
    {
        // Cmd+Q / Ctrl+Q to quit (Mac/Windows) — always available
        if ((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand) ||
             Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            && Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }

        // While the Start overlay is up, Space / Enter / Return also begin the game.
        // Speed and sound shortcuts (1/2/3, M) below remain available so users can
        // tweak preferences before starting. Reset (R) is gated until after start.
        if (!gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.Return) ||
                Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                StartGame();
            }
        }
        else
        {
            // Keyboard reset (still works on Mac) — only after the game has started
            if (Input.GetKeyDown(KeyCode.R))
                ResetGame();
        }

        // Keyboard speed shortcuts (also available via Settings panel)
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetSpeed(5f, 0.3f, 12f);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetSpeed(8f, 0.5f, 20f);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetSpeed(12f, 0.8f, 28f);

        // Sound toggle
        if (Input.GetKeyDown(KeyCode.M)) ToggleSound();
    }

    public void SetSpeed(float initial, float increase, float max)
    {
        ball.initialSpeed = initial;
        ball.speedIncrease = increase;
        ball.maxSpeed = max;
        ball.ApplySpeedChange();
    }

    public void ToggleSound()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMuted(!SoundManager.Instance.IsMuted());
    }
}
