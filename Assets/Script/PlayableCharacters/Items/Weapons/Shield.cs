using UnityEngine;
using Items;
using System.Collections;
using Fusion;

public class Shield : Weapon
{
    private bool isAttackCooldown = false;
    public Shield(float attackSpeed)
    {
        this.attackSpeed = attackSpeed;
        range = 2.0f;
        damage = 30.0f;
        defense = 100;
        healingAmount = 50;
    }

    public override IEnumerator Attack(Animator anim, NetworkMecanimAnimator mecanim, Transform character)
    {
        if (!isAttackCooldown)
        {
            // 0.5f = animation length
            // 0.3f = combo delay

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
            yield return new WaitForSeconds(0.1f);
            isAttackCooldown = true;
            yield return new WaitForSeconds(1.0f / attackSpeed - 0.1f);
            isAttackCooldown = false;
            yield return new WaitForSeconds(0.3f);
            if (Time.time - prevAttack > 1.0f / attackSpeed + 0.3f)
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