using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterClass
{
    public float maxHealth;
    public float attackSpeed;
    public float moveSpeed;
    public Items.Weapon weapon;
    public enum CharacterClassEnum
    {
        Warrior,
        Archer,
        Tank
    }

    public static void ChangeClass(int characterClass, GameObject player)
    {
        switch (characterClass)
        {
            case (int)CharacterClassEnum.Warrior:
                var Warrior = new Warrior();
                ChangeStats(Warrior, characterClass, player);
                break;
            case (int)CharacterClassEnum.Archer:
                var archer = new Archer();
                ChangeStats(archer, characterClass, player);
                break;
            case (int)CharacterClassEnum.Tank:
                var tank = new Tank();
                ChangeStats(tank, characterClass, player);
                break;
        }
    }

    static void ChangeStats(CharacterClass classObj, int classEnum, GameObject player)
    {
        var controller = player.GetComponent<PlayerControllerSingle>();
        controller.moveSpeed = classObj.moveSpeed;
        controller.attackSpeed = classObj.attackSpeed;
        controller.maxHealth = classObj.maxHealth;
        controller.characterClass = classEnum;
        controller.weapon = classObj.weapon;
        controller.currentHealth = classObj.maxHealth;
    }
}
