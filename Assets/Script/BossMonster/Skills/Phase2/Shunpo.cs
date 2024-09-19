using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shunpo : BossSkill
{
    public float WaitTime = 3;
    public Shunpo()
    {
        name = "Shunpo";
        phase = 2;
        attackDamage = 10;
        projectile = Resources.Load<GameObject>("Phase2Sword");
    }

    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject boss = null)
    {
        Debug.Log("skill " + name);
        if (projectile.TryGetComponent<Phase2Sword>(out var sword))
        {
            BossMonsterNetworked bossScript = boss.GetComponent<BossMonsterNetworked>();
            bossScript.P_Shunpo = true;
            var target = boss.GetComponent<BossMonsterNetworked>().FollowTarget;
            sword.ThrowSword(target.transform.position);
            yield return new WaitForSeconds(WaitTime);
            transform.position = sword.transform.position;
        }
        yield return null;
    }

}
