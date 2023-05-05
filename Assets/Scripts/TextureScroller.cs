using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    public Material material;
    public Vector2 direction;
    public float speed;
    void Update()
    {
        material.mainTextureOffset += direction * speed * Time.deltaTime;
    }

    private void OnDestroy()
    {
        material.mainTextureOffset = Vector2.zero;
    }
}
