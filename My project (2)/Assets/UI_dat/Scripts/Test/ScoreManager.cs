using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public void AddScore(int amount)
    {
        score += amount;
    }
    public void RestScore()
    {
        score = 0;
    }
}
