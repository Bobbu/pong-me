using UnityEngine;

public class GoalZone : MonoBehaviour
{
    public bool isLeftGoal = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            GameManager.Instance.ScorePoint(isLeftGoal);
        }
    }
}
