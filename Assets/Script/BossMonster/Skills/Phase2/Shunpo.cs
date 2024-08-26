using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shunpo : BossSkill
{
    public float WaitTime = 3;
    Shunpo()
    {
        name = "Shunpo";
        phase = 2;
        attackDamage = 10;
    }

    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject projectile = null/*<-this is sword*/, NetworkObject boss = null)
    {
        if (projectile.TryGetComponent<Phase2Sword>(out var sword))
        {
            animator.SetTrigger("Shunpo");
            var target = boss.GetComponent<BossMonsterNetworked>().FollowTarget;
            sword.ThrowSword(target.transform.position);
            yield return new WaitForSeconds(WaitTime);
            transform.position = sword.transform.position;
        }
        yield return null;
    }

}
