using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score = 0;
    private int[] gameEndTurnScores = { -50, 0, 100, 300, 800, 2000, 4000, 7000};

    public int GetScore()
    {
        return score;
    }

    public void RaiseTemp()
    {
        score += 50;
    }

    public void RaiseGene()
    {
        score += 200;
    }

    public void GameEndScore(bool hasWon, int thermoIn, int radIn, int rInHand, int turn)
    {
        score += gameEndTurnScores[turn] + (hasWon ? 40 * thermoIn + 100 * radIn : 0) + 
                 20 * rInHand + Random.Range(-5, 6);
    }
}
