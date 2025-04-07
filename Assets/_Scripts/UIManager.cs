using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening.Core.Easing;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private TextMeshProUGUI _timerText;
    [SerializeField]
    private TextMeshProUGUI _collectiblesText;

    [SerializeField]
    private GameObject _endScreen;

    [SerializeField]
    private TextMeshProUGUI _endScreenTimer;
    [SerializeField]
    private TextMeshProUGUI _endScreenCollectibles;
    [SerializeField]
    private TextMeshProUGUI _endScreenTimesDucked;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;    
        }
    }

    private string GetFormattedTime()
    {
        return "";
    }

    private static List<int> GetTimerValues(float rawTime)
    {
        //double rawDecimal = Math.Round(rawTime, 2);
        
        List<int> timerValues = new List<int>();        

        int minutesValue = Mathf.FloorToInt(rawTime / 60f);
        timerValues.Add(minutesValue);

        int secondsValue = Mathf.FloorToInt(rawTime - (minutesValue * 60));
        timerValues.Add(secondsValue);

        int millisecondsValue = (int)((rawTime - (minutesValue * 60) - secondsValue) * 100);

        timerValues.Add(millisecondsValue);

        return timerValues;
    }

    public static string GetTimerString(float rawTime)
    {
        List<int> timerValues = UIManager.GetTimerValues(rawTime);

        string minutesValue = (timerValues[0] > 9) ? timerValues[0].ToString() : "0" + timerValues[0].ToString();
        string secondsValue = (timerValues[1] > 9) ? timerValues[1].ToString() : "0" + timerValues[1].ToString();
        string millisecondsValue = (timerValues[2] > 9) ? timerValues[2].ToString() : "0" + timerValues[2].ToString();        

        return minutesValue + ":" + secondsValue + ":" + millisecondsValue;
    }

    // Update is called once per frame
    void Update()
    {
        this._timerText.text = UIManager.GetTimerString(ScoreManager.timeElapsed);
        this._collectiblesText.text = ScoreManager.collectiblesGrabbed.ToString();
    }    

    public void DisplayEndScreen()
    {
        this._endScreen.SetActive(true);

        float revisedTime = (int)(ScoreManager.timeElapsed * 100) / 100.0f;
        string timerString = UIManager.GetTimerString(revisedTime);

        this._timerText.text = timerString;

        this._endScreenTimer.text = timerString;
        this._endScreenCollectibles.text = ScoreManager.collectiblesGrabbed.ToString();
        this._endScreenTimesDucked.text = ScoreManager.totalTimesDucked.ToString();
    }

    public void HideEndScreen()
    {
        this._endScreen.SetActive(false);
    }

    public void RetryLevel()
    {
        SceneLoader.instance.LoadScene("MainScene");
    }

    public void ReturnToMenu()
    {
        SceneLoader.instance.LoadScene("MainMenu");
    }
}
