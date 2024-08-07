using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : CharacterClass
{
    public Tank()
    {
        maxHealth = 130.0f;
        attackSpeed = 1.6f;
        moveSpeed = 10f;
        weapon = new Shield(attackSpeed);
        characterAnimator = Resources.Load<RuntimeAnimatorController>("TankAnimator");
        skillList.Add("Parry", 5.25f);
        skillList.Add("Roll", 5.0f);
    }

}