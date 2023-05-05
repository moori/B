using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RenamePanel : MonoBehaviour
{
    public string newName;

    public System.Action OnRenameComplete;
    public TMP_InputField field;

    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        field.text = HighScoreManager.GetUsername();
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnValueChange(string newString)
    {
        newName = newString;
    }

    public void OnClickConfirm()
    {
        HighScoreManager.SetUsername(newName);
        HighScoreManager.SetPlayerName(newName,()=> {
            OnRenameComplete?.Invoke();
        });
        Hide();
    }
}
