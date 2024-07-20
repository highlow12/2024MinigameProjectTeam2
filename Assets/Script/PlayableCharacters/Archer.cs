using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : CharacterClass
{
    public Archer()
    {
        maxHealth = 100.0f;
        attackSpeed = 1.0f;
        moveSpeed = 2.3f;
        weapon = new Bow(attackSpeed);
        characterAnimator = Resources.Load<RuntimeAnimatorController>("ArcherAnimator");
        skillList.Add("Parry", 5.25f);
        skillList.Add("Roll", 5.0f);
    }

}