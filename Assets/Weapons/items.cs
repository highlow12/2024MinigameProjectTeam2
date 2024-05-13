using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public abstract class Weapon
    {
        public float attackSpeed;
        public float range;
        public int damage;
        public int defense;
        public int healingAmount;
        public float projectileSpeed;
        public virtual IEnumerator Attack()
        {
            Debug.Log("Attacking");
            yield return new WaitForSeconds(attackSpeed);
        }
    }
}