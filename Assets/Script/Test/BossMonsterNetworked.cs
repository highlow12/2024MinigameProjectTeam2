using System.Collections;
using System.Collections.Generic;
using System;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;


public class BossMonsterNetworked : NetworkBehaviour
{
    // Enums
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
    public enum AttackType
    {
        Melee = 1 << 0,
        JumpDash = 1 << 1,
    }


    // Networked variables
    [Networked] public float CurrentHealth { get; set; }
    [Networked] public float BossScale { get; set; }
    [Networked] public NetworkObject FollowTarget { get; set; }
    [Networked] public BossState CurrentState { get; set; }
    [Networked] public Condition bossCondition { get; set; }
    [Networked] public AttackType attackType { get; set; }
    [Networked] public float conditionDuration { get; set; }
    [Networked] public float currentDistance { get; set; }
    [Networked] public float attackCooldown { get; set; }
    [Networked] public bool isAttacking { get; set; }
    [Networked] public bool isMoving { get; set; }


    // Local variables
    NetworkRigidbody2D _rb;
    Animator _animator;
    public readonly float maxHealth = 1000.0f;
    // public GameObject effectPool;
    public CameraMovement cameraMovement;
    public Image healthBar;
    public DurationIndicator durationIndicator;
    public Collider2D attackRange;
    public GameObject attackRangeIndicator;


    void Awake()
    {
        _rb = GetComponent<NetworkRigidbody2D>();
        _animator = GetComponent<Animator>();
        cameraMovement = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
        healthBar = GameObject.FindGameObjectWithTag("BossHealthUI").GetComponent<Image>();
        durationIndicator = GameObject.FindGameObjectWithTag("DurationUI").GetComponent<DurationIndicator>();
        attackRange = transform.GetChild(0).GetComponent<Collider2D>();
        attackRangeIndicator = transform.GetChild(1).gameObject;
    }

    void Start()
    {
        transform.localScale = new Vector3(BossScale, BossScale, 1);
        CurrentHealth = maxHealth;
        CurrentState = BossState.Idle;
        StartCoroutine(SetTargetRecursive());
    }

    void Update()
    {
        // Debug.Log($"{Runner.TicksExecuted} ticks executed");
        // Debug.Log($"{Runner.TickRate} ticks per second");
        // Debug.Log($"{Runner.LatestServerTick} latest server tick");
        // Debug.Log($"{Time.fixedDeltaTime} fixed delta time");
        // Debug.Log($"{Time.deltaTime} delta time");
        // Debug.Log($"{Runner.DeltaTime} Runner delta time");
    }

    // Networked boss behaviour
    public override void FixedUpdateNetwork()
    {
        BossBehaviour();
    }
    // Networked animation
    public override void Render()
    {
        base.Render();
    }

