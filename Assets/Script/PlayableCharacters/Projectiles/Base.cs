using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : PoolAble
{
    public int shotType;
    public GameObject projectile;
    public float projectileSpeed = 0.0f;
    public float damage;
    public float range;
    public bool isReady;

    void Start()
    {
        // Release on sync by fusion after initial release
        if (projectileSpeed == 0.0f)
        {
            gameObject.SetActive(false);
        }
    }
}