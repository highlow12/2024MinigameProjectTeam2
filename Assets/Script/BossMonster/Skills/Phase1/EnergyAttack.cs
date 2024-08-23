using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using System;
using Unity.Mathematics;

public class EnergyAttack : BossSkill
{
    public EnergyAttack()
    {
        name = "EnergyAttack";
        attackDamage = 100.0f;
        phase = 1;
    }

    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack)
    {
        throw new NotImplementedException();
    }

    public override IEnumerator AttackWithProjectile(Transform transform, Animator animator, NetworkRunner runner, NetworkObject projectile, NetworkObject boss)
    {
        animator.SetTrigger("doAttack");
        float attackLength = 1.2f;
        Vector3 yOffset = ((Vector3)Vector2.up * 2);
        Vector3 xOffset = transform.localScale.x > 0 ? Vector3.left : Vector3.right;
        // attack logic by animation event required
        runner.Spawn(projectile,
            transform.position + yOffset + xOffset, quaternion.Euler(0, 0, 0),
            boss.InputAuthority, (runner, o) =>
            {
                // Initialize the sword energy before synchronizing it
                var script = o.GetComponent<BossSwordEnergy>();
                script.Init((int)transform.localScale.x);
                script.damage = (int)attackDamage;

            }
        );
        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(runner, attackLength);
        while (!attackLengthTimer.Expired(runner))
        {
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
}