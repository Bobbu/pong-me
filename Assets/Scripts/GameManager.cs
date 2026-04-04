using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI leftScoreText;
    public TextMeshProUGUI rightScoreText;
    public BallController ball;

    private int leftScore;
    private int rightScore;

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
        // Ball entered left side = right player scores, and vice versa
        if (leftSideGoal)
            rightScore++;
        else
            leftScore++;

        UpdateScoreUI();
        ball.ResetBall();
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
            UpdateScoreUI();
            ball.ResetBall();
        }
    }
}
