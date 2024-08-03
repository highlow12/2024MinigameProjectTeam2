using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ArrowEffect : NetworkBehaviour
{

    [Networked]
    public int ShotType { get; set; }
    public NetworkMecanimAnimator _networkAnimator;

    void Awake()
    {
        _networkAnimator = GetComponent<NetworkMecanimAnimator>();
    }

    public override void Render()
    {
        if (!HasStateAuthority) return;
        _networkAnimator.Animator.SetInteger("ShotType", ShotType);
    }


    void Despawn()
    {
        NetworkRunner runner = GameObject.FindAnyObjectByType<NetworkManager>().Runner;
        runner.Despawn(GetComponent<NetworkObject>());
    }

}
