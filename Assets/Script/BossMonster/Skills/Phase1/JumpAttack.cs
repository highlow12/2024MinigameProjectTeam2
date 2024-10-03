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

    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject boss = null)
    {
        BossMonsterNetworked bossScript = boss.GetComponent<BossMonsterNetworked>();
        bossScript.P_DoJumpAttack = true;
        float attackLength = 2.2f;
        // attack logic by animation event required
        bossAttack.playersHit = new List<PlayerRef>();
        bossAttack.damage = attackDamage;
        bossAttack.isApplyKnockback = false;
        bossAttack.isParryable = false;
        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(runner, attackLength);
        while (!attackLengthTimer.Expired(runner))
        {
            yield return new WaitForFixedUpdate();
        }
        bossAttack.damage = 0.0f;
        bossAttack.isParryable = true;
        yield return null;
    }


}