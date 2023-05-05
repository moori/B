using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{

    public GameObject loading;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI maxHPText;
    public TextMeshProUGUI shieldSlotText;
    public TextMeshProUGUI missileText;

    private Player player;

    private void Awake()
    {
        gameObject.SetActive(false);
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GameController.IsGamePaused)
        {
            OnClickResume();
        }
    }

    public void Pause()
    {
        GameController.IsGamePaused = true;
        Time.timeScale = 0f;
        gameObject.SetActive(true);

        if (!player) {
            player = FindObjectOfType<Player>();
        }
        scoreText.text = $"score:\n{string.Format("{0:000000000}", GameController.instance.score)}";
        fireRateText.text = $"fire rate: {player.fireRate_UpgradeUnlocks}/{Player.max_fireRate_UpgradeUnlocks}";
        maxHPText.text = $"max hp: {player.maxHP_UpgradesUnlocked}/{Player.max_maxHP_UpgradesUnlocked}";
        shieldSlotText.text = $"shield slots: {player.bubbleSlot_UpgradeUnlocks}/{Player.max_bubbleSlot_UpgradeUnlocks}";
        missileText.text = $"missiles: {player.missile_UpgradeUnlocks}/{Player.max_missile_UpgradeUnlocks}";
    }

    public void OnClickResume()
    {
        GameController.IsGamePaused = false;
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
    public void OnClickRestart()
    {
        GameController.IsGamePaused = false;
        Time.timeScale = 1f;
        loading.SetActive(true);
        this.DelayAction(1f, () =>
        {
            SceneManager.LoadScene("Game");
        });
    }
    public void OnClickMainMenu()
    {
        GameController.IsGamePaused = false;
        Time.timeScale = 1f;
        loading.SetActive(true);
        this.DelayAction(1f, () =>
        {
            SceneManager.LoadScene("Menu");
        });
    }
}
