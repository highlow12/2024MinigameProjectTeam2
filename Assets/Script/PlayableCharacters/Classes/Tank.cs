using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : CharacterClass
{
    public Tank()
    {
        maxHealth = 1200.0f;
        attackSpeed = 1.6f;
        moveSpeed = 10f;
        weapon = new Shield(attackSpeed);
        characterAnimator = Resources.Load<RuntimeAnimatorController>("TankAnimator");
        var roll = new SkillStruct
        {
            coolDown = 5.0f,
            coolDownTimer = default
        };
        var aura = new SkillStruct
        {
            coolDown = 30.0f,
            coolDownTimer = default,
            duration = 10.0f
        };
        SkillList.Add("Roll", roll);
        SkillList.Add("Aura", aura);
    }

}