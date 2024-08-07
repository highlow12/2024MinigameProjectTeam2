using System.Collections;
using UnityEngine;
using Items;
using Fusion;
using System.Linq;

public class Katana : Weapon
{
    private float cooldownMultiplier = 1.0f;
    private bool isAttackCooldown = false;
    private CustomTickTimer attackTimer;
    public Katana(float attackSpeed)
    {
        this.attackSpeed = attackSpeed;
        range = 2.0f;
        damage = 60.0f;
    }

    public override IEnumerator Attack(Animator anim, NetworkMecanimAnimator mecanim, Transform character)
    {
        if (!isAttackCooldown)
        {
            // 0.5f = animation length
            // 0.3f = combo delay
            NetworkRunner runner = NetworkRunner.Instances.First();
            PlayerAttack playerAttack = rangeObject.GetComponent<PlayerAttack>();


            if (attackState == 3)
            {
                attackState = 0;
            }
            if (attackState == 2)
            {
                // 3rd attack has 2x length
                cooldownMultiplier = 2.0f;
            }
            else
            {
                cooldownMultiplier = 1.0f;
            }
            attackState++;
            anim.SetFloat("AttackAnimSpeed", 0.5f * attackSpeed);
            anim.SetInteger("AttackState", attackState);
            prevAttack = (int)runner.Tick;
            anim.SetFloat("PrevAttack", prevAttack);
            mecanim.SetTrigger("Attack", true);
            anim.SetBool("Combo", true);
            playerAttack.isHit = false;
            playerAttack.damage = damage * damageMultiplier;
            isAttackCooldown = true;
            attackTimer = CustomTickTimer.CreateFromSeconds(runner, 1.0f / attackSpeed * cooldownMultiplier);
            while (attackTimer.Expired(runner) == false)
            {
                yield return new WaitForFixedUpdate();
            }
            isAttackCooldown = false;
            if (attackState < 3)
            {
                attackTimer = CustomTickTimer.CreateFromSeconds(runner, 0.3f);
                while (attackTimer.Expired(runner) == false)
                {
                    yield return new WaitForFixedUpdate();
                }
                // 3틱 차이까지는 허용
                // 테스트 결과 1~2틱 정도 오차가 자주 발생하였음
                if (((int)runner.Tick - prevAttack) / runner.TickRate >= 1.0f / attackSpeed + (0.3f - 3.0f / runner.TickRate))
                {
                    anim.SetInteger("AttackState", 0);
                    anim.SetBool("Combo", false);
                    attackState = 0;
                }
            }
        }
        else
        {
            yield return null;
        }
    }


}
