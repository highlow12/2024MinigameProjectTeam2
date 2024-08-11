using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Items
{
    public abstract class Weapon
    {
        public float damageMultiplier;
        public float attackSpeed;
        public int attackState;
        public float prevAttack;
        public float range;
        public float projectileSpeed;
        public float damage;
        public List<float> damages = new();
        public int defense;
        public int healingAmount;
        public GameObject rangeObject;
        public NetworkObject skillObject;
        public PlayerControllerNetworked controller;
        public bool isDraw = false;

        public virtual IEnumerator Attack(Animator anim, NetworkMecanimAnimator mecanim, Transform character)
        {
            Debug.Log("Attacking");
            yield return new WaitForSeconds(attackSpeed);
        }
        public virtual IEnumerator DrawWeapon(Animator anim, NetworkMecanimAnimator mecanim, Transform character)
        {
            Debug.Log("DrawWeapon");
            yield return new WaitForSeconds(attackSpeed);
        }

        public virtual void FireProjectileAlt(int state, Transform character)
        {
            Debug.Log("Firing projectile with alt method");
            return;
        }

        public virtual IEnumerator Skill(Transform character, float duration)
        {
            Debug.Log("Using skill");
            yield return new WaitForFixedUpdate();
        }

        public virtual IEnumerator Parry(Transform character)
        {
            Debug.Log("Parrying");
            yield return new WaitForFixedUpdate();
        }

        public virtual void ApplyRush(Transform character)
        {
            Debug.Log("Applying rush");
            return;
        }
        public virtual void ApplyJump()
        {
            Debug.Log("Applying jump");
            return;
        }
    }
}