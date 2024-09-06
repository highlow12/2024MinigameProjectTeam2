using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bind : BossSkill
{
    public Bind()
    {
        name = "Bind";
        phase = 2;
        attackDamage = 10;
    }
    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject projectile = null, NetworkObject boss = null)
    {
        Debug.Log("skill " + name);
        yield return new WaitForEndOfFrame();
    }
}
