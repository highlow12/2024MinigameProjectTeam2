using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using System;

public class BackAttack2 : BossSkill
{
    public BackAttack2()
    {
        name = "BackAttack2";
        baseDamage = 75.0f;
        attackDamage = 75.0f;
        phase = 2;
    }

    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject boss = null)
    {
        BossMonsterNetworked bossScript = boss.GetComponent<BossMonsterNetworked>();
        bossScript.P_DoAttack2 = true;
        transform.localScale = new Vector3(transform.localScale.x * -1, 2, 1);
        float attackLength = 1.1f;
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