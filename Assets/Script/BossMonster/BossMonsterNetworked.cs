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
    [Networked, OnChangedRender(nameof(UpdateBossPhaseCallback))] public int BossPhase { get; set; } = 1;
    [Networked] public CustomTickTimer BossAttackTimer { get; set; }
    [Networked, OnChangedRender(nameof(UpdateHealthBarCallback))] public float CurrentHealth { get; set; }
    [Networked] public float BossSpeed { get; set; } = 5.0f;
    [Networked] public NetworkObject FollowTarget { get; set; }
    [Networked] public BossState CurrentState { get; set; }
    [Networked] public Condition bossCondition { get; set; }
    [Networked] public AttackType attackType { get; set; }
    [Networked] public float conditionDuration { get; set; }
    [Networked] public float currentDistance { get; set; }
    [Networked] public bool isAttacking { get; set; }
    [Networked] public bool isMoving { get; set; }
    // Animator parameters
    [Networked] public int P_WalkState { get; set; }
    [Networked] public bool P_DoAttack { get; set; }
    [Networked] public bool P_DoAttack2 { get; set; }
    [Networked] public bool P_DoJumpAttack { get; set; }
    [Networked] public bool P_Shunpo { get; set; }

    // Local variables
    [SerializeField] private GameObject phase1;
    [SerializeField] private GameObject phase2;
    [SerializeField] private RuntimeAnimatorController _phase1Animator;
    [SerializeField] private RuntimeAnimatorController _phase2Animator;
    public BossAttack phase1BossAttack;
    public BossAttack phase2BossAttack;
    NetworkRigidbody2D _rb;
    Animator _currentAnimator;
    NetworkMecanimAnimator _networkAnimator;
    public readonly float maxHealth = 50000.0f;
    // public GameObject effectPool;
    public CameraMovement cameraMovement;
    public Image healthBar;
    public DurationIndicator durationIndicator;
    public BossAttack currentBossAttack;
    public TextMeshProUGUI bossHealthText;
    public List<BossHitFeedbackEffect> BossHitFeedbackEffects = new();
    public AudioClip[] audioClips;
    private AudioSource Audio;
    void Awake()
    {
        _rb = GetComponent<NetworkRigidbody2D>();
        _currentAnimator = GetComponent<Animator>();
        _networkAnimator = GetComponent<NetworkMecanimAnimator>();
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
        for (int i = 0; i < objects.Length; i++)
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
                ScheduledBehaviors.Behavior behavior = ScheduledBehaviors.instance.GetBehavior(maxHealth, CurrentHealth, isAttacking, BossPhase);
                if (!Equals(behavior.runBy, ScheduledBehaviors.RunBy.Default))
                {
                    StartCoroutine(AttackController(
                        GetBossSkill((string)behavior.skillName, BossPhase)
                            .Attack(transform, _currentAnimator, Runner, currentBossAttack, Object)));
                }
                BossBehaviour();
            }
        }
    }
    // Networked animation
    // 뭔 짓을 쳐 해도 AssertException이 계속 뜨네
    public override void Render()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        _networkAnimator.Animator.SetInteger("WalkState", P_WalkState);
        if (P_DoAttack)
        {
            _networkAnimator.Animator.SetTrigger("DoAttack");
            P_DoAttack = false;
        }
        if (P_DoAttack2)
        {
            _networkAnimator.Animator.SetTrigger("DoAttack2");
            P_DoAttack2 = false;
        }
        if (P_DoJumpAttack)
        {
            _networkAnimator.Animator.SetTrigger("DoJumpAttack");
            P_DoJumpAttack = false;
        }
        if (P_Shunpo)
        {
            _networkAnimator.Animator.SetTrigger("Shunpo");
            P_Shunpo = false;
        }
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
                transform.localScale = new Vector3(2, 2, 1);
            }
            else
            {
                transform.localScale = new Vector3(-2, 2, 1);
            }
            P_WalkState = 1;
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
            P_WalkState = 0;
            yield return null;
        }
    }

    BossSkill GetBossSkill(string skillName, int phase)
    {
        return AttackManager.Instance.BossAttacks.Find(
            skill => skill.name == skillName && skill.phase == phase
        );
    }

    // This function will be called by the animation event
    public void JumpTP()
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
            if (conditionDuration >= 2.0f)
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
                BossAttackTimer = CustomTickTimer.CreateFromSeconds(Runner, Random.Range(1.5f, 2.0f));
            }
        }
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
                // If player is not in attack range, cancel the attack
                // and set state as Move
                if (!bossCondition.HasFlag(Condition.IsPlayerInAttackRange) && attackType == AttackType.Melee)
                {
                    Debug.Log("Player is not in attack range");
                    CurrentState = BossState.Move;
                    isAttacking = false;
                    return;
                }
                switch (BossPhase)
                {
                    case 1:
                        switch (attackType)
                        {
                            // Attack controller makes sure that the attack is executed once
                            // If end of the attack is reached, isAttacking is set as false
                            case AttackType.Melee:
                                int phase1_melee_skillRandom = Random.Range(0, 4);
                                switch (phase1_melee_skillRandom)
                                {
                                    case 0:
                                        BossSkill phase1_baseAttack = GetBossSkill("BaseAttack1", BossPhase);
                                        // you can modify attack damage like this
                                        // attack.damage = 100;
                                        // call coroutine with attack.Attack(_animator, Runner, bossAttack)
                                        StartCoroutine(AttackController(phase1_baseAttack.Attack(transform, _currentAnimator, Runner, currentBossAttack, Object)));
                                        Debug.Log("Do Melee Attack");
                                        break;

                                    case 1:
                                        BossSkill phase1_backAttack = GetBossSkill("BackAttack", BossPhase);
                                        StartCoroutine(AttackController(phase1_backAttack.Attack(transform, _currentAnimator, Runner, currentBossAttack, Object)));
                                        Debug.Log("Do Melee Attack2");
                                        break;

                                    case 2:
                                        BossSkill phase1_bothAttack = GetBossSkill("BothAttack", BossPhase);
                                        StartCoroutine(AttackController(phase1_bothAttack.Attack(transform, _currentAnimator, Runner, currentBossAttack, Object)));
                                        Debug.Log("Do bothAttack");
                                        break;

                                    case 3:
                                        BossSkill phase1_energyAttack = GetBossSkill("EnergyAttack", BossPhase);
                                        StartCoroutine(AttackController(phase1_energyAttack.Attack(transform, _currentAnimator, Runner, currentBossAttack, Object)));
                                        Debug.Log("Do AttackWithEnergy");
                                        break;
                                }
                                BossSkill phase1_defaultAttack = GetBossSkill("BaseAttack1", BossPhase);
                                StartCoroutine(AttackController(phase1_defaultAttack.Attack(transform, _currentAnimator, Runner, currentBossAttack, Object)));
                                Debug.Log("Do Melee Attack");
                                break;
                            case AttackType.JumpDash:
                                int phase1_jumpDash_skillRandom = Random.Range(0, 1);
                                switch (phase1_jumpDash_skillRandom)
                                {
                                    case 0:
                                        BossSkill phase1_jumpDashAttack = GetBossSkill("JumpAttack", BossPhase);
                                        StartCoroutine(AttackController(phase1_jumpDashAttack.Attack(transform, _currentAnimator, Runner, currentBossAttack, Object)));
                                        Debug.Log("Do Jump Dash Attack");
                                        break;
                                }
                                break;
                        }
                        break;
                    case 2:
                        switch (attackType)
                        {
                            case AttackType.Melee:
                                int phase2_melee_skillRandom = Random.Range(0, 4);
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

    // This function is used to execute the boss skill by debug console
    public bool ExecuteSkill(string skillName)
    {
        RPC_ForceRetarget();
        BossSkill skill = GetBossSkill(skillName, BossPhase);
        if (skill == null)
        {
            return false;
        }
        StartCoroutine(skill.Attack(transform, _currentAnimator, Runner, currentBossAttack, Object));
        return true;
    }

    private float GetDistance(Vector3 target)
    {
        return Vector2.Distance(transform.position, target);
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
                SFXManager.Instance.playSFX(audioClips[0]);
                break;
            case PlayerAttack.AttackType.ProjectileOrShield:
                //Audio.PlayOneShot(audioClips[1]);
                SFXManager.Instance.playSFX(audioClips[1]);
                break;
        }
        RPC_PlaySound(attack.attackType);
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
            if (BossPhase == 2)
            {
                BossDead();
            }
            else if (BossPhase == 1)
            {
                BossPhase++;
                CurrentHealth = maxHealth;
                RPC_ForceRetarget();
            }
        }

    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlaySound(PlayerAttack.AttackType a)
    {
        switch (a)
        {
            case PlayerAttack.AttackType.Katana:

                //Audio.PlayOneShot(audioClips[0]);
                SFXManager.Instance.playSFX(audioClips[0]);
                break;
            case PlayerAttack.AttackType.ProjectileOrShield:
                //Audio.PlayOneShot(audioClips[1]);
                SFXManager.Instance.playSFX(audioClips[1]);
                break;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ForceRetarget()
    {
        StopAllCoroutines();
        FollowTarget = null;
        isAttacking = false;
        isMoving = false;
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

    public void UpdateBossPhaseCallback()
    {
        RPC_ForceRetarget();
        if (BossPhase == 1)
        {
            phase1.SetActive(true);
            currentBossAttack = phase1BossAttack;
            _currentAnimator.runtimeAnimatorController = _phase1Animator;
            phase2.SetActive(false);
        }
        else if (BossPhase == 2)
        {
            phase1.SetActive(false);
            currentBossAttack = phase2BossAttack;
            _currentAnimator.runtimeAnimatorController = _phase2Animator;
            phase2.SetActive(true);
            ScheduledBehaviors.Behavior[] behaviors =
            {
                new() {
                    skillName = "SpawnBindSword",
                    runBy = ScheduledBehaviors.RunBy.Tick,
                    tick = Runner.Tick + (15 * Runner.TickRate),
                    phase = 2,
                    canPend = false,
                    canRenew = true,
                    renewTick = 15 * Runner.TickRate
                },
            };
            foreach (var behavior in behaviors)
            {
                ScheduledBehaviors.instance.AddBehavior(behavior);
            }

        }
    }

    public void attackSound1()
    {
        SFXManager.Instance.playSFX(audioClips[2]);
    }
    public void attackSound2()
    {
        SFXManager.Instance.playSFX(audioClips[3]);
    }
    public void attackSound3()
    {
        SFXManager.Instance.playSFX(audioClips[4]);
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
        SceneManager.LoadScene("Ending");
        Runner.Disconnect(Runner.LocalPlayer);
        Destroy(InputManager.Instance);
        Destroy(Runner.gameObject);
    }
}
