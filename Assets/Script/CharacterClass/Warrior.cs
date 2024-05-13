using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Katana;

public class Warrior : CharacterClass
{
    public Warrior()
    {
        maxHealth = 100.0f;
        attackSpeed = 1.0f;
        moveSpeed = 5.0f;
        weapon = new Katana();
    }

}