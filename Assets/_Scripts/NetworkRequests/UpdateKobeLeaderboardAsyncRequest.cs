using CabbageNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateKobeLeaderboardAsyncRequest : AsyncRequest
{
    public UpdateKobeLeaderboardAsyncRequest(string username, string timeElapsed, string collectiblesGrabbed, string timesDucked, string tableName, NetworkRequestSuccess successCallback = null, NetworkRequestFailure failureCallback = null)
    {
        string url = ServerSecrets.ServerName + "ldjam57/leaderboard/updateCabbageLeaderboard.php";

        this.form = new WWWForm();
        this.form.AddField("username", username);
        this.form.AddField("timeElapsed", timeElapsed);
        this.form.AddField("collectiblesGrabbed", collectiblesGrabbed);
        this.form.AddField("timesDucked", timesDucked);
        this.form.AddField("tableName", tableName);

        this.SetupRequest(url, successCallback, failureCallback);
    }
}
