using Fusion;
using System.Collections;
using UnityEngine;

public class SpawnBindSword : BossSkill
{
    public SpawnBindSword()
    {
        name = "SpawnBindSword";
        phase = 2;
        attackDamage = 0;
        projectile = Resources.Load<GameObject>("BindSword");
    }
    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject boss = null)
    {
        Debug.Log("skill " + name);
        Vector2[] spawnPositions = new Vector2[4];
        Vector2 initalSpawnPosition = new Vector2(transform.position.x + Random.Range(-10.0f, 0f), transform.position.y + 10);
        for (int i = 0; i < 4; i++)
        {
            spawnPositions[i] = initalSpawnPosition + new Vector2(i * 5, 0);
        }
        foreach (var pos in spawnPositions)
        {
            runner.Spawn(projectile,
            pos, Quaternion.Euler(0, 0, 0),
                boss.InputAuthority, (runner, o) =>
                {
                    var s = o.GetComponent<BindSword>();
                    s.boss = boss;
                }
            );
        }

        yield return null;
    }
}
