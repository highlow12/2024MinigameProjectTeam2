using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using System;

public class BackAttack : BossSkill
{
    public BackAttack()
    {
        name = "BackAttack";
        attackDamage = 50.0f;
        phase = 1;
    }

    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack)
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, 2, 1);
        animator.SetTrigger("doAttack2");
        float attackLength = 1f;
        // attack logic by animation event required
        bossAttack.playersHit = new List<PlayerRef>();
        bossAttack.damage = attackDamage;
        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(runner, attackLength);
        while (!attackLengthTimer.Expired(runner))
        {
            yield return new WaitForFixedUpdate();
        }
        transform.localScale = new Vector3(transform.localScale.x * -1, 2, 1);
        yield return null;
    }

    public override IEnumerator AttackWithProjectile(Transform transform, Animator animator, NetworkRunner runner, NetworkObject projectile, NetworkObject boss)
    {
        throw new NotImplementedException();
    }
}