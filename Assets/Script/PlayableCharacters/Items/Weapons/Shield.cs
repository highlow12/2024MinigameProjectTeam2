using UnityEngine;
using Items;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using System.Linq;
using Fusion.Addons.Physics;

public class Shield : Weapon
{
    private float cooldownMultiplier = 1.0f;
    private bool isAttackCooldown = false;
    private bool isSkillCooldown = false;
    private CustomTickTimer attackTimer;
    private NetworkRigidbody2D _rb;

    public Shield(float attackSpeed)
    {
        this.attackSpeed = attackSpeed;
        range = 2.0f;
        damages = new List<float> { 100.0f, 200.0f, 150.0f };
    }

    public override IEnumerator Attack(Animator anim, NetworkMecanimAnimator mecanim, Transform character)
    {
        // abort if character is not grounded
        if (anim.GetBool("Grounded") == false)
        {
            yield break;
        }
        if (_rb == null)
        {
            _rb = character.gameObject.GetComponent<NetworkRigidbody2D>();
        }
        if (!isAttackCooldown)
        {
            // 0.5f = animation length
            // 0.3f = combo delay
            _rb.Rigidbody.velocity = Vector2.zero;
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
            playerAttack.attackType = PlayerAttack.AttackType.ProjectileOrShield;
            isAttackCooldown = true;
            // Cooldown Timer
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

    public override void ApplyRush(Transform character)
    {
        controller.StartCoroutine(Rush(character));
    }

    // Rush
    public IEnumerator Rush(Transform character)
    {
        NetworkRunner runner = NetworkRunner.Instances.First();
        CustomTickTimer timer;
        float rushSpeed = 0.0f;
        // check if rigidbody is null
        if (_rb == null)
        {
            _rb = controller.GetComponent<NetworkRigidbody2D>();
        }
        // abort if character is moving
        if (_rb.Rigidbody.velocity.x != 0)
        {
            yield break;
        }
        // different rush speed based on attack state
        switch (controller.AttackState)
        {
            case 1:
                rushSpeed = 0.3f;
                break;
            case 2:
                rushSpeed = 0.5f;
                break;
            case 3:
                rushSpeed = 0.7f;
                break;
        }
        // add force for 15 ticks
        timer = CustomTickTimer.CreateFromTicks(runner, 15);
        while (timer.Expired(runner) == false)
        {
            _rb.Rigidbody.velocity = Vector2.zero;
            if (character.localScale.x > 0)
            {
                _rb.Rigidbody.MovePosition(new Vector2(_rb.Rigidbody.position.x + rushSpeed, _rb.Rigidbody.position.y));
            }
            else
            {
                _rb.Rigidbody.MovePosition(new Vector2(_rb.Rigidbody.position.x - rushSpeed, _rb.Rigidbody.position.y));
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public override void ApplyJump()
    {
        controller.StartCoroutine(Jump());
    }

    // Jump
    public IEnumerator Jump()
    {
        NetworkRunner runner = NetworkRunner.Instances.First();
        CustomTickTimer timer;

        // check if rigidbody is null 
        if (_rb == null)
        {
            _rb = controller.GetComponent<NetworkRigidbody2D>();
        }

        // add force for 5 ticks
        timer = CustomTickTimer.CreateFromTicks(runner, 5);
        while (timer.Expired(runner) == false)
        {
            _rb.Rigidbody.velocity = Vector2.zero;
            _rb.Rigidbody.AddForce(new Vector2(0, 15), ForceMode2D.Impulse);
            yield return new WaitForFixedUpdate();
        }
    }

    public override IEnumerator Skill(Transform character)
    {
        // Toggle skill by calling ToggleSkill method from Aura script
        skillObject.GetComponent<Aura>().ToggleSkill();
        yield return new WaitForFixedUpdate();
    }

    public override IEnumerator DrawWeapon(Animator anim, NetworkMecanimAnimator mecanim, Transform character)
    {
        isdraw = true;
        yield return new WaitForSeconds(attackSpeed);
        isdraw = false;
    }

}