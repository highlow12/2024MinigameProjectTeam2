using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;

public class Katana : Items.Weapon
{
    public Katana()
    {
        attackSpeed = 2.0f;
        range = 2.0f;
        damage = 60;
    }

    public override IEnumerator Attack()
    {
        Debug.Log("Swinging Katana");
        yield return new WaitForSeconds(attackSpeed);
    }
}
