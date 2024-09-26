using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class Base : NetworkBehaviour
{
    public int shotType;
    public GameObject projectile;
    public float projectileSpeed = 0.0f;
    public float damage;
    public float range;
    public NetworkMecanimAnimator _networkAnimator;
    NetworkRigidbody2D _rb;

    void Awake()
    {
        _networkAnimator = GetComponent<NetworkMecanimAnimator>();
        _rb = GetComponentInChildren<NetworkRigidbody2D>();
    }

}