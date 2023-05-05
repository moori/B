using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    public TextMeshProUGUI counterText;

    public Color defaultColor;
    public Color lowColor;

 
    public void UpdateBar(int ammo, int maxAmmo)
    {
        counterText.text = $"{ammo}/{maxAmmo}";
        counterText.color = ammo < Player.BASE_MAX_AMMO * 0.2f ? lowColor : defaultColor;
    }
}
