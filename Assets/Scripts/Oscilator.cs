using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscilator : MonoBehaviour
{
    public float oscSpeed;
    public float oscPhaseDegrees;
    public float angle;

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles = new Vector3(0, 0, angle * Mathf.Sin((oscPhaseDegrees * Mathf.Deg2Rad) + (oscSpeed * (Time.time*Mathf.Deg2Rad))));
    }
}
