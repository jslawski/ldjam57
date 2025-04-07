using CabbageNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetKobeLeaderboardAsyncRequest : AsyncRequest
{
    public GetKobeLeaderboardAsyncRequest(string playerName, string tableName, NetworkRequestSuccess successCallback = null, NetworkRequestFailure failureCallback = null)
    {
        string url = ServerSecrets.ServerName + "ldjam57/leaderboard/getCabbageLeaderboardEntry.php";

        this.form = new WWWForm();
        this.form.AddField("username", playerName);
        this.form.AddField("tableName", tableName);

        this.SetupRequest(url, successCallback, failureCallback);
    }
}
