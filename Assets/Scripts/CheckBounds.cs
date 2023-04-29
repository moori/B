using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBounds : MonoBehaviour
{
    public const float H_Bound = 12;
    public const float V_Bound = 9;


    private void FixedUpdate()
    {

        if (Mathf.Abs(transform.position.x) >= H_Bound || Mathf.Abs(transform.position.y) >= V_Bound)
        {
            gameObject.SetActive(false);
        }
    }
}
