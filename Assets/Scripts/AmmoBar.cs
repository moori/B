using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoBar : MonoBehaviour
{
    public Player player;
    public List<Image> images;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }
    public void Spend()
    {
        UpdateBar(player.ammo / (float)player.maxAmmo);
    }

    public void UpdateBar(float percent)
    {
        foreach (var item in images)
        {
            item.fillAmount = percent;
        }
    }
}
