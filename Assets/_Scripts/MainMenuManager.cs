using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _leaderboardParent;

    public void GoToLevel()
    {
        SceneLoader.instance.LoadScene("MainScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenLeaderboard()
    {
        this._leaderboardParent.SetActive(true);
        LeaderboardManager.instance.RefreshLeaderboard("leaderboard");
    }

    public void CloseLeaderboard()
    {
        this._leaderboardParent.SetActive(false);
    }
}
