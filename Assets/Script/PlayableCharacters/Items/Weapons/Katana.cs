using System.Collections;
using UnityEngine;
using Items;
using Fusion;

public class Katana : Weapon
{
    private bool isAttackCooldown = false;
    public Katana()
    {
        attackSpeed = 2.0f;
        attackState = 0;
        prevAttack = 0f;
        range = 2.0f;
        damage = 60.0f;
        isRangeObjectSpawned = false;
        rangeObject = Resources.Load<GameObject>("KatanaRange");
    }

    public override IEnumerator Attack(Animator anim, NetworkMecanimAnimator mecanim, Transform character)
    {
        if (!isRangeObjectSpawned)
        {
            rangeObject = GameObject.Instantiate(rangeObject, character);
            rangeObject.GetComponent<Attack>().damage = damage;
            isRangeObjectSpawned = true;
        }
        if (!isAttackCooldown)
        {
            // 0.5f = animation length
            // 0.3f = combo delay

            if (attackState == 3)
            {
                attackState = 0;
            }
            attackState++;
            anim.SetFloat("AttackAnimSpeed", 0.5f * attackSpeed);
            anim.SetInteger("AttackState", attackState);
            prevAttack = Time.time;
            anim.SetFloat("PrevAttack", prevAttack);
            mecanim.SetTrigger("Attack");
            anim.SetBool("Combo", true);
            rangeObject.GetComponent<Collider2D>().enabled = true;
            yield return new WaitForSeconds(0.1f);
            rangeObject.GetComponent<Collider2D>().enabled = false;
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
