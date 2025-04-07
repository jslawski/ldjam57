using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening.Core.Easing;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timerText;
    [SerializeField]
    private TextMeshProUGUI _collectiblesText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private string GetFormattedTime()
    {
        return "";
    }

    public List<int> GetTimerValues()
    {
        List<int> timerValues = new List<int>();

        int minutesValue = Mathf.FloorToInt(ScoreManager.timeElapsed / 60f);
        timerValues.Add(minutesValue);

        int secondsValue = Mathf.FloorToInt(ScoreManager.timeElapsed - (minutesValue * 60));
        timerValues.Add(secondsValue);

        int millisecondsValue = (int)((ScoreManager.timeElapsed - (minutesValue * 60) - secondsValue) * 100);
        timerValues.Add(millisecondsValue);

        return timerValues;
    }

    public string GetTimerString()
    {
        List<int> timerValues = this.GetTimerValues();

        string minutesValue = (timerValues[0] > 9) ? timerValues[0].ToString() : "0" + timerValues[0].ToString();
        string secondsValue = (timerValues[1] > 9) ? timerValues[1].ToString() : "0" + timerValues[1].ToString();
        string millisecondsValue = (timerValues[2] > 9) ? timerValues[2].ToString() : "0" + timerValues[2].ToString();

        return minutesValue + ":" + secondsValue + ":" + millisecondsValue;
    }

    // Update is called once per frame
    void Update()
    {
        this._timerText.text = this.GetTimerString();
        this._collectiblesText.text = ScoreManager.collectiblesGrabbed.ToString();
    }

    private void FixedUpdate()
    {
        ScoreManager.IncrementTime();
    }
}
