using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : CharacterClass
{

    public Knight()
    {
        maxHealth = 100.0f;
        attackSpeed = 2.0f;
        moveSpeed = 12f;
        weapon = new Katana(attackSpeed);
        characterAnimator = Resources.Load<RuntimeAnimatorController>("KnightAnimator");
        var roll = new SkillStruct
        {
            coolDown = 5.0f,
            coolDownTimer = default
        };
        SkillList.Add("Roll", roll);
    }

}