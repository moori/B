using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerHelper : MonoBehaviour
{
    public float rotationSpeed;
    void Update()
    {
        transform.rotation = transform.rotation * Quaternion.Euler(0, 0, rotationSpeed * Time.deltaTime);
    }
}
