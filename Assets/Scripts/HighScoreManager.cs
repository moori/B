using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;

public class HighScoreManager : MonoBehaviour
{
    public void Login()
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (!response.success)
            {
                Debug.Log("error starting LootLocker session");

                return;
            }

            Debug.Log("successfully started LootLocker session");
        });
    }

    public void SendHighscore(string memberID, int score, string leaderboardKey)
    {
        LootLockerSDKManager.SubmitScore(memberID, score, leaderboardKey, "default", (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful");
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
        });
    }

}

[System.Serializable]
public class HighScoreMetadata
{
    public string username;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public HighScoreMetadata FromJson(string json)
    {
        return JsonUtility.FromJson<HighScoreMetadata>(json);
    }
}