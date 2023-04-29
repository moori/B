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
        Fire = Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.J);
        Bubble = Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.K);
        Slow = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.L);
        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = Input.GetAxisRaw("Vertical");
    }
}
