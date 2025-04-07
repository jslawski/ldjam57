using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool gameInProgress = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        this.gameInProgress = true;
        ScoreManager.ResetScores();
    }

    public void StartGame()
    {
        this.gameInProgress = true;
    }

    public void StopGame()
    {
        this.gameInProgress = false;
    }

    private void FixedUpdate()
    {
        if (this.gameInProgress == true)
        {
            ScoreManager.IncrementTime();
        }
    }
}
