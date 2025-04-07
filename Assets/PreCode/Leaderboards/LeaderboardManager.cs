using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class LeaderboardUpdate
{
    public string username;
    public float value;
    public string tableName;

    public LeaderboardUpdate(string newName, float newValue, string tableName)
    {
        this.username = newName;
        this.value = newValue;
        this.tableName = tableName;
    }
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager instance;

    public LeaderboardEntryObject[] leaderboardEntryObjects;

    private Queue<LeaderboardUpdate> queuedUpdates;

    private bool readyToProcessUpdate = true;

    public LeaderboardData currentLeaderboardData;

    [SerializeField]
    private GameObject leaderboardObject;

    [SerializeField]
    private Sprite[] leaderboardEntryBackgrounds;

    [SerializeField]
    private TextMeshProUGUI _pbPlacement;

    [SerializeField]
    private TextMeshProUGUI _pbUsername;

    [SerializeField]
    private TextMeshProUGUI _pbScore;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.leaderboardEntryObjects = GetComponentsInChildren<LeaderboardEntryObject>(true);

        this.queuedUpdates = new Queue<LeaderboardUpdate>();

        this.InitializeLeaderboardEntryObjects();
    }

    private void Start()
    {
        //this.RefreshLeaderboard(LevelList.GetCurrentLevel().sceneName);
    }

    private void InitializeLeaderboardEntryObjects()
    {
        for (int i = 0; i < this.leaderboardEntryObjects.Length; i++)
        {
            this.leaderboardEntryObjects[i].placementText.text = (i + 1).ToString();
            //this.SetLeaderboardEntryObjectBackground(this.leaderboardEntryObjects[i], i);
        }
    }

    private void SetLeaderboardEntryObjectBackground(LeaderboardEntryObject entryObject, int index)
    {
        int moddedIndex = index % 4;

        switch (moddedIndex)
        {
            case 0:
                entryObject.backgroundImage.sprite = this.leaderboardEntryBackgrounds[0];
                break;
            case 1:
                entryObject.backgroundImage.sprite = this.leaderboardEntryBackgrounds[1];
                break;
            case 2:
                entryObject.backgroundImage.sprite = this.leaderboardEntryBackgrounds[2];
                break;
            case 3:
                entryObject.backgroundImage.sprite = this.leaderboardEntryBackgrounds[3];
                break;
            default:
                entryObject.backgroundImage.sprite = this.leaderboardEntryBackgrounds[0];
                break;
        }
    }

    public void DisableLeaderboard()
    {
        this.leaderboardObject.SetActive(false);
    }

    public void RefreshLeaderboard(string tableName)
    {
        this.RequestLeaderboard(tableName);
        this.RefreshLatestHighScoreValues();
    }

    #region Get Leaderboard
    private void RequestLeaderboard(string tableName)
    {
        GetCabbageLeaderboardAsyncRequest request = new GetCabbageLeaderboardAsyncRequest(tableName, this.RequestLeaderboardSuccess, this.RequestLeaderboardFailure);
        request.Send();
    }

    private void RequestLeaderboardSuccess(string data)
    {
        //Empty leaderboard, return
        if (data == "[]")
        {
            return;
        }

        this.currentLeaderboardData = JsonUtility.FromJson<LeaderboardData>(data);

        if (this.leaderboardObject.activeSelf == true)
        {
            this.UpdateLeaderboardVisuals();
        }
    }

    private void RequestLeaderboardFailure()
    {
        Debug.LogError("Error: Unable to get leaderboard");
    }
    #endregion
    
    #region UpdateLeaderboard
    private void UpdateLeaderboard(LeaderboardUpdate updateValues)
    {
        UpdateCabbageLeaderboardAsyncRequest request = new UpdateCabbageLeaderboardAsyncRequest(updateValues.username, updateValues.value.ToString(), updateValues.tableName, this.UpdateLeaderboardSuccess, this.UpdateLeaderboardFailure);
        request.Send();
    }

    private void UpdateLeaderboardSuccess(string data)
    {
        this.readyToProcessUpdate = true;
        this.RefreshLeaderboard("leaderboard");
    }

    private void UpdateLeaderboardFailure()
    {
        Debug.LogError("Error: Unable to update leaderboard entry");
    }
    #endregion

    public LeaderboardEntryData GetTopPlayer()
    {
        if (this.currentLeaderboardData.entries.Count > 0)
        {
            return this.currentLeaderboardData.entries[0];
        }
        else
        {
            return null;
        }
    }

    public void QueueLeaderboardUpdate(string username, float value, string tableName)
    {
        this.queuedUpdates.Enqueue(new LeaderboardUpdate(username, value, tableName));
    }

    private void FixedUpdate()
    {
        if (this.queuedUpdates.Count > 0 && this.readyToProcessUpdate == true)
        {
            this.ProcessUpdate(this.queuedUpdates.Dequeue());
        }
    }

    private void ProcessUpdate(LeaderboardUpdate updateValues)
    {
        this.readyToProcessUpdate = false;

        this.UpdateLeaderboard(updateValues);
    }

    private void UpdateLeaderboardVisuals()
    {
        this.ClearLeaderboardVisuals();

        for (int i = 0; i < this.currentLeaderboardData.entries.Count && i < this.leaderboardEntryObjects.Length; i++)
        {
            this.leaderboardEntryObjects[i].UpdateEntry(this.currentLeaderboardData.entries[i].username, this.currentLeaderboardData.entries[i].value, this.currentLeaderboardData.entries[i].placement);
        }
    }

    private void ClearLeaderboardVisuals()
    {
        for (int i = 0; i < this.leaderboardEntryObjects.Length; i++)
        {
            this.leaderboardEntryObjects[i].UpdateEntry(string.Empty, 0, 0);
        }
    }

    public bool IsTopPlayer(string username)
    {
        return (this.GetTopPlayer().username == username);
    }

    public void RefreshLatestHighScoreValues()
    {
        string playerName = PlayerPrefs.GetString("username", "");
        GetCabbageLeaderboardEntryAsyncRequest request = new GetCabbageLeaderboardEntryAsyncRequest(playerName, "leaderboard", this.GetDataSuccess, this.GetDataFailure);
        request.Send();
    }

    private void GetDataSuccess(string data)
    {
        //Empty leaderboard, return
        if (data == "[]")
        {
            return;
        }

        LeaderboardEntryData leaderboardEntry = JsonUtility.FromJson<LeaderboardEntryData>(data);

        if (leaderboardEntry.value >= 99999998)
        {
            this._pbPlacement.text = "--";
            this._pbScore.text = "--";
        }
        else
        {
            this._pbPlacement.text = leaderboardEntry.placement.ToString();
            this._pbScore.text = UIManager.GetTimerString(leaderboardEntry.value / 100.0f);
        }

        this._pbUsername.text = leaderboardEntry.username;
    }

    private void GetDataFailure()
    {
        this._pbPlacement.text = "--";
        this._pbScore.text = "--";

        this._pbUsername.text = PlayerPrefs.GetString("username", "--");
    }
}
