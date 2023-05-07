using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject loading;

    [Header("highscores")]
    public HighScoreScreen highscoreScreen;
    public GameObject creditsScreen;

    public void OnClickStart()
    {
        loading.SetActive(true);
        this.DelayAction(1f, () => {
            SceneManager.LoadScene("Game");
        });
    }
    public void OnClickHighscore()
    {
        highscoreScreen.Show();
    }

    public void OnClickCredits()
    {
        creditsScreen.gameObject.SetActive(true);
    }

    public void CloseHighlightscore()
    {
        highscoreScreen.Hide();
    }
    public void CloseCredits()
    {
        creditsScreen.gameObject.SetActive(false);
    }
    public void OnClickQuit()
    {
        Application.Quit();
    }
    public void OnClickReset()
    {
        HighScoreManager.ResetPlayerPrefs();
    }

    private void Update()
    {
        if (highscoreScreen.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            highscoreScreen.Hide();
        }
    }
}
