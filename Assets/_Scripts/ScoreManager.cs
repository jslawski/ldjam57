using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class ScoreManager
{

    public static float timeElapsed = 0.0f;
    public static int collectiblesGrabbed = 0;
    public static int totalTimesDucked = 0;

    public static void ResetScores()
    {
        ScoreManager.timeElapsed = 0.0f;
        ScoreManager.collectiblesGrabbed = 0;
        ScoreManager.totalTimesDucked = 0;
    }

    public static void IncrementTimesDucked()
    {
        ScoreManager.totalTimesDucked++;
    }

    public static void IncrementTime()
    {
        ScoreManager.timeElapsed += Time.fixedDeltaTime;
    }

    public static void IncrementCollectiblesGrabbed()
    {
        ScoreManager.collectiblesGrabbed++;
    }

    public static void SubmitScore()
    {
        string username = PlayerPrefs.GetString("username", "");

        LeaderboardManager.instance.QueueLeaderboardUpdate(username, (int)(ScoreManager.timeElapsed * 100), "leaderboard");
    }
}
