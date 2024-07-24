using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : CharacterClass
{

    public Knight()
    {
        maxHealth = 100.0f;
        attackSpeed = 1.0f;
        moveSpeed = 1.5f;
        weapon = new Katana();
        characterAnimator = Resources.Load<RuntimeAnimatorController>("KnightAnimator");
        skillList.Add("Parry", 5.25f);
        skillList.Add("Roll", 5.0f);
    }

}