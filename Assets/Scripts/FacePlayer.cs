using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    Player player;

    public float turningSpeed;

    private void Awake()
    {
        player = FindAnyObjectByType<Player>();
    }

    private void Update()
    {
        transform.up = Vector3.Lerp(transform.up, (player.transform.position - transform.position).normalized, turningSpeed * Time.deltaTime);
    }

}
