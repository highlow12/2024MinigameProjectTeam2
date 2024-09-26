using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using System;

public class Rush1 : BossSkill
{
    public Rush1()
    {
        name = "Rush";
        attackDamage = 50.0f;
        phase = 1;
    }

    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject boss = null)
    {
        BossMonsterNetworked bossScript = boss.GetComponent<BossMonsterNetworked>();
        CapsuleCollider2D _collider = boss.GetComponent<CapsuleCollider2D>();
        Rigidbody2D _rb = boss.GetComponent<Rigidbody2D>();
        bossScript.P_DoRush = true;
        bossScript.isRushing = true;
        float omenLength = 0.5f;
        float attackLength = 0.95f;
        var omenLengthTimer = CustomTickTimer.CreateFromSeconds(runner, omenLength);
        while (!omenLengthTimer.Expired(runner))
        {
            yield return new WaitForFixedUpdate();
        }
        bossAttack.playersHit = new List<PlayerRef>();
        bossAttack.damage = attackDamage;
        bossAttack.isApplyKnockback = true;
        _collider.excludeLayers = LayerMask.GetMask("PlayerLayer");
        int direction = (bossScript.FollowTarget.transform.position.x - transform.position.x) > 0 ? 1 : -1;
        transform.localScale = new Vector3(direction * -2, 2, 1);
        Debug.Log(direction);
        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(runner, attackLength);
        while (!attackLengthTimer.Expired(runner))
        {
            _rb.velocity = new Vector2(direction * 10, 0);
            yield return new WaitForFixedUpdate();
        }
        _rb.velocity = Vector2.zero;
        bossScript.isRushing = false;
        _collider.excludeLayers = 0;
        yield return null;
    }

}