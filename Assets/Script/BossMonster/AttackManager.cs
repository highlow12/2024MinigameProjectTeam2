using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public static AttackManager Instance;
    public List<BossSkill> BossAttacks = new();

    void Awake()
    {
        Instance = this;
        BossAttacks.Add(new BaseAttack1());
        BossAttacks.Add(new BackAttack());
        BossAttacks.Add(new BothAttack());
        BossAttacks.Add(new EnergyAttack());
        BossAttacks.Add(new JumpAttack());
        BossAttacks.Add(new Rush1());
        //phase2 skills
        BossAttacks.Add(new SpawnFireSpirit());
        BossAttacks.Add(new SpawnBindSword());
        BossAttacks.Add(new BaseAttack2());
        BossAttacks.Add(new SpawnHorizontalBindSword());

    }



}

public abstract class BossSkill
{
    public string name;
    public int phase;
    public abstract IEnumerator Attack(Transform transform, /* DO NOT USE animator parameter!!! It will be removed*/ Animator animator, NetworkRunner runner, BossAttack bossAttack = null, NetworkObject boss = null);
    public float attackDamage;
    public GameObject projectile;
}
