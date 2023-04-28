using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMkr : MonoBehaviour
{
    public Sprite sprite;
    public int width;
    public int height;
    public float gutter;
    public float size;
    public float scale;
    public Color color;

    [ContextMenu("CreateGrid")]
    public void CreateGrid()
    {
        foreach (Transform child in transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var sp = new GameObject("Sprite", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
                sp.sprite = sprite;
                sp.transform.parent = transform;
                sp.transform.localRotation = Quaternion.identity;
                sp.transform.localPosition = new Vector3((size + gutter) * x, (size + gutter) * y, 0);
                sp.transform.localScale = Vector3.one * scale;
                sp.color = color;
            }
        }
    }
}
