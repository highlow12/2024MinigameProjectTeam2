using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class BossBehaviourTree : MonoBehaviour
{
    public BossState currentState;
    public Condition bossCondition;
    private TestBossMonsterSingle mainScript;
    public float conditionDuration;
    public float currentDistance;
    public float attackCooldown;
    public bool isAttacking;
    public bool isMoving;
    [Flags]
    public enum BossState
    {
        Idle = 1 << 0,
        Move = 1 << 1,
        Attack = 1 << 2,
        Hit = 1 << 3,
        Die = 1 << 4
    }

    [Flags]
    public enum Condition
    {
        IsPlayerInNear = 1 << 0,
        IsPlayerInFar = 1 << 1,
        IsPlayerInAttackRange = 1 << 2,
        IsAttackTypeNull = 1 << 3,
        RequireDurationUpdate = 1 << 4
    }
    [Flags]
    enum AttackType
    {
        Melee = 1 << 0,
        JumpDash = 1 << 1,
    }

    public Dictionary<string, object> bossAttribute = new();

    void Start()
    {
        currentState = BossState.Idle;
    }
    void Awake()
    {
        mainScript = GetComponent<TestBossMonsterSingle>();
        bossAttribute.Add("maxHealth", mainScript.maxHealth);
        bossAttribute.Add("currentHealth", mainScript.currentHealth);
        bossAttribute.Add("bossScale", mainScript.bossScale);
        bossAttribute.Add("followTarget", mainScript.followTarget);
        bossAttribute.Add("attackType", AttackType.Melee);
    }
    void FixedUpdate()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        if (bossCondition.HasFlag(Condition.RequireDurationUpdate))
        {
            conditionDuration = 0.0f;
            bossCondition &= ~Condition.RequireDurationUpdate;
        }
        if (bossCondition.HasFlag(Condition.IsPlayerInFar))
        {
            conditionDuration += Time.deltaTime;
            if (conditionDuration >= 5.0f)
            {
                bossCondition |= Condition.RequireDurationUpdate;
                bossAttribute["attackType"] = AttackType.JumpDash;
                currentState = BossState.Attack;
            }
        }
        else
        {
            conditionDuration = 0.0f;
        }
    }
    private void SyncAttribute()
    {
        bossAttribute["maxHealth"] = mainScript.maxHealth;
        bossAttribute["currentHealth"] = mainScript.currentHealth;
        bossAttribute["bossScale"] = mainScript.bossScale;
        bossAttribute["followTarget"] = mainScript.followTarget;
    }

    private float GetDistance(Vector3 target)
    {
        return Vector2.Distance(transform.position, target);
    }

    private void UpdateAttribute()
    {
        mainScript.healthBar.fillAmount = mainScript.currentHealth / mainScript.maxHealth;
    }
    private void UpdateCondition()
    {
        if (bossAttribute.TryGetValue("followTarget", out var followTargetObj) && followTargetObj is GameObject followTarget)
        {
            currentDistance = GetDistance(followTarget.transform.position);
            if (currentState.HasFlag(BossState.Attack) && bossCondition.HasFlag(Condition.IsAttackTypeNull))
            {
                bossCondition &= ~Condition.IsAttackTypeNull;

                AttackType[] values = (AttackType[])Enum.GetValues(typeof(AttackType));
                bossAttribute["attackType"] = values[Random.Range(0, values.Length)];
                return;
            }
            bossCondition &= ~Condition.IsPlayerInNear;
            bossCondition &= ~Condition.IsPlayerInFar;
            bossCondition &= ~Condition.IsPlayerInAttackRange;
            switch (currentDistance)
            {
                case float distance when distance < 3.0f:
                    bossCondition |= Condition.IsPlayerInAttackRange;
                    bossAttribute["attackType"] = AttackType.Melee;
                    break;
                case float distance when distance <= 10.0f:
                    bossCondition |= Condition.IsPlayerInNear;
                    break;
                case float distance when distance > 10.0f:
                    bossCondition |= Condition.IsPlayerInFar;
                    break;

            }
        }
    }

    IEnumerator AttackController(IEnumerator attack)
    {
        yield return attack;
        isAttacking = false;
        yield return null;
    }

    IEnumerator MoveController(IEnumerator move)
    {
        yield return move;
        isMoving = false;
        yield return null;
    }


    void Update()
    {
        SyncAttribute();
        UpdateAttribute();
        UpdateCondition();
        switch (currentState)
        {
            case BossState.Idle:
                currentState = BossState.Move;
                break;
            case BossState.Move:
                if (isAttacking || isMoving)
                {
                    return;
                }
                isMoving = true;
                StartCoroutine(MoveController(mainScript.Move()));
                if (bossCondition.HasFlag(Condition.IsPlayerInAttackRange))
                {
                    currentState = BossState.Attack;
                }
                break;
            case BossState.Attack:
                if (attackCooldown > 0)
                {
                    return;
                }
                attackCooldown = Random.Range(2.0f, 3.5f);
                isAttacking = true;
                switch (bossAttribute["attackType"])
                {
                    case AttackType.Melee:
                        if (!bossCondition.HasFlag(Condition.IsPlayerInAttackRange))
                        {
                            attackCooldown = 0;
                            currentState = BossState.Move;
                            isAttacking = false;
                            return;
                        }

                        StartCoroutine(AttackController(mainScript.Attack()));
                        Debug.Log("Do Melee Attack");
                        break;
                    case AttackType.JumpDash:
                        StartCoroutine(AttackController(mainScript.JumpDashAttack()));
                        Debug.Log("Do Jump Dash Attack");
                        break;
                }
                currentState = BossState.Idle;
                break;
            case BossState.Hit:
                break;
            case BossState.Die:
                break;
        }

    }
}
