using UnityEngine;
using Items;
using System.Collections;

public class Shield : Items.Weapon
{
    public Shield()
    {
        attackSpeed = 1.0f;
        range = 2.0f;
        defense = 100;
        healingAmount = 50;
    }

    public override IEnumerator Attack()
    {
        Debug.Log("Blocking");
        yield return new WaitForSeconds(attackSpeed);
    }
}