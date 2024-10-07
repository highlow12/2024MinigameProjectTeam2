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
        baseDamage = 50.0f;
        attackDamage = 50.0f;
        phase = 1;
    }

    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject boss = null)
    {
        BossMonsterNetworked bossScript = boss.GetComponent<BossMonsterNetworked>();
        bossScript.P_DoAttack2 = true;
        transform.localScale = new Vector3(transform.localScale.x * -1, 2, 1);
        float attackLength = 1f;
        // attack logic by animation event required
        bossAttack.playersHit = new List<PlayerRef>();
        bossAttack.damage = attackDamage;
        bossAttack.isApplyKnockback = true;
        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(runner, attackLength);
        while (!attackLengthTimer.Expired(runner))
        {
            yield return new WaitForFixedUpdate();
        }
        transform.localScale = new Vector3(transform.localScale.x * -1, 2, 1);
        yield return null;
    }

}