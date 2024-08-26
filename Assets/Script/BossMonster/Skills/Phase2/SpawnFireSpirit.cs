using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Fusion.Sockets.NetBitBuffer;

public class SpawnFireSpirit : BossSkill
{
    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject projectile = null, NetworkObject boss = null)
    {
        runner.Spawn(projectile,
        transform.position, Quaternion.Euler(0, 0, 0),
            boss.InputAuthority, (runner, o) =>
            {
                
                var s = o.GetComponent<FireSpirit>();
                o.transform.parent = transform;
                s.setTarget(transform.GetComponent<BossMonsterNetworked>().FollowTarget.transform.position);

            }
        );
        yield return null;
    }
}
