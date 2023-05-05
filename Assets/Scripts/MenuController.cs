using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject loading;

    [Header("highscores")]
    public HighScoreScreen highscoreScreen;

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

    public void CloseHighlightscore()
    {
        highscoreScreen.Hide();
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
