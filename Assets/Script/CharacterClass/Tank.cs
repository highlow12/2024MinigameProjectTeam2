using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : CharacterClass
{
    public Tank()
    {
        maxHealth = 130.0f;
        attackSpeed = 0.8f;
        moveSpeed = 1.3f;
        weapon = new Shield();
        characterAnimator = Resources.Load<RuntimeAnimatorController>("Warrior");
        skillList.Add("Parry", 5.25f);
        skillList.Add("Roll", 5.0f);
    }

}