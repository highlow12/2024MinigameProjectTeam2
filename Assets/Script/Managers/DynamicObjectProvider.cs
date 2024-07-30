using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class DynamicObjectProvider : NetworkBehaviour
{
    public NetworkPrefabRef arrowPrefab;
    public NetworkPrefabRef arrowEffectPrefab;

    // [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    // public void RPC_SpawnObjectByName(string objectName)
    // {
    //     switch (objectName)
    //     {
    //         case "Arrow":
    //             RPC_SpawnObject(arrowPrefab, Vector3.zero, Quaternion.identity);
    //             break;
    //         case "ArrowEffect":
    //             RPC_SpawnObject(arrowEffectPrefab, Vector3.zero, Quaternion.identity);
    //             break;
    //     }

    // }

    // [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    // public void RPC_SpawnObject(NetworkPrefabRef prefab, Vector3 position, Quaternion rotation)
    // {
    //     NetworkObject spawnedObject = Runner.Spawn(prefab, position, rotation, null);
    // }

    // [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    // public void RPC_DespawnObject(NetworkObject networkObject)
    // {
    //     Runner.Despawn(networkObject);
    // }

}