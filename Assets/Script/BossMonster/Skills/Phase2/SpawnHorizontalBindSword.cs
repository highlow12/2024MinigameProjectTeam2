using Fusion;
using System.Collections;
using UnityEngine;

public class SpawnHorizontalBindSword : BossSkill
{
    public SpawnHorizontalBindSword()
    {
        name = "SpawnHorizontalBindSword";
        phase = 2;
        baseDamage = 1.2f;
        attackDamage = 1.2f;
        /* damage per tick -> total damage: bind duration 3s * tick rate 64 * attackDamage 1.2 = 230.4 */
        projectile = Resources.Load<GameObject>("BindSword");
    }
    GameObject omenEffect = Resources.Load<GameObject>("HorizontalBindSwordOmenEffect");
    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject boss = null)
    {
        Debug.Log("skill " + name);
        var omenPos = new Vector3(0, -1, 0);
        var projectilePos = new Vector3(-30, -1, 0);
        NetworkObject omen = runner.Spawn(omenEffect, omenPos, Quaternion.identity, boss.InputAuthority);
        CustomTickTimer omenDuration = CustomTickTimer.CreateFromSeconds(runner, 1.5f);
        while (!omenDuration.Expired(runner))
        {
            yield return new WaitForFixedUpdate();
        }
        runner.Spawn(projectile, projectilePos, Quaternion.Euler(0, 0, 90), boss.InputAuthority, (runner, o) =>
        {
            var s = o.GetComponent<BindSword>();
            s.boss = boss;
            s.damagePerTick = attackDamage;
            o.GetComponent<Rigidbody2D>().gravityScale = 0;
            s.StartCoroutine(s.ApplyHorizontalMovement());
        }
);


        yield return null;
    }
}
