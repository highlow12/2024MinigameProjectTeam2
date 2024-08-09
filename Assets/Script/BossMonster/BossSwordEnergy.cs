using Fusion;
using UnityEngine;

public class BossSwordEnergy : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
            Runner.Despawn(Object);
    }

    public void shoot(Vector3 dir)
    {
        gameObject.SetActive(true);
    }
}
