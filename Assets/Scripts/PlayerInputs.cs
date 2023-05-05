using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public static bool Fire;
    public static bool Bubble;
    public static bool Slow;
    public static float Horizontal;
    public static float Vertical;

    private void Update()
    {
        if (GameController.IsGamePaused) return;
        Fire = Input.GetButton("Fire") || Input.GetButton("Fire1");
        Bubble = Input.GetButton("Bubble") || Input.GetButton("Fire2");
        Slow = Input.GetButton("Slow") || Input.GetButton("Fire3");
        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = Input.GetAxisRaw("Vertical");
    }
}
