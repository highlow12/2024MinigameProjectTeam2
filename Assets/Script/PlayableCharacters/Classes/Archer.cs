using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion;

public class Archer : CharacterClass
{
    public Archer()
    {
        maxHealth = 100.0f;
        attackSpeed = 1.3f;
        moveSpeed = 15f;
        weapon = new Bow(attackSpeed);
        characterAnimator = Resources.Load<RuntimeAnimatorController>("ArcherAnimator");
        var roll = new SkillStruct
        {
            coolDown = 5.0f,
            coolDownTimer = default
        };
        SkillList.Add("Roll", roll);
    }

}