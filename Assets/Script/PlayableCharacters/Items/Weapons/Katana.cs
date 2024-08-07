using System.Collections;
using UnityEngine;
using Items;
using Fusion;
using System.Linq;

public class Katana : Weapon
{
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
            attackState++;
            anim.SetFloat("AttackAnimSpeed", 0.5f * attackSpeed);
            anim.SetInteger("AttackState", attackState);
            prevAttack = Time.time;
            anim.SetFloat("PrevAttack", prevAttack);
            mecanim.SetTrigger("Attack", true);
            anim.SetBool("Combo", true);
            playerAttack.isHit = false;
            playerAttack.damage = damage;
            isAttackCooldown = true;
            attackTimer = CustomTickTimer.CreateFromSeconds(runner, 1.0f / attackSpeed);
            while (attackTimer.Expired(runner) == false)
            {
                yield return new WaitForFixedUpdate();
            }
            isAttackCooldown = false;
            attackTimer = CustomTickTimer.CreateFromSeconds(runner, 0.3f);
            while (attackTimer.Expired(runner) == false)
            {
                yield return new WaitForFixedUpdate();
            }
            if (Time.time - prevAttack >= 1.0f / attackSpeed + 0.3f)
            {
                anim.SetInteger("AttackState", 0);
                anim.SetBool("Combo", false);
                attackState = 0;
            }
        }
        else
        {
            yield return null;
        }
    }


}
