using Fusion;
using System.Collections;
using UnityEngine;

public class SpawnFireSpirit : BossSkill
{
    public SpawnFireSpirit()
    {
        name = "SpawnFireSpirit";
        phase = 2;
        attackDamage = 0;
        projectile = Resources.Load<GameObject>("FireSpirit");
    }
    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject boss = null)
    {
        Debug.Log("skill " + name);
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
