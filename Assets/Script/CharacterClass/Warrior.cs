using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : CharacterClass
{

    public Warrior()
    {
        maxHealth = 100.0f;
        attackSpeed = 1.0f;
        moveSpeed = 5.0f;
        weapon = new Katana();
        characterAnimator = Resources.Load<RuntimeAnimatorController>("Warrior");
        skillList.Add("Parry", 5.25f);
        skillList.Add("Roll", 5.0f);
    }

}