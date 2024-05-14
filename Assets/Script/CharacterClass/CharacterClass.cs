using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;

public abstract class CharacterClass
{
    public float maxHealth;
    public float attackSpeed;
    public float moveSpeed;
    public Weapon weapon;
    public RuntimeAnimatorController characterAnimator;
    public enum CharacterClassEnum
    {
        Warrior,
        Archer,
        Tank,
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
        if (controller.weapon != null && controller.weapon.rangeObject != null && controller.weapon.isRangeObjectSpawned)
        {
            Debug.Log(controller.weapon.rangeObject);
            GameObject.DestroyImmediate(controller.weapon.rangeObject, true);
        }
        controller.moveSpeed = classObj.moveSpeed;
        controller.attackSpeed = classObj.attackSpeed;
        controller.maxHealth = classObj.maxHealth;
        controller.characterClass = classEnum;
        controller.weapon = classObj.weapon;
        controller.currentHealth = classObj.maxHealth;
        player.GetComponent<Animator>().runtimeAnimatorController = classObj.characterAnimator;
    }
}
