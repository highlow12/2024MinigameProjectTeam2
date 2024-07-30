using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ArrowEffect : NetworkBehaviour
{

    void Despawn()
    {
        NetworkRunner runner = GameObject.FindAnyObjectByType<NetworkManager>().Runner;
        runner.Despawn(GetComponent<NetworkObject>());
    }

}
