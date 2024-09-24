using Fusion;
using System.Collections;
using UnityEngine;

public class SpawnHorizontalBindSword : BossSkill
{
    public SpawnHorizontalBindSword()
    {
        name = "SpawnHorizontalBindSword";
        phase = 2;
        attackDamage = 1.2f;
        /* damage per tick -> total damage: bind duration 3s * tick rate 64 * attackDamage 1.2 = 230.4 */
        projectile = Resources.Load<GameObject>("BindSword");
    }
    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject boss = null)
    {
        Debug.Log("skill " + name);
        var pos = new Vector3(-30, -3,0);
        var initVal = new Vector3(30,0,0);
        runner.Spawn(projectile, pos, Quaternion.Euler(0, 0, 90), boss.InputAuthority, (runner, o) =>
            {
                var s = o.GetComponent<BindSword>();
                s.boss = boss;
                s.damagePerTick = attackDamage;
                o.GetComponent<Rigidbody2D>().gravityScale = 0;
                o.GetComponent<Rigidbody2D>().velocity = initVal;
            }
        );
        

        yield return null;
    }
}
