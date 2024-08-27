using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bind : BossSkill
{
    Bind()
    {
        name = "bind";
        phase = 2;
        attackDamage = 10;
    }
    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject projectile = null, NetworkObject boss = null)
    {
        yield return new WaitForEndOfFrame();
    }
}
