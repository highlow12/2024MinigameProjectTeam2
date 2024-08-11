using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion;

public class Archer : CharacterClass
{
    public Archer()
    {
        maxHealth = 800.0f;
        attackSpeed = 1.3f;
        moveSpeed = 9f;
        weapon = new Bow(attackSpeed);
        characterAnimator = Resources.Load<RuntimeAnimatorController>("ArcherAnimator");
        var roll = new SkillStruct
        {
            coolDown = 0.0f,
            coolDownTimer = default
        };
        SkillList.Add("Roll", roll);
    }

}