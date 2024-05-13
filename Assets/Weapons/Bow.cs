using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;

public class Bow : Items.Weapon
{
    public Bow()
    {
        attackSpeed = 1.0f;
        projectileSpeed = 10.0f;
        range = 20.0f;
        damage = 50;
    }

    public override IEnumerator Attack()
    {
        Debug.Log("Firing Arrow");
        yield return new WaitForSeconds(attackSpeed);
    }

}
