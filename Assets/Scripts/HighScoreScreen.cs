using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class HighScoreScreen : MonoBehaviour
{
    public List<TextMeshProUGUI> topLines;
    public List<TextMeshProUGUI> personalLines;
    public Color playerColor;
    public Color defautlColor;
    
    private void Awake()
    {
        gameObject.SetActive(false);
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        foreach (var item in topLines)
        {
            item.text = "Loading";
            item.color = defautlColor;
        }
        foreach (var item in personalLines)
        {
            item.text = "Loading";
            item.color = defautlColor;
        }


        HighScoreManager.GetTopScores(results => {
            if (results == null) return;
            for (int i = 0; i < 10; i++)
            {
                var line = topLines[i];

                if (i < results.Count)
                {
                    var data = results.ElementAtOrDefault(i);
                    line.text = $"#{string.Format("{0:00}", data.rank)} - {data.player.name,12} .......... {string.Format("{0:0000000000}", data.score)}";
                    line.color = data.player.id == HighScoreManager.GetPlayerID() ? playerColor : defautlColor;
                }
                else
                {
                    line.text = $"#{string.Format("{0:00}", 0)} - {"Violet",12} .......... {string.Format("{0:0000000000}", 0)}";
                    line.color = defautlColor;
                }
            }
        });

        HighScoreManager.GetNearScores(results=> {
            for (int i = 0; i < 5; i++)
            {
                var line = personalLines[i];
                if (i < results.Count)
                {
                    var data = results.ElementAtOrDefault(i);
                    line.text = $"#{string.Format("{0:0000}", data.rank)} - {data.player.name,12} .......... {string.Format("{0:0000000000}", data.score)}";
                    line.color = data.player.id == HighScoreManager.GetPlayerID() ? playerColor : defautlColor;
                }
                else
                {
                    line.text = $"#{string.Format("{0:0000}", 0)} - {"Violet",12} .......... {string.Format("{0:0000000000}", 0)}";
                    line.color = defautlColor;
                }
            }
        });
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClickClose()
    {
        Hide();
    }
}
