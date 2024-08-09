using System.Collections;
using System.Collections.Generic;
using System;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using TMPro;


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
    [Networked] public CustomTickTimer BossAttackTimer { get; set; }
    [Networked, OnChangedRender(nameof(UpdateHealthBarCallback))] public float CurrentHealth { get; set; }
    [Networked] public float BossScale { get; set; }
    [Networked] public float BossSpeed { get; set; }
    [Networked] public NetworkObject FollowTarget { get; set; }
    [Networked] public BossState CurrentState { get; set; }
    [Networked] public Condition bossCondition { get; set; }
    [Networked] public AttackType attackType { get; set; }
    [Networked] public float conditionDuration { get; set; }
    [Networked] public float currentDistance { get; set; }
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
    public BossAttack bossAttack;
    public TextMeshProUGUI bossHealthText;
    public List<BossHitFeedbackEffect> BossHitFeedbackEffects = new();
    public NetworkObject bossSwordEffect;
    void Awake()
    {
        _rb = GetComponent<NetworkRigidbody2D>();
        _animator = GetComponent<Animator>();
        cameraMovement = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
        healthBar = GameObject.FindGameObjectWithTag("BossHealthUI").GetComponent<Image>();
        durationIndicator = GameObject.FindGameObjectWithTag("DurationUI").GetComponent<DurationIndicator>();
        bossHealthText = GameObject.FindGameObjectWithTag("BossHealthText").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        CurrentHealth = maxHealth;
        CurrentState = BossState.Idle;
        var objects = GameObject.FindGameObjectsWithTag("BossHitFeedbackEffect");
        Debug.Log(objects.Length);
        for (int i = 0; i < 3; i++)
        {
            Debug.Log($"Set {i}th object");
            BossHitFeedbackEffects.Add(objects[i].GetComponent<BossHitFeedbackEffect>());
        }
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
        transform.localScale = new Vector3(BossScale, Math.Abs(BossScale), 1);
        // Debug.Log(BossAttackTimer.ElapsedTicks(Runner));
        // Debug.Log($"Timer expired: {BossAttackTimer.Expired(Runner)}");
        // Debug.Log($"Timer is default: {Equals(BossAttackTimer, default(CustomTickTimer))}");
        if (HasStateAuthority)
        {

            BossBehaviour();
        }
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
        else
        {
            float distance = transform.position.x - FollowTarget.transform.position.x;
            // BossScale is used to flip the boss sprite in FixedUpdateNetwork
            if (distance > 0)
            {
                BossScale = Mathf.Abs(BossScale);
            }
            else
            {
                BossScale = -Mathf.Abs(BossScale);
            }
            _animator.SetInteger("walkState", 1);
            // Move the boss to the player until the distance is less than 2.5f
            while (Math.Abs(distance) > 2.5f)
            {
                // Vector3 targetPos = FollowTarget.transform.position;
                // targetPos.y = transform.position.y;
                // transform.position = Vector2.Lerp(transform.position, targetPos, 0.025f);
                if (distance > 0)
                {
                    _rb.Rigidbody.velocity = new Vector2(-1 * BossSpeed, _rb.Rigidbody.velocity.y);
                }
                else
                {
                    _rb.Rigidbody.velocity = new Vector2(BossSpeed, _rb.Rigidbody.velocity.y);
                }
                distance = transform.position.x - FollowTarget.transform.position.x;
                // wait for the next tick
                yield return new WaitForFixedUpdate();
            }
            _rb.Rigidbody.velocity = Vector2.zero;
            _animator.SetInteger("walkState", 0);
            yield return null;
        }
    }
    public IEnumerator Attack()
    {
        _animator.SetTrigger("doAttack");
        float attackLength = 1.2f;
        // attack logic by animation event required
        bossAttack.playersHit = new List<PlayerRef>();
        bossAttack.damage = 10.0f;
        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(Runner, attackLength);
        while (!attackLengthTimer.Expired(Runner))
        {
            yield return new WaitForFixedUpdate();
        }
        bossAttack.damage = 0.0f;
        yield return null;
    }
    public IEnumerator bothAttack()
    {
        _animator.SetTrigger("doAttack");
        float attackLength = 1.2f;
        // attack logic by animation event required
        bossAttack.playersHit = new List<PlayerRef>();
        bossAttack.damage = 10.0f;
        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(Runner, attackLength);
        while (!attackLengthTimer.Expired(Runner))
        {
            yield return new WaitForFixedUpdate();
        }

        BossScale *= -1;
        _animator.SetTrigger("doAttack2");

        // attack logic by animation event required
        bossAttack.playersHit = new List<PlayerRef>();

        attackLengthTimer = CustomTickTimer.CreateFromSeconds(Runner, attackLength);
        while (!attackLengthTimer.Expired(Runner))
        {
            yield return new WaitForFixedUpdate();
        }
        bossAttack.damage = 0.0f;
        BossScale *= -1;
        yield return null;
    }
    public IEnumerator backAttack()
    {
        BossScale *= -1;
        _animator.SetTrigger("doAttack2");
        float attackLength = 1.2f;
        // attack logic by animation event required
        bossAttack.playersHit = new List<PlayerRef>();
        bossAttack.damage = 10.0f;
        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(Runner, attackLength);
        while (!attackLengthTimer.Expired(Runner))
        {
            yield return new WaitForFixedUpdate();
        }
        bossAttack.damage = 0.0f;
        BossScale *= -1;
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
        // If attack timer is expired, set it as default
        // If attack timer is default, boss attack can be triggered
        // RequireDurationUpdate is set when the condition requires duration update
        // Example: If player is in far for 5 seconds, set attack type as JumpDash
        // And force set the state as Attack
        if (BossAttackTimer.Expired(Runner))
        {
            Debug.Log("Set timer as default");
            BossAttackTimer = default;
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
        if (!bossCondition.HasFlag(Condition.IsPlayerInAttackRange) && attackType != AttackType.JumpDash && CurrentState != BossState.Move)
        {
            CurrentState = BossState.Move;
        }
        if (bossCondition.HasFlag(Condition.IsPlayerInAttackRange) && CurrentState != BossState.Attack)
        {
            CurrentState = BossState.Attack;
        }
        // If attack timer is default and boss is in attack state,
        // not attacking, and set new attack timer
        if (Equals(BossAttackTimer, default(CustomTickTimer)))
        {
            if (CurrentState == BossState.Attack && isAttacking == true)
            {
                Debug.Log("Set timer");
                BossAttackTimer = CustomTickTimer.CreateFromSeconds(Runner, Random.Range(2.0f, 3.5f));
            }
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
                // Check if attack timer is default
                if (!Equals(BossAttackTimer, default(CustomTickTimer)))
                {
                    return;
                }
                isAttacking = true;
                switch (attackType)
                {
                    // Attack controller makes sure that the attack is executed once
                    // If end of the attack is reached, isAttacking is set as false
                    case AttackType.Melee:
                        // If player is not in attack range, cancel the attack
                        // and set state as Move
                        if (!bossCondition.HasFlag(Condition.IsPlayerInAttackRange))
                        {
                            Debug.Log("Player is not in attack range");
                            CurrentState = BossState.Move;
                            isAttacking = false;
                            return;
                        }

                        StartCoroutine(AttackController(bothAttack()));
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
        // healthBar.fillAmount = CurrentHealth / maxHealth;
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

    // RPCs
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_OnBossHit(PlayerAttack.AttackData attack)
    {
        CurrentHealth -= attack.damage;
        // call effect
        var oldestEffect = BossHitFeedbackEffects[0];
        foreach (var effect in BossHitFeedbackEffects)
        {
            if (effect.CallTime <= oldestEffect.CallTime)
            {
                oldestEffect = effect;
            }
            if (effect.IsCallable || (BossHitFeedbackEffects.IndexOf(effect) == BossHitFeedbackEffects.Count - 1))
            {
                oldestEffect.AttackType = (int)attack.attackType;
                oldestEffect.EffectType = Random.Range(1, 4);
                oldestEffect.IsCallable = false;
                oldestEffect.CallTime = Time.time;
                oldestEffect.CallPositon = attack.hitPosition;
                break;
            }

        }
        oldestEffect.PlayEffect();
        if (Runner.IsSceneAuthority && CurrentHealth <= 0)
        {
            Debug.Log("dead");
            // bossDead();
        }

    }

    public void UpdateHealthBarCallback()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = CurrentHealth / maxHealth;
            bossHealthText.text = $"{CurrentHealth} / {maxHealth}";
        }
    }


    public void bossDead()
    {
        Runner.LoadScene(SceneRef.FromIndex(0));
    }
    // TriggerEvent for player attack
    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.gameObject.CompareTag("CharacterProjectile"))
    //     {
    //         Base projectile = other.transform.parent.gameObject.GetComponent<Base>();
    //         CurrentHealth -= projectile.damage;
    //         projectile.ReleaseObject();
    //     }
    //     else if (other.gameObject.CompareTag("MeleeAttack"))
    //     {
    //         Attack melee = other.gameObject.GetComponent<Attack>();
    //         CurrentHealth -= melee.damage;
    //     }
    // }

}
