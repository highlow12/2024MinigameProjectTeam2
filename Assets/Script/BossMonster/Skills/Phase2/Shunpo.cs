using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shunpo : BossSkill
{
    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject projectile = null, NetworkObject boss = null)
    {
        yield return null;
    }

}
