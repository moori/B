using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class BulletPool : MonoBehaviour
{
    public Bullet bulletPrefab;
    public int amount;
    public List<Bullet> pool = new List<Bullet>();

    private void Awake()
    {
        Setup();
    }

    public void Setup()
    {
        for (int i = 0; i < amount; i++)
        {
            var b = Instantiate(bulletPrefab);
            pool.Add(b);
            b.gameObject.SetActive(false);
        }
    }

    public Bullet GetBullet()
    {
        var b = pool.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
        if (b)
        {
            return b;
        }
        else
        {
            Debug.LogError("Not enought bullets on pool");
            return null;
        }
    }

}
