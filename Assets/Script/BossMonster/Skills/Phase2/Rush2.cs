using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public class Rush2 : BossSkill
{
    public Rush2()
    {
        name = "RushAttack";
        baseDamage = 150.0f;
        attackDamage = 150.0f;
        phase = 2;
    }

    public override IEnumerator Attack(Transform transform, Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject boss = null)
    {
        BossMonsterNetworked bossScript = boss.GetComponent<BossMonsterNetworked>();
        CapsuleCollider2D _collider = boss.GetComponent<CapsuleCollider2D>();
        Rigidbody2D _rb = boss.GetComponent<Rigidbody2D>();
        bossScript.P_DoRush = true;
        bossScript.isRushing = true;
        float omenLength = 0.33f;
        float attackLength = 0.1f;
        var omenLengthTimer = CustomTickTimer.CreateFromSeconds(runner, omenLength);
        while (!omenLengthTimer.Expired(runner))
        {
            yield return new WaitForFixedUpdate();
        }
        bossAttack.playersHit = new List<PlayerRef>();
        bossAttack.damage = attackDamage;
        bossAttack.isApplyKnockback = true;
        bossAttack.isParryable = false;
        _collider.excludeLayers = LayerMask.GetMask("PlayerLayer");
        int rushDirectionType = Random.Range(0, 2);
        int direction = 1;
        switch (rushDirectionType)
        {
            case 0:
                IEnumerable<float> playersXPos = runner.ActivePlayers.ToArray()
                    .Select(player => runner.TryGetPlayerObject(player, out NetworkObject playerObject)
                        ? playerObject.transform.position.x : 0);
                int positiveX = 0;
                int negativeX = 0;
                foreach (var playerXPos in playersXPos)
                {
                    if (playerXPos > transform.position.x)
                    {
                        positiveX++;
                    }
                    else
                    {
                        negativeX++;
                    }
                }
                direction = positiveX > negativeX ? 1 : -1;
                break;
            case 1:
                direction = (bossScript.FollowTarget.transform.position.x - transform.position.x) > 0 ? 1 : -1;
                break;
        }
        transform.localScale = new Vector3(direction * -2, 2, 1);
        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(runner, attackLength);
        while (!attackLengthTimer.Expired(runner))
        {
            _rb.velocity = new Vector2(direction * 100, 0);
            yield return new WaitForFixedUpdate();
        }
        _rb.velocity = Vector2.zero;
        bossScript.isRushing = false;
        bossAttack.isParryable = true;
        bossScript.RPC_ForceRetarget();
        _collider.excludeLayers = 0;
        yield return null;
    }

}