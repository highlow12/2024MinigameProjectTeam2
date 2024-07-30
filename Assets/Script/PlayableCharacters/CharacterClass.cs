using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;

public enum CharacterClassEnum
{
    Knight,
    Archer,
    Tank,
}

public class ClassIcons
{
    static public List<Sprite> classIcons = new();
    static public Sprite GetIcon(CharacterClassEnum type)
    {
        if (classIcons.Count == 0)
        {
            classIcons = Resources.LoadAll<Sprite>("Icons/Classes").ToList();
        }

        return classIcons.Find(x => x.name == type.ToString());
    }
}

public abstract class CharacterClass
{
    public float maxHealth;
    public float attackSpeed;
    public float moveSpeed;
    public Weapon weapon;
    public RuntimeAnimatorController characterAnimator;
    public DynamicObjectProvider dynamicObjectProvider;
    public Dictionary<string, float> skillList = new(); // skillName, coolDown

    public static void ChangeClass(int characterClass, GameObject player)
    {
        switch (characterClass)
        {
            case (int)CharacterClassEnum.Knight:
                var Knight = new Knight();
                ChangeStats(Knight, characterClass, player);
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
        var controller = player.GetComponent<PlayerControllerNetworked>();
        if (controller.weapon != null && controller.weapon.rangeObject != null && controller.weapon.isRangeObjectSpawned)
        {
            Debug.Log(controller.weapon.rangeObject);
            GameObject.DestroyImmediate(controller.weapon.rangeObject, true);
        }
        controller.speed = classObj.moveSpeed;
        controller.attackSpeed = classObj.attackSpeed;
        controller.maxHealth = classObj.maxHealth;
        controller.characterClass = classEnum;
        controller.weapon = classObj.weapon;
        controller.weapon.dynamicObjectProvider = controller.dynamicObjectProvider;
        controller.currentHealth = classObj.maxHealth;
        controller.skillList = classObj.skillList;
        player.GetComponent<Animator>().runtimeAnimatorController = classObj.characterAnimator;
    }
}
