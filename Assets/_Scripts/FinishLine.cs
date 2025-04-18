using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.instance.StopGame();

            ScoreManager.SubmitScore();

            UIManager.instance.DisplayEndScreen();
        }
    }
}
