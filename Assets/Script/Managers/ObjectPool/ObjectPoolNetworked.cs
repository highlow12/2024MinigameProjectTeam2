using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PooledNetworkObjectProvider : NetworkObjectProviderDefault
{
    protected override NetworkObject InstantiatePrefab(NetworkRunner runner, NetworkObject prefab)
    {
        // Get object from pool and return it.
        return null;
    }

    protected override void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, NetworkObject instance)
    {
        // Return the instance to the pool.
    }
}