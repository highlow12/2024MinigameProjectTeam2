using System.Collections;
using System.Collections.Generic;
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
        damages = new List<float> { 100.0f, 200.0f, 300.0f };
    }

    public override IEnumerator Attack(Animator anim, NetworkMecanimAnimator mecanim, Transform character)
    {
        if (!isAttackCooldown)
        {
            // 0.5f = animation length
            // 0.3f = combo delay
            NetworkRunner runner = NetworkRunner.Instances.First();
            PlayerAttack playerAttack = rangeObject.GetComponent<PlayerAttack>();

            if (controller.AttackState == 3)
            {
                controller.AttackState = 0;
            }
            if (controller.AttackState == 2)
            {
                // 3rd attack has 2x length
                cooldownMultiplier = 2.0f;
            }
            else
            {
                cooldownMultiplier = 1.0f;
            }
            controller.AttackState++;
            // 애니메이션 배속
            controller.AttackAnimSpeed = 0.5f * attackSpeed;
            controller.AttackState = controller.AttackState;
            prevAttack = (int)runner.Tick;
            controller.PrevAttack = prevAttack;
            controller.Attack = true;
            controller.Combo = true;
            playerAttack.isHit = false;
            playerAttack.damage = damages[controller.AttackState - 1] * damageMultiplier;
            playerAttack.attackType = PlayerAttack.AttackType.Katana;
            isAttackCooldown = true;
            attackTimer = CustomTickTimer.CreateFromSeconds(runner, 1.0f / attackSpeed * cooldownMultiplier);
            while (attackTimer.Expired(runner) == false)
            {
                yield return new WaitForFixedUpdate();
            }
            isAttackCooldown = false;
            if (controller.AttackState < 3)
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
                    controller.AttackState = 0;
                    controller.Combo = false;
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
