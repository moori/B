using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

public class TutorialController : MonoBehaviour
{

    Player player;

    public TextMeshPro worldText;
    public Enemy dummy;
    private bool dummykilled;
    private bool playerRecharged;

    public static bool IsTutorial = true;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        player.OnRecharge.AddListener(OnPlayerRecharge);

        IsTutorial = PlayerPrefs.GetInt("tutorialComplete", 0) == 0;
    }
    private void OnDestroy()
    {
        player.OnRecharge.RemoveListener(OnPlayerRecharge);
    }

    public void DummyKilled()
    {
        dummykilled = true;
    }
    private void OnPlayerRecharge(float v)
    {
        playerRecharged = true;
    }

    public IEnumerator TutorialRoutine()
    {
        if (!IsTutorial)
        {
            Destroy(gameObject);
            yield break;
        }

        AudioManager.PlayAmbience();

        //movement
        worldText.text = $"Press the <color=\"green\">Arrow keys</color> or <color=\"green\">WASD</color> to move\nYou aim away from where you're moving";
        bool mv_left = false;
        var mv_right = false;
        var mv_up = false;
        var mv_down = false;

        yield return new WaitUntil(() =>
        {
            if (PlayerInputs.Horizontal < 0) mv_left = true;
            if (PlayerInputs.Horizontal > 0) mv_right = true;
            if (PlayerInputs.Vertical < 0) mv_down = true;
            if (PlayerInputs.Vertical > 0) mv_up = true;
            return (mv_left && mv_right && mv_down && mv_up);
        });
        BackColorController.instance.Flash(Color.green, () => BackColorController.instance.SetIdle());
        yield return new WaitForSeconds(2f);
        worldText.text = $"";
        yield return new WaitForSeconds(1f);

        // Slow
        worldText.text = $"Hold <color=\"green\">Z</color> or <color=\"green\">L</color> to move slowly\nThis might help you aim and dogde bullets";
        var slowTimer = 2f;
        yield return new WaitUntil(() =>
        {
            if (PlayerInputs.Slow && (PlayerInputs.Horizontal != 0 || PlayerInputs.Vertical != 0))
            {
                slowTimer -= Time.deltaTime;
            }

            return slowTimer <= 0;
        });
        BackColorController.instance.Flash(Color.green, () => BackColorController.instance.SetIdle());
        yield return new WaitForSeconds(2f);
        worldText.text = $"";
        yield return new WaitForSeconds(1f);

        //Fire
        worldText.text = $"Hold <color=\"green\">C</color> or <color=\"green\">J</color> to fire";
        var fireTimer = 2f;
        yield return new WaitUntil(() =>
        {
            if (PlayerInputs.Fire) fireTimer -= Time.deltaTime;
            return fireTimer <= 0;
        });
        BackColorController.instance.Flash(Color.green, () => BackColorController.instance.SetIdle());
        yield return new WaitForSeconds(1f);
        worldText.text = $"";
        yield return new WaitForSeconds(1f);

        worldText.text = $"Every shot you fire consumes your HP\nMAKE THEM COUNT";
        yield return new WaitForSeconds(6f);

        // dummy
        worldText.text = $"Detroy the target";
        dummy.Respawn(Vector3.zero);
        yield return new WaitUntil(() => dummykilled);
        BackColorController.instance.Flash(Color.green, () => BackColorController.instance.SetIdle());
        yield return new WaitForSeconds(1f);
        worldText.text = $"";
        yield return new WaitForSeconds(1f);
        
        worldText.text = $"<color=\"purple\">Energy Shards</color> will recharge your ship\nPress <color=\"green\">X</color> or <color=\"green\">K</color> to activate your shield and collect them";
        var shardLifetime = PoolManager.instance.GetBatteryShard().lifetime;
        var shardTimer = shardLifetime;
        yield return new WaitUntil(() => {
            shardTimer -= Time.deltaTime;
            if (shardTimer <= 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    var s = PoolManager.instance.GetBatteryShard();
                    s.Spawn(Vector3.zero);
                }
                shardTimer = shardLifetime;
            }
            return playerRecharged;
            });
        BackColorController.instance.Flash(Color.green, () => BackColorController.instance.SetIdle());
        yield return new WaitForSeconds(3f);
        
        worldText.text = $"Advance and Destroy";
        yield return new WaitForSeconds(4f);
        AudioManager.FadeOut();
        worldText.text = $"";
        PlayerPrefs.SetInt("tutorialComplete", 1);
        IsTutorial = false;
        player.Recharge(1f);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void Update()
    {
        if(player.ammo < 10)
        {
            player.ammo = 10;
        }
    }
}
