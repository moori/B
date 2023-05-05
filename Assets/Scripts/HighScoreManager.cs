using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using System.Linq;

public class HighScoreManager : MonoBehaviour
{

    public static string leaderboardKey = "hs_board";

    private void Start()
    {
        Login();
    }

    public static void SetUsername(string name)
    {
        PlayerPrefs.SetString("username", name);
    }
    public static string GetUsername()
    {
        return PlayerPrefs.GetString("username", $"user_{System.Guid.NewGuid().ToString().Substring(0,7)}");
    }

    public static void SetBestScore(int score)
    {
        PlayerPrefs.SetInt("bestScore", score);
    }
    public static int GetBestScore()
    {
        return PlayerPrefs.GetInt("bestScore", 0);
    }
    public static string GetUserID()
    {
        if (!PlayerPrefs.HasKey("userid"))
        {
            PlayerPrefs.SetString("userid", System.Guid.NewGuid().ToString());
        }

        return PlayerPrefs.GetString("userid");
    }

    public static void Login()
    {
        LootLockerSDKManager.StartGuestSession(GetUserID(), (response) =>
        {
            if (!response.success)
            {
                Debug.Log($"error starting LootLocker session: {response.Error} - {response.statusCode} - {response.hasError}  ");
                return;
            }
            PlayerPrefs.SetInt("PlayerID", response.player_id);
            PlayerPrefs.SetString("PlayerUID", response.public_uid);
            Debug.Log($"successfully started LootLocker session: {response.player_id} - {response.player_identifier}");
        });
    }

    public static int GetPlayerID()
    {
        return PlayerPrefs.GetInt("PlayerID", -1);
    }
    public static string GetPlayerUID()
    {
        return PlayerPrefs.GetString("PlayerUID", "");
    }

    public static void SetPlayerName(string name, System.Action callback=null)
    {
        SetUsername(name);
        LootLockerSDKManager.SetPlayerName(name, (response) =>
        {
            if (response.success)
            {
                Debug.Log($"Successfully set player name: {name}");
                callback?.Invoke();
            }
            else
            {
                Debug.Log("Error setting player name");
            }
        });
    }

    public static void SendHighscore(int score)
    {
        LootLockerSDKManager.SubmitScore("", score, leaderboardKey, GetUsername(), (response) =>
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

    public static void GetTopScores(System.Action<List<LootLockerLeaderboardMember>> results)
    {
        LootLockerSDKManager.GetScoreList(leaderboardKey, 10, 0, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful");
                results(response.items.ToList());
                return;
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
        });
        results(null);
    }

    public static void GetNearScores(System.Action<List<LootLockerLeaderboardMember>> results)
    {

        LootLockerSDKManager.GetMemberRank(leaderboardKey, GetPlayerID(), (response) =>
        {
            if (response.statusCode == 200)
            {
                int rank = response.rank;
                int count = 5;
                int after = rank < 2 ? 0 : rank - 3;

                LootLockerSDKManager.GetScoreList(leaderboardKey, count, after, (response) =>
                {
                    if (response.statusCode == 200)
                    {
                        Debug.Log("Successful");
                        results(response.items.ToList());
                        return;
                    }
                    else
                    {
                        Debug.Log("failed: " + response.Error);
                    }

                    results(null);
                });
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
        });
    }

    [ContextMenu("ResetPlayerPrefs")]
    public static void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Login();
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