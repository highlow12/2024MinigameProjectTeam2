using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using System;

public class BothAttack : BossSkill
{
    public BothAttack()
    {
        name = "BothAttack";
        attackDamage = 50.0f;
        phase = 1;
    }

    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject boss = null)
    {
        BossMonsterNetworked bossScript = boss.GetComponent<BossMonsterNetworked>();
        bossScript.P_DoAttack = true;
        float attackLength = 1.2f;
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
        bossScript.P_DoAttack2 = true;
        attackLength = 1f;
        // attack logic by animation event required
        bossAttack.playersHit = new List<PlayerRef>();

        attackLengthTimer = CustomTickTimer.CreateFromSeconds(runner, attackLength);
        while (!attackLengthTimer.Expired(runner))
        {
            yield return new WaitForFixedUpdate();
        }
        transform.localScale = new Vector3(transform.localScale.x * -1, 2, 1);
        yield return null;
    }


}