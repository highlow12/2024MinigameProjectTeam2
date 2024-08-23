using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using System;

public class BaseAttack1 : BossSkill
{
    public BaseAttack1()
    {
        name = "BaseAttack1";
        attackDamage = 50.0f;
        phase = 1;
    }

    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack)
    {
        animator.SetTrigger("doAttack");
        float attackLength = 1.2f;
        // attack logic by animation event required
        bossAttack.playersHit = new List<PlayerRef>();
        bossAttack.damage = attackDamage;
        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(runner, attackLength);
        while (!attackLengthTimer.Expired(runner))
        {
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    public override IEnumerator AttackWithProjectile(Transform transform, Animator animator, NetworkRunner runner, NetworkObject projectile, NetworkObject boss)
    {
        throw new NotImplementedException();
    }
}