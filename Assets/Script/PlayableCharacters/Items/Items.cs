using System.Collections;
using UnityEngine;

namespace Items
{
    public abstract class Weapon
    {
        public float attackSpeed;
        public int attackState;
        public float prevAttack;
        public float range;
        public float projectileSpeed;
        public float damage;
        public int defense;
        public int healingAmount;
        public GameObject rangeObject;
        public DynamicObjectProvider dynamicObjectProvider;
        public bool isRangeObjectSpawned = false;

        public virtual IEnumerator Attack(Animator anim, Transform character)
        {
            Debug.Log("Attacking");
            yield return new WaitForSeconds(attackSpeed);
        }
        public virtual IEnumerator FireProjectile(Animator anim, Transform character)
        {
            Debug.Log("Firing projectile");
            yield return new WaitForSeconds(attackSpeed);
        }
    }
}