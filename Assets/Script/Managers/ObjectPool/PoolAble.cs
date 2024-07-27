using UnityEngine;
using UnityEngine.Pool;
using Fusion;

public class PoolAble : NetworkBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }

    public void Awake()
    {
    }

    public void ReleaseObject()
    {
        Pool.Release(gameObject);
    }
}