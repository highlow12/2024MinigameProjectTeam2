using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : CharacterClass
{
    public Tank()
    {
        maxHealth = 130.0f;
        attackSpeed = 0.8f;
        moveSpeed = 4.0f;
        weapon = new Shield();
        characterAnimator = Resources.Load<RuntimeAnimatorController>("Warrior");

    }

}