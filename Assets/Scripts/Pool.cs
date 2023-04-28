using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pool : MonoBehaviour
{
    public GameObject prefab;
    public int amount;
    public List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        Setup();
    }

    public void Setup()
    {
        for (int i = 0; i < amount; i++)
        {
            var b = Instantiate(prefab);
            pool.Add(b);
            b.gameObject.SetActive(false);
        }
    }

    public GameObject GetItem()
    {
        var b = pool.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
        if (b)
        {
            return b;
        }
        else
        {
            Debug.LogError($"Not enought elements ({prefab.gameObject.name}) in the pool {gameObject.name}");
            return null;
        }
    }
}
