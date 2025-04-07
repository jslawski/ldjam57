using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardEntryObject : MonoBehaviour
{
    public Image backgroundImage;
    public TextMeshProUGUI placementText;
    public TextMeshProUGUI username;
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        if (this.scoreText.text == "0")
        {
            this.gameObject.SetActive(false);
        }
    }

    public void UpdateEntry(string username, float value, float placement)
    {
        this.gameObject.SetActive(true);

        if (username == "" || value == 0)
        {
            return;
        }       

        this.username.text = username;
        if (value < 99999999)
        {
            this.scoreText.text = UIManager.GetTimerString(value / 100.0f);
        }
        else
        {
            this.scoreText.text = "--";
        }
    }
}