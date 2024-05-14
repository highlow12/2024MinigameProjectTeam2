using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : CharacterClass
{
    public Archer()
    {
        maxHealth = 100.0f;
        attackSpeed = 1.5f;
        moveSpeed = 8.0f;
        weapon = new Bow();
        characterAnimator = Resources.Load<RuntimeAnimatorController>("Warrior");

    }

}