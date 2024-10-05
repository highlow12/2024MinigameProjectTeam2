using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class Rush1 : BossSkill
{
    public Rush1()
    {
        name = "RushAttack";
        attackDamage = 100.0f;
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

        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(runner, attackLength);
        while (!attackLengthTimer.Expired(runner))
        {
            transform.localScale = new Vector3(direction * -2, 2, 1);
            _rb.velocity = new Vector2(direction * 10, 0);
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