    // Player targeting coroutine
    IEnumerator SetTargetRecursive()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            foreach (var player in players)
            {
                if (FollowTarget == null)
                {
                    FollowTarget = player.GetComponent<NetworkObject>();
                }
                else
                {
                    if (Vector2.Distance(transform.position, player.transform.position) < Vector2.Distance(transform.position, FollowTarget.transform.position))
                    {
                        FollowTarget = player.GetComponent<NetworkObject>();
                    }
                }
            }
        }
        StartCoroutine(SetTargetRecursive());
    }

    // Behaviour coroutines
    public IEnumerator Move()
    {
        if (FollowTarget == null)
        {
            yield return new WaitForSecondsRealtime(1.0f);
            StartCoroutine(Move());
        }
        float distance = transform.position.x - FollowTarget.transform.position.x;
        Vector3 scale = transform.localScale;
        if (distance > 0)
        {
            scale.x = BossScale;
            transform.localScale = scale;
        }
        else
        {
            scale.x = -BossScale;
            transform.localScale = scale;
        }
        _animator.SetInteger("walkState", 1);
        while (Math.Abs(distance) > 3.0f)
        {
            Vector3 targetPos = FollowTarget.transform.position;
            targetPos.y = transform.position.y;
            transform.position = Vector2.Lerp(transform.position, targetPos, 0.01f);

            distance = transform.position.x - FollowTarget.transform.position.x;
            // wait for the next tick
            yield return new WaitForSecondsRealtime(1.0f / 64);
        }
        _animator.SetInteger("walkState", 0);
        yield return null;
    }
    public IEnumerator Attack()
    {
        _animator.SetTrigger("doAttack");
        attackRangeIndicator.SetActive(true);
        float omenLength = 0.4f;
        float attackLength = 0.3f;
        durationIndicator.CreateDurationIndicator(omenLength, "OmenDuration");
        yield return new WaitForSecondsRealtime(omenLength);
        attackRange.enabled = true;
        yield return new WaitForSecondsRealtime(attackLength);
        attackRange.enabled = false;
        attackRangeIndicator.SetActive(false);
        yield return null;
    }

    public IEnumerator JumpDashAttack()
    {
        float jumpVelocity = 15.0f;
        float dashVelocity = 10.0f;
        float jumpTime = 0.8f;
        float dashTime = 0.5f;
        cameraMovement.isBossJumping = true;
        _rb.Rigidbody.velocity = Vector2.zero;
        Vector2 targetPos = FollowTarget.transform.position;
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        _rb.Rigidbody.velocity = new Vector2(0, jumpVelocity);
        yield return new WaitForSecondsRealtime(jumpTime);
        _rb.Rigidbody.velocity = direction * dashVelocity;
        yield return new WaitForSecondsRealtime(dashTime);
        cameraMovement.isBossJumping = false;
        yield return null;
    }


    // Coroutine callers
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
    // Behaviour Tree
    void BossBehaviour()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.fixedDeltaTime;
        }
        if (bossCondition.HasFlag(Condition.RequireDurationUpdate))
        {
            conditionDuration = 0.0f;
            bossCondition &= ~Condition.RequireDurationUpdate;
        }
        if (bossCondition.HasFlag(Condition.IsPlayerInFar))
        {
            conditionDuration += Time.fixedDeltaTime;
            if (conditionDuration >= 5.0f)
            {
                bossCondition |= Condition.RequireDurationUpdate;
                attackType = AttackType.JumpDash;
                CurrentState = BossState.Attack;
            }
        }
        else
        {
            conditionDuration = 0.0f;
        }
        UpdateAttribute();
        UpdateCondition();
        switch (CurrentState)
        {
            case BossState.Idle:
                CurrentState = BossState.Move;
                break;
            case BossState.Move:
                if (isAttacking || isMoving)
                {
                    return;
                }
                isMoving = true;
                StartCoroutine(MoveController(Move()));
                if (bossCondition.HasFlag(Condition.IsPlayerInAttackRange))
                {
                    CurrentState = BossState.Attack;
                }
                break;
            case BossState.Attack:
                if (attackCooldown > 0)
                {
                    return;
                }
                attackCooldown = Random.Range(2.0f, 3.5f);
                isAttacking = true;
                switch (attackType)
                {
                    case AttackType.Melee:
                        if (!bossCondition.HasFlag(Condition.IsPlayerInAttackRange))
                        {
                            attackCooldown = 0;
                            CurrentState = BossState.Move;
                            isAttacking = false;
                            return;
                        }

                        StartCoroutine(AttackController(Attack()));
                        Debug.Log("Do Melee Attack");
                        break;
                    case AttackType.JumpDash:
                        StartCoroutine(AttackController(JumpDashAttack()));
                        Debug.Log("Do Jump Dash Attack");
                        break;
                }
                CurrentState = BossState.Idle;
                break;
            case BossState.Hit:
                break;
            case BossState.Die:
                break;
        }
    }

    private float GetDistance(Vector3 target)
    {
        return Vector2.Distance(transform.position, target);
    }

    private void UpdateAttribute()
    {
        healthBar.fillAmount = CurrentHealth / maxHealth;
    }
    private void UpdateCondition()
    {
        if (!FollowTarget)
        {
            return;
        }
        currentDistance = GetDistance(FollowTarget.transform.position);
        if (CurrentState.HasFlag(BossState.Attack) && bossCondition.HasFlag(Condition.IsAttackTypeNull))
        {
            bossCondition &= ~Condition.IsAttackTypeNull;

            AttackType[] values = (AttackType[])Enum.GetValues(typeof(AttackType));
            attackType = values[Random.Range(0, values.Length)];
            return;
        }
        bossCondition &= ~Condition.IsPlayerInNear;
        bossCondition &= ~Condition.IsPlayerInFar;
        bossCondition &= ~Condition.IsPlayerInAttackRange;
        switch (currentDistance)
        {
            case float distance when distance < 3.0f:
                bossCondition |= Condition.IsPlayerInAttackRange;
                attackType = AttackType.Melee;
                break;
            case float distance when distance <= 10.0f:
                bossCondition |= Condition.IsPlayerInNear;
                break;
            case float distance when distance > 10.0f:
                bossCondition |= Condition.IsPlayerInFar;
                break;

        }

    }
    // TriggerEvent for player attack
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("CharacterProjectile"))
        {
            Base projectile = other.transform.parent.gameObject.GetComponent<Base>();
            CurrentHealth -= projectile.damage;
            projectile.ReleaseObject();
        }
    }

}
