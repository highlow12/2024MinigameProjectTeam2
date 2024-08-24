using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using System;

public class JumpAttack : BossSkill
{
    public JumpAttack()
    {
        name = "JumpAttack";
        attackDamage = 200.0f;
        phase = 1;
    }

    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject projectile = null, NetworkObject boss = null)
    {
        animator.SetTrigger("doJumpAttack");

        float attackLength = 2.2f;

        // attack logic by animation event required
        bossAttack.playersHit = new List<PlayerRef>();
        bossAttack.damage = attackDamage;

        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(runner, attackLength);
        while (!attackLengthTimer.Expired(runner))
        {
            yield return new WaitForFixedUpdate();
        }
        bossAttack.damage = 0.0f;
        yield return null;
    }


}