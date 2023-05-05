using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePhaseController : MonoBehaviour
{

    public List<UpgradeShip> upgradeShips;

    private Player player;

    private bool isActive;
    private LevelController levelController;

    public List<UpgradeShip.UpgradeType> randomUpgrade = new List<UpgradeShip.UpgradeType>() {
        UpgradeShip.UpgradeType.Firerate,
        UpgradeShip.UpgradeType.MaxHP,
        UpgradeShip.UpgradeType.ShieldSlot,
        UpgradeShip.UpgradeType.Missile
    };
    private void Awake()
    {
        player = FindObjectOfType<Player>();
        levelController = GetComponent<LevelController>();
    }

    public void StartUpgradePhase()
    {
        isActive = true;
        for (int i = randomUpgrade.Count - 1; i >= 0; i--)
        {
            if (!CanUpgrade(randomUpgrade[i])) randomUpgrade.RemoveAt(i);
        }
        randomUpgrade.Shuffle();

        if (randomUpgrade.Count >= 3)
        {
            for (int i = 0; i < 3; i++)
            {
                var uShip = upgradeShips[i];
                uShip.SetShip(randomUpgrade[i]);
                uShip.Show();
            }
        }
        else if (randomUpgrade.Count == 2)
        {
            var uShip = upgradeShips[0];
            uShip.SetShip(randomUpgrade[0]);
            uShip.Show();

            uShip = upgradeShips[2];
            uShip.SetShip(randomUpgrade[1]);
            uShip.Show();
        }
        else if (randomUpgrade.Count == 1)
        {
            var uShip = upgradeShips[1];
            uShip.SetShip(randomUpgrade[0]);
            uShip.Show();
        }
        else
        {
            var uShip = upgradeShips[1];
            uShip.SetShip(UpgradeShip.UpgradeType.Heal);
            uShip.Show();
        }

    }

    public void FinishUpgradePhase()
    {
        if (!isActive) return;
        isActive = false;
        foreach (var uShip in upgradeShips)
        {
            uShip.Hide();
        }
        this.DelayAction(1f, () => { 
            levelController.StartLevel();
        });
    }

    public bool CanUpgrade(UpgradeShip.UpgradeType type)
    {
        switch (type)
        {
            case UpgradeShip.UpgradeType.None:
                return true;
            case UpgradeShip.UpgradeType.Firerate:
                return player.fireRate_UpgradeUnlocks < Player.max_fireRate_UpgradeUnlocks;
            case UpgradeShip.UpgradeType.MaxHP:
                return player.maxHP_UpgradesUnlocked < Player.max_maxHP_UpgradesUnlocked;
            case UpgradeShip.UpgradeType.ShieldSlot:
                return player.bubbleSlot_UpgradeUnlocks < Player.max_bubbleSlot_UpgradeUnlocks;
            case UpgradeShip.UpgradeType.Missile:
                return player.missile_UpgradeUnlocks < Player.max_missile_UpgradeUnlocks;
            default:
                return false;
        }
    }
}
