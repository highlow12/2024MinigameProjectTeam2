using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBossMonster : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }

    public void Init()
    {
        life = TickTimer.CreateFromSeconds(Runner, 100.0f);
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
            Runner.Despawn(Object);
    }

}
