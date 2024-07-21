using System.Collections;
using UnityEngine;
using Items;

public class Bow : Weapon
{
    private bool isAttackCooldown = false;
    public Bow(float attackSpeed)
    {
        this.attackSpeed = attackSpeed;
        projectileSpeed = 10.0f;
        range = 3.0f;
        damage = 50;
    }

    public override IEnumerator Attack(Animator anim, Transform character)
    {

        if (!isAttackCooldown)
        {
            // 0.5f = animation length
            // 0.3f = combo delay

            if (attackState == 3)
            {
                attackState = 0;
            }
            attackState++;
            // 애니메이션 배속
            anim.SetFloat("AttackAnimSpeed", 0.5f * attackSpeed);
            anim.SetInteger("AttackState", attackState);
            prevAttack = Time.time;
            anim.SetFloat("PrevAttack", prevAttack);
            anim.SetTrigger("Attack");
            var projectile = ObjectPoolManager.Instance.GetGo("ArcherProjectile");
            Vector3 scale = character.localScale;
            // character's scale must be 1 or -1
            projectile.transform.position = character.position + new Vector3(scale.x * 1.5f, 0, 0);
            projectile.transform.localScale = scale;
            Arrow arrow = projectile.GetComponent<Arrow>();
            arrow.projectileSpeed = projectileSpeed;
            arrow.damage = damage;
            arrow.range = range;
            anim.SetBool("Combo", true);
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
