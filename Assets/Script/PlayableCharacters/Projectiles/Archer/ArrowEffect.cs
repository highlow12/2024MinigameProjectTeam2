using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ArrowEffect : NetworkBehaviour
{

    public int ShotType;
    public Animator _anim;

    public override void Spawned()
    {
        base.Spawned();
        _anim = GetComponent<Animator>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        _anim.SetInteger("ShotType", ShotType);
    }


    void Despawn()
    {
        NetworkRunner runner = GameObject.FindAnyObjectByType<NetworkManager>().Runner;
        runner.Despawn(GetComponent<NetworkObject>());
    }

}
