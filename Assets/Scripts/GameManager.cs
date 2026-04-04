using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI leftScoreText;
    public TextMeshProUGUI rightScoreText;
    public TextMeshProUGUI winText;
    public BallController ball;
    public int winningScore = 3;

    private int leftScore;
    private int rightScore;
    private bool gameOver;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void ScorePoint(bool leftSideGoal)
    {
        if (gameOver) return;

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
            EndGame("AI WINS!");
        }
        else
        {
            ball.ResetBall();
        }
    }

    void EndGame(string message)
    {
        gameOver = true;
        if (winText != null)
        {
            winText.text = message + "\nPRESS R TO PLAY AGAIN";
            winText.gameObject.SetActive(true);
        }
        ball.ResetBall();
        ball.CancelInvoke();
        ball.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }

    void UpdateScoreUI()
    {
        if (leftScoreText != null)
            leftScoreText.text = leftScore.ToString();
        if (rightScoreText != null)
            rightScoreText.text = rightScore.ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            leftScore = 0;
            rightScore = 0;
            gameOver = false;
            UpdateScoreUI();
            if (winText != null)
                winText.gameObject.SetActive(false);
            ball.ResetBall();
        }
    }
}
