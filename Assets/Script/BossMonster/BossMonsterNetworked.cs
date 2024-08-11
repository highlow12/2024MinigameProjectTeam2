using System.Collections;
using System.Collections.Generic;
using System;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;
using UnityEngine.SceneManagement;


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
        ChargeAttack = 1 << 2,
    }


    // Networked variables
    [Networked] public bool IsDead { get; set; }
    [Networked] public CustomTickTimer BossAttackTimer { get; set; }
    [Networked, OnChangedRender(nameof(UpdateHealthBarCallback))] public float CurrentHealth { get; set; }
    [Networked] public float BossScale { get; set; }
    [Networked] public float BossSpeed { get; set; } = 5.0f;
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
    public readonly float maxHealth = 50000.0f;
    // public GameObject effectPool;
    public CameraMovement cameraMovement;
    public Image healthBar;
    public DurationIndicator durationIndicator;
    public BossAttack bossAttack;
    public TextMeshProUGUI bossHealthText;
    public List<BossHitFeedbackEffect> BossHitFeedbackEffects = new();
    public NetworkObject bossSwordEffect;

    public AudioClip[] audioClips;
    private AudioSource Audio;
    void Awake()
    {
        _rb = GetComponent<NetworkRigidbody2D>();
        _animator = GetComponent<Animator>();
        cameraMovement = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
        healthBar = GameObject.FindGameObjectWithTag("BossHealthUI").GetComponent<Image>();
        durationIndicator = GameObject.FindGameObjectWithTag("DurationUI").GetComponent<DurationIndicator>();
        bossHealthText = GameObject.FindGameObjectWithTag("BossHealthText").GetComponent<TextMeshProUGUI>();
        Audio = GetComponent<AudioSource>();
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
            if (Runner.SessionInfo.IsOpen == true)
            {
                return;
            }
            if (IsDead)
            {
                CurrentState = BossState.Die;
                RPC_Mission_Accomplished();
            }
            if (CurrentState != BossState.Die)
            {
                BossBehaviour();
            }
        }
        if (!BGMmanager.instance.isBossBGM())
        {
            BGMmanager.instance.playBossBGM();
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
        if (FollowTarget != null)
        {
            CustomTickTimer timer = CustomTickTimer.CreateFromSeconds(Runner, 5.0f);
            while (!timer.Expired(Runner))
            {
                yield return new WaitForFixedUpdate();
            }
        }
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
            while (Math.Abs(distance) > 2.5f && isMoving)
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
        bossAttack.damage = 50.0f;
        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(Runner, attackLength);
        while (!attackLengthTimer.Expired(Runner))
        {
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
    public IEnumerator bothAttack()
    {
        _animator.SetTrigger("doAttack");
        float attackLength = 1.2f;
        // attack logic by animation event required
        bossAttack.playersHit = new List<PlayerRef>();
        bossAttack.damage = 50.0f;
        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(Runner, attackLength);
        while (!attackLengthTimer.Expired(Runner))
        {
            yield return new WaitForFixedUpdate();
        }

        BossScale *= -1;
        _animator.SetTrigger("doAttack2");
        attackLength = 1f;
        // attack logic by animation event required
        bossAttack.playersHit = new List<PlayerRef>();

        attackLengthTimer = CustomTickTimer.CreateFromSeconds(Runner, attackLength);
        while (!attackLengthTimer.Expired(Runner))
        {
            yield return new WaitForFixedUpdate();
        }
        BossScale *= -1;
        yield return null;
    }
    public IEnumerator backAttack()
    {
        BossScale *= -1;
        _animator.SetTrigger("doAttack2");
        float attackLength = 1f;
        // attack logic by animation event required
        bossAttack.playersHit = new List<PlayerRef>();
        bossAttack.damage = 50.0f;
        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(Runner, attackLength);
        while (!attackLengthTimer.Expired(Runner))
        {
            yield return new WaitForFixedUpdate();
        }
        BossScale *= -1;
        yield return null;
    }
    public IEnumerator AttackWithEnergy()
    {
        _animator.SetTrigger("doAttack");
        float attackLength = 1.2f;
        Vector3 yOffset = ((Vector3)Vector2.up * 2);
        Vector3 xOffset = BossScale > 0 ? Vector3.left : Vector3.right;
        // attack logic by animation event required
        Runner.Spawn(bossSwordEffect,
            transform.position + yOffset + xOffset, quaternion.Euler(0, 0, 0),
            Object.InputAuthority, (runner, o) =>
            {
                // Initialize the Ball before synchronizing it
                var script = o.GetComponent<BossSwordEnergy>();
                script.Init();
                script.damage = 100;
            }
        );
        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(Runner, attackLength);
        while (!attackLengthTimer.Expired(Runner))
        {
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    public IEnumerator JumpAttack()
    {
        _animator.SetTrigger("doJumpAttack");

        float attackLength = 2.2f;

        // attack logic by animation event required
        bossAttack.playersHit = new List<PlayerRef>();
        bossAttack.damage = 200.0f;

        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(Runner, attackLength);
        while (!attackLengthTimer.Expired(Runner))
        {
            //if(_animator.)
            yield return new WaitForFixedUpdate();
        }
        bossAttack.damage = 0.0f;
        yield return null;
    }
    public void jumpTP()
    {

        StartCoroutine(IjumpTP());
    }
    public IEnumerator IjumpTP()
    {
        for (int i = 0; i < 3; i++)
        {
            _rb.Teleport(new Vector2(FollowTarget.transform.position.x, transform.position.y));
            yield return new WaitForFixedUpdate();
        }
    }
    public IEnumerator chargeAttack()
    {
        _animator.SetTrigger("doChargeAttack");

        float attackLength = 2.2f;

        // attack logic by animation event required
        bossAttack.playersHit = new List<PlayerRef>();
        bossAttack.damage = 10.0f;

        var attackLengthTimer = CustomTickTimer.CreateFromSeconds(Runner, attackLength);
        while (!attackLengthTimer.Expired(Runner))
        {
            //if(_animator.)
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
    /*
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
    */

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
            if (conditionDuration >= 3.0f)
            {
                attackType = AttackType.JumpDash;
                CurrentState = BossState.Attack;
                bossCondition |= Condition.RequireDurationUpdate;

                isMoving = false;
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
                        int attacktype = Random.Range(0, 4);

                        switch (attacktype)
                        {
                            case (0):
                                StartCoroutine(AttackController(Attack()));
                                Debug.Log("Do Melee Attack");
                                break;

                            case (1):
                                StartCoroutine(AttackController(backAttack()));
                                Debug.Log("Do Melee Attack2");
                                break;

                            case (2):
                                StartCoroutine(AttackController(bothAttack()));
                                Debug.Log("Do bothAttack");
                                break;

                            case (3):
                                StartCoroutine(AttackController(AttackWithEnergy()));
                                Debug.Log("Do AttackWithEnergy");
                                break;
                        }

                        StartCoroutine(AttackController(Attack()));
                        Debug.Log("Do Melee Attack");
                        break;
                    case AttackType.JumpDash:
                        int attacktype1 = Random.Range(0, 1);
                        switch (attacktype1)
                        {
                            case (0):
                                StartCoroutine(AttackController(JumpAttack()));
                                Debug.Log("Do Jump Dash Attack");
                                break;

                            case (1):
                                StartCoroutine(AttackController(chargeAttack()));
                                Debug.Log("Do chargeAttack");
                                break;
                        }
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
        //play sound
        switch (attack.attackType)
        {
            case PlayerAttack.AttackType.Katana:

                //Audio.PlayOneShot(audioClips[0]);
                SFXManager.instance.playSFX(audioClips[0]);
                break;
            case PlayerAttack.AttackType.ProjectileOrShield:
                //Audio.PlayOneShot(audioClips[1]);
                SFXManager.instance.playSFX(audioClips[1]);
                break;
        }
        RPC_PlaySond(attack.attackType);
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
            BossDead();
        }

    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlaySond(PlayerAttack.AttackType a)
    {
        switch (a)
        {
            case PlayerAttack.AttackType.Katana:

                //Audio.PlayOneShot(audioClips[0]);
                SFXManager.instance.playSFX(audioClips[0]);
                break;
            case PlayerAttack.AttackType.ProjectileOrShield:
                //Audio.PlayOneShot(audioClips[1]);
                SFXManager.instance.playSFX(audioClips[1]);
                break;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ForceRetarget()
    {
        StopAllCoroutines();
        FollowTarget = null;
        StartCoroutine(SetTargetRecursive());
    }

    public void UpdateHealthBarCallback()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = CurrentHealth / maxHealth;
            bossHealthText.text = $"{CurrentHealth} / {maxHealth}";
        }
    }
    public void attackSound1()
    {
        SFXManager.instance.playSFX(audioClips[2]);
    }
    public void attackSound2()
    {
        SFXManager.instance.playSFX(audioClips[3]);
    }
    public void attackSound3()
    {
        SFXManager.instance.playSFX(audioClips[4]);
    }

    public void BossDead()
    {
        IsDead = true;
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_Mission_Accomplished()
    {
        Runner.Disconnect(Runner.LocalPlayer);
        Destroy(InputManager.Instance);
        Destroy(Runner.gameObject);
        SceneManager.LoadScene("Ending");
    }
}
