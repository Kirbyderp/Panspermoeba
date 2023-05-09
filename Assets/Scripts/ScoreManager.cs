using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score = 0;
    private int[] gameEndTurnScores = { -100, 0, 500, 1000, 2000, 3000, 4000, 5000};

    public int GetScore()
    {
        return score;
    }

    public void RaiseTemp()
    {
        score += 80;
    }

    public void RaiseGene()
    {
        score += 250;
    }

    public void Move()
    {
        score += 30;
    }

    public void GameEndScore(bool hasWon, int thermoIn, int radIn, int rInHand, int turn)
    {
        score += gameEndTurnScores[turn] + (hasWon ? 40 * thermoIn + 300 * radIn : 0) + 
                 40 * rInHand + Random.Range(-5, 6);
    }
}
