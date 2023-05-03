using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float lifetime = 10f;
    public Transform modelTransform;
    public float rotateSpeed = 2f;
    private float speed;

    private float spawnSpreadDuration;
    private float spawnTime;
    private bool isActive;

    [Header("Shadow")]
    public Transform shadowTransform;
    public float startScale;
    public float endScale;


    private void Update()
    {
        if (!isActive) return;

        if (Time.time - spawnTime <= spawnSpreadDuration)
        {
            transform.position += transform.up * speed * Time.deltaTime;
        }

        if (Time.time - spawnTime >= lifetime)
        {
            Kill();
        }
        shadowTransform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, (Time.time - spawnTime) / lifetime);

        modelTransform.Rotate(Vector3.forward, rotateSpeed*Time.deltaTime);
    }

    public void Kill()
    {
        gameObject.SetActive(false);
        isActive = false;
    }

    internal void Spawn(Vector3 position, float speed=10f)
    {
        var direction = Random.insideUnitSphere;
        direction = new Vector3(direction.z, direction.y, 0).normalized;
        transform.up = direction;

        this.speed = speed;
        spawnSpreadDuration = Random.Range(0.05f, 0.2f);
        transform.position = position;
        spawnTime = Time.time;
        isActive = true;
        gameObject.SetActive(true);
    }
}
