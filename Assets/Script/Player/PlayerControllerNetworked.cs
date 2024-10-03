using Fusion;
using Fusion.Addons.Physics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class PlayerControllerNetworked : NetworkBehaviour
{
    // Networked Variables
    [Networked, OnChangedRender(nameof(LifeChanged))] public int PlayerLifes { get; set; } = 2;
    [Networked] public bool isLeader { get; set; } = false;
    [Networked, OnChangedRender(nameof(ReadyStatusChanged))] public bool isReady { get; set; } = false;
    [Networked, OnChangedRender(nameof(NickNameChanged))] public NetworkString<_16> NickName { get; set; }
    [Networked, OnChangedRender(nameof(ClassChanged))] public int CharacterClass { get; set; }
    [Networked] public float MaxHealth { get; set; } = 100;
    [Networked, OnChangedRender(nameof(HPChanged))] public float CurrentHealth { get; set; } = 100;
    [Networked] public PlayerRef Player { get; set; }
    [Networked] public int CurrentServerTick { get; set; }
    [Networked] public CustomTickTimer DurationTickTimer { get; set; }

    [Networked]
    [Capacity(10)]
    public NetworkDictionary<NetworkString<_16>, SkillStruct> SkillList { get; }
    [Networked] public CustomTickTimer SkillTickTimer { get; set; }
    [Networked] public CustomTickTimer HealthRegenIntervalTickTimer { get; set; }
    [Networked] public NetworkObject SkillObject { get; set; }
    [Networked] public NetworkBool IsBinded { get; set; }
    [Networked] public NetworkBool IsConsoleFocused { get; set; }
    // Animator parameters
    [Networked] public int RunState { get; set; }
    [Networked] public bool Grounded { get; set; }
    [Networked] public bool Falling { get; set; }
    [Networked] public int AttackState { get; set; }
    [Networked] public float AttackAnimSpeed { get; set; }
    [Networked] public float PrevAttack { get; set; }
    [Networked] public bool Combo { get; set; }
    [Networked] public bool Attack { get; set; }
    [Networked] public bool P_Jump { get; set; }
    [Networked] public bool P_Roll { get; set; }
    [Networked] public bool P_Parry { get; set; }

    [Networked]
    [Capacity(12)]
    [OnChangedRender(nameof(ApplyMultipliers))]
    public NetworkArray<CharacterStatMultiplier> CharacterStatMultipliers { get; }
        = MakeInitializer(new CharacterStatMultiplier[] { });

    // Local Variables
    [Space]
    public GameObject rangeObject;
    [Space]
    public PlayerBuffs buffs;
    NetworkRigidbody2D _rb;
    PlayerInputConsumer _input;
    TestBuffIndicator buffIndicator;
    public Collider2D _collider;
    Animator _anim;
    NetworkMecanimAnimator _mecanim;
    public DurationIndicator durationIndicator;
    public Image healthBar;
    public OtherStatusPanel otherStatusPanel;
    public LobbyUIController lobbyUI;

    LifeIndicator lifeUI;

    #region
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _enemyLayer;

    [Space]
    public float speed;
    public float speedMultiplier = 1;
    public float attackSpeed;
    public float attackSpeedMultiplier = 1;
    public float damageMultiplier = 1;
    public Items.Weapon weapon;
    [Space]
    [Header("Knockback")]
    // knockback is applied by boss attack
    // knockbackForce is the distance of knockback for each tick
    // knockbackApplyTick is the tick count of knockback
    // knockbackYMultiplier is the multiplier of y axis of knockback
    public float knockbackForce = 0.3f;
    public int knockbackApplyTick = 15;
    public float knockbackYMultiplier = 0.8f;
    [Space]
    //[SerializeField] float _jumpForce = 10f; 
    [SerializeField] float _jumpHeight = 10f;
    [SerializeField] float _timeToApex = 0.5f;
    //[SerializeField] float _DoubleJumpForce = 8f;
    [SerializeField] float _DoubleJumpHeight = 8f;
    [SerializeField] float _maxVelocity = 8f;

    [SerializeField] float _rollDuration = 0.5f; // ?�� ???? ?��?
    [SerializeField] float _rollDistance = 6.0f; // ?�� ???

    [Space]

    [SerializeField] private float fallMultiplier = 3.3f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Space]

    [SerializeField] Vector2 _groundHorizontalDragVector = new Vector2(.1f, 1);
    [SerializeField] Vector2 _airHorizontalDragVector = new Vector2(.98f, 1);
    [SerializeField] Vector2 _horizontalSpeedReduceVector = new Vector2(.95f, 1);
    [SerializeField] Vector2 _verticalSpeedReduceVector = new Vector2(1, .95f);

    [Space]
    [SerializeField]
    bool IsGrounded = false;
    bool _isRolling = false;

    float _jumpBufferThreshold = .2f;
    float _jumpBufferTime;

    float _rollBufferThreshold = .2f;
    float _rollBufferTime;


    float CoyoteTimeThreshold = .1f;
    float TimeLeftGrounded;
    bool CoyoteTimeCD;
    bool WasGrounded;
    bool hasDoubleJumped;

    [SerializeField]
    Vector3 Velocity;
    [SerializeField]
    Vector3 GroundcheckPosition;
    #endregion

    void Awake()
    {
        // Initialize local variables
        _rb = gameObject.GetComponent<NetworkRigidbody2D>();
        _input = gameObject.GetComponent<PlayerInputConsumer>();
        _collider = GetComponent<Collider2D>();
        _mecanim = GetComponent<NetworkMecanimAnimator>();
        durationIndicator = GameObject.FindGameObjectWithTag("DurationUI").GetComponent<DurationIndicator>();
        buffs = gameObject.GetComponent<PlayerBuffs>();

        lobbyUI = FindAnyObjectByType<LobbyUIController>();
    }

    void Start()
    {
        if (HasInputAuthority)
        {
            healthBar = GameObject.FindGameObjectWithTag("CharacterHealthUI").GetComponent<Image>();
            lifeUI = GameObject.FindGameObjectWithTag("CharacterLifeUI").GetComponent<LifeIndicator>();
            // Set camera follow target
            Camera.main.GetComponent<CameraMovement>().followTarget = gameObject;
            buffIndicator = GameObject.FindGameObjectWithTag("BuffIndicator").GetComponent<TestBuffIndicator>();
            buffIndicator.playerBuffs = buffs;
            buffIndicator.player = this;
            buffs.buffIndicator = buffIndicator;
            buffs.Test();
            DebugConsole.Instance.localPlayer = this;
            RPC_SetNickName(Runner.gameObject.GetComponent<NetworkManager>().nickName);
            RPC_SetClass(0);

        }
        else
        {
            Canvas canvas = GameObject.FindGameObjectWithTag("InGameUICanvas").GetComponent<Canvas>();
            GameObject otherStatusPrefab = Runner.gameObject.GetComponent<NetworkManager>().otherStatusPrefab;
            GameObject other = Instantiate(otherStatusPrefab, canvas.gameObject.transform);
            OtherStatusPanel osp = other.GetComponent<OtherStatusPanel>();
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            // Always render behind local player
            spriteRenderer.sortingOrder = -1;
            otherStatusPanel = osp;
            osp.player = this;
            osp.buffIndicator.playerBuffs = buffs;
            buffs.buffIndicator = osp.buffIndicator;

            other.SetActive(true);

            OtherPanelUpdate();
            UpdateCharacterClass(CharacterClass);
        }

        UpdateLobbyUI();
    }

    // Networked animation
    public override void Render()
    {
        _mecanim.Animator.SetBool("Grounded", Grounded);
        _mecanim.Animator.SetBool("Falling", Falling);
        _mecanim.Animator.SetInteger("RunState", RunState);
        _mecanim.Animator.SetInteger("AttackState", AttackState);
        _mecanim.Animator.SetFloat("AttackAnimSpeed", AttackAnimSpeed);
        _mecanim.Animator.SetFloat("PrevAttack", PrevAttack);
        _mecanim.Animator.SetBool("Combo", Combo);
        _mecanim.Animator.SetBool("Attack", Attack);
        _mecanim.Animator.SetBool("Jump", P_Jump);
        _mecanim.Animator.SetBool("Roll", P_Roll);
        if (CharacterClass == (int)CharacterClassEnum.Tank)
        {
            _mecanim.Animator.SetBool("Parry", P_Parry);
        }
        if (Attack)
        {
            Attack = false;
        }
        if (P_Jump)
        {
            P_Jump = false;
        }
        if (P_Roll)
        {
            P_Roll = false;
        }
        if (P_Parry)
        {
            P_Parry = false;
        }
        base.Render();
    }

    // Character class change function
    void UpdateCharacterClass(int characterClass)
    {
        Debug.Log("Set character class: " + characterClass);
        global::CharacterClass.ChangeClass(characterClass, gameObject);
    }

    // Test function to check if player is grounded
    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube((Vector2)transform.position + (Vector2)GroundcheckPosition, Vector2.one * .85f);
    // }

    // Ground check function
    void DetectGroundAndWalls()
    {
        WasGrounded = IsGrounded;
        IsGrounded = default;
        IsGrounded = (bool)Runner.GetPhysicsScene2D().OverlapBox((Vector2)transform.position + (Vector2)GroundcheckPosition, Vector2.one * .85f, 0, _groundLayer);
        // Jump animation and run animation depends on this value
        Grounded = IsGrounded;
        if (IsGrounded)
        {
            CoyoteTimeCD = false;
            hasDoubleJumped = false;
            return;
        }

        if (WasGrounded)
        {
            if (CoyoteTimeCD)
            {
                CoyoteTimeCD = false;
            }
            else
            {
                TimeLeftGrounded = Runner.SimulationTime;
            }
        }
    }

    // Networked physics
    public override void FixedUpdateNetwork()
    {
        CurrentServerTick = (int)Runner.Tick;
        if (
            // conditon 1: if room is open to join
            (Runner.SessionInfo.IsOpen == true)
            ||
            // condition 2: if player is binded by boss
            (
                IsBinded
            )
            ||
            // condition 3: if player focus on console
            (
                IsConsoleFocused
            )
        )
        {
            _rb.Rigidbody.velocity = new Vector2(0, _rb.Rigidbody.velocity.y);
            RunState = 0;
        }
        else
        {
            InputTask();
        }
        Velocity = _rb.Rigidbody.velocity;
        if (weapon != null)
        {
            weapon.attackSpeed = attackSpeed * attackSpeedMultiplier;
            weapon.damageMultiplier = damageMultiplier;
        }
        // RPC_GetTickDeltaBetweenClients(nickName);
    }

    void InputTask()
    {
        // dir is the direction of the player(left, right)
        var dir = _input.dir.normalized;
        UpdateMovement(dir.x);
        // PlayerButtons are set in OnInput function of CharacterSpawner.cs script
        Jump(_input.pressed.IsSet(PlayerButtons.Jump));
        BetterJumpLogic(_input.pressed.IsSet(PlayerButtons.Jump));
        Roll(_input.pressed.IsSet(PlayerButtons.Roll));
        Parry(_input.pressed.IsSet(PlayerButtons.Parry));
        // Run attack coroutine of weapon script directly
        if (_input.pressed.IsSet(PlayerButtons.Attack))
        {
            StartCoroutine(weapon.Attack(_mecanim.Animator, _mecanim, gameObject.transform));
        }


    }
    void UpdateMovement(float input)
    {
        if (_isRolling) return;
        // Run animation
        if (input != 0)
        {
            RunState = 1;
        }
        else
        {
            RunState = 0;
        }
        if (_rb.Rigidbody.velocity.y < -3)
        {
            Falling = true;
        }
        else
        {
            Falling = false;
        }
        // Ground check
        DetectGroundAndWalls();
        if (input < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            //Reset x velocity if start moving in oposite direction.
            if (_rb.Rigidbody.velocity.x > 0 && IsGrounded)
            {
                _rb.Rigidbody.velocity *= Vector2.up;
            }
            _rb.Rigidbody.velocity = new Vector2(-1 * speed * speedMultiplier, _rb.Rigidbody.velocity.y);
            // _rb.Rigidbody.AddForce(speed * Vector2.left, ForceMode2D.Impulse);
        }
        else if (input > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            //Reset x velocity if start moving in oposite direction.
            if (_rb.Rigidbody.velocity.x < 0 && IsGrounded)
            {
                _rb.Rigidbody.velocity *= Vector2.up;
            }
            _rb.Rigidbody.velocity = new Vector2(speed * speedMultiplier, _rb.Rigidbody.velocity.y);
            // _rb.Rigidbody.AddForce(speed * Vector2.right, ForceMode2D.Impulse);
        }
        else
        {
            //Different horizontal drags depending if grounded or not.
            if (IsGrounded)
                _rb.Rigidbody.velocity *= _groundHorizontalDragVector;
            else
                _rb.Rigidbody.velocity *= _airHorizontalDragVector;
        }

        LimitSpeed();
    }

    private void LimitSpeed()
    {
        //Limit horizontal velocity
        if (Mathf.Abs(_rb.Rigidbody.velocity.x) > _maxVelocity)
        {
            _rb.Rigidbody.velocity *= _horizontalSpeedReduceVector;
        }

        if (Mathf.Abs(_rb.Rigidbody.velocity.y) > _maxVelocity * 2)
        {
            _rb.Rigidbody.velocity *= _verticalSpeedReduceVector;
        }
    }

    public void Skill()
    {
        // now it is only for tank's skill
        if (CharacterClass != (int)CharacterClassEnum.Tank)
        {
            return;
        }
        SkillStruct skillStruct = SkillList.Get("Aura");
        CustomTickTimer cooldownTimer = skillStruct.coolDownTimer;
        if (Equals(cooldownTimer, default(CustomTickTimer)) || cooldownTimer.Expired(Runner))
        {
            skillStruct.coolDownTimer = CustomTickTimer.CreateFromSeconds(Runner, skillStruct.coolDown);
            RPC_CreateDurationIndicator(skillStruct.duration, "아우라 쿨다운");
            SkillList.Set("Aura", skillStruct);
            StartCoroutine(weapon.Skill(gameObject.transform, skillStruct.duration));
        }
        else
        {
            var remainingTime = (Runner.TickRate * skillStruct.coolDown - skillStruct.coolDownTimer.ElapsedTicks(Runner)) / 64;
            Debug.Log($"Skill is on cooldown. Remaining time: {remainingTime}");
        }
    }

    private void Parry(bool parry)
    {
        if (parry && CharacterClass == (int)CharacterClassEnum.Tank)
        {
            SkillStruct skillStruct = SkillList.Get("Parry");
            CustomTickTimer cooldownTimer = skillStruct.coolDownTimer;
            if (Equals(cooldownTimer, default(CustomTickTimer)) || cooldownTimer.Expired(Runner))
            {
                skillStruct.coolDownTimer = CustomTickTimer.CreateFromSeconds(Runner, skillStruct.coolDown);
                RPC_CreateDurationIndicator(skillStruct.coolDown, "패링 쿨다운");
                SkillList.Set("Parry", skillStruct);
                StartCoroutine(weapon.Parry(gameObject.transform));
            }
            else
            {
                var remainingTime = (Runner.TickRate * skillStruct.coolDown - skillStruct.coolDownTimer.ElapsedTicks(Runner)) / 64;
                Debug.Log($"Skill is on cooldown. Remaining time: {remainingTime}");
            }
        }
    }

    private void Jump(bool jump)
    {

        //Jump
        if (jump || CalculateJumpBuffer())
        {
            // Deprecated jump function
            // void _jump(float __jumpForce)
            // {
            //     _rb.Rigidbody.velocity *= Vector2.right; //Reset y Velocity
            //     _rb.Rigidbody.AddForce(Vector2.up * __jumpForce, ForceMode2D.Impulse);
            //     CoyoteTimeCD = true;

            // }
            void advanced_jump(float jumpHeight, float timeToApex = 0.5f)
            {
                /*float gravity = (2 * jumpHeight) / Mathf.Pow(timeToApex, 2);
                float initialJumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);

                _rb.Rigidbody.velocity = new Vector2(_rb.Rigidbody.velocity.x, initialJumpVelocity);
                Physics.gravity = new Vector3(0, -gravity, 0);*/

                _rb.Rigidbody.velocity = new Vector2(_rb.Rigidbody.velocity.x, jumpHeight);
                CoyoteTimeCD = true;
            }
            if (!IsGrounded && jump)
            {
                if (!hasDoubleJumped)
                {
                    // Run jump animation
                    P_Jump = true;
                    advanced_jump(_DoubleJumpHeight, _timeToApex);
                    hasDoubleJumped = true;
                }

                _jumpBufferTime = Runner.SimulationTime;
            }

            if (IsGrounded || CalculateCoyoteTime())
            {
                // Run jump animation
                P_Jump = true;
                advanced_jump(_jumpHeight, _timeToApex);
            }


        }
    }

    private bool CalculateJumpBuffer()
    {
        return (Runner.SimulationTime <= _jumpBufferTime + _jumpBufferThreshold) && IsGrounded;
    }

    private bool CalculateCoyoteTime()
    {
        return (Runner.SimulationTime <= TimeLeftGrounded + CoyoteTimeThreshold);
    }

    private void BetterJumpLogic(bool input)
    {
        if (IsGrounded) { return; }
        if (_rb.Rigidbody.velocity.y < 0)
        {
            if (_rb.Rigidbody.velocity.y > 0 && !input)
            {
                _rb.Rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Runner.DeltaTime;
            }
            else
            {
                _rb.Rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Runner.DeltaTime;
            }
        }

    }
    // In progress
    private void Roll(bool dash)
    {
        if (dash || CalculateRollBuffer())
        {
            if (_isRolling || !IsGrounded && dash)
            {
                _rollBufferTime = Runner.SimulationTime;
            }

            if (IsGrounded && dash && !_isRolling) // if player is grounded and dash button is pressed and player is not rolling
            {
                SkillStruct rollSkillStruct = SkillList.Get("Roll");
                CustomTickTimer cooldownTimer = rollSkillStruct.coolDownTimer;
                if (Equals(cooldownTimer, default(CustomTickTimer)) || cooldownTimer.Expired(Runner))
                {
                    rollSkillStruct.coolDownTimer = CustomTickTimer.CreateFromSeconds(Runner, rollSkillStruct.coolDown);
                    SkillList.Set("Roll", rollSkillStruct);
                    StartCoroutine(RollCoroutine());
                }
                else
                {
                    var remainingTime = (Runner.TickRate * rollSkillStruct.coolDown - rollSkillStruct.coolDownTimer.ElapsedTicks(Runner)) / 64;
                    Debug.Log($"Skill is on cooldown. Remaining time: {remainingTime}");
                }
            }
        }
    }
    // It will be called by animation event
    // projectile fire
    public void FireProjectile()
    {
        weapon.FireProjectileAlt(_mecanim.Animator.GetInteger("AttackState"), gameObject.transform);
    }

    // It will be called by animation event
    // apply rush
    public void ApplyRush()
    {
        weapon.ApplyRush(transform);
    }

    // It will be called by animation event
    // apply jump after rush
    public void ApplyJump()
    {
        weapon.ApplyJump();
    }

    private bool CalculateRollBuffer()
    {
        return (Runner.SimulationTime <= _rollBufferTime + _rollBufferThreshold) && IsGrounded;
    }

    // Apply knockback by boss attack
    public IEnumerator ApplyKnockback(Vector2 direction, float force)
    {
        if (!IsGrounded)
        {
            yield break;
        }
        CustomTickTimer knockbackApplyTickTimer = CustomTickTimer.CreateFromTicks(Runner, knockbackApplyTick);
        while (!knockbackApplyTickTimer.Expired(Runner))
        {
            _rb.Rigidbody.velocity = Vector2.zero;
            _rb.Rigidbody.MovePosition(new Vector2(
                _rb.Rigidbody.position.x + direction.x * force,
                _rb.Rigidbody.position.y + force * knockbackYMultiplier
            ));
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator ApplyBind(float duration)
    {
        IsBinded = true;
        _collider.excludeLayers = _enemyLayer;
        CustomTickTimer bindDurationTimer = CustomTickTimer.CreateFromSeconds(Runner, duration);
        while (!bindDurationTimer.Expired(Runner))
        {
            yield return new WaitForFixedUpdate();
        }
        IsBinded = false;
        _collider.excludeLayers = 0;
    }

    private IEnumerator RollCoroutine()
    {
        DurationTickTimer = CustomTickTimer.CreateFromSeconds(Runner, _rollDuration);
        _isRolling = true;
        P_Roll = true;
        Vector2 dashDirection = _rb.Rigidbody.velocity.normalized; // -1 or 1
        _rb.Rigidbody.velocity = dashDirection * Vector2.right * _rollDistance / _rollDuration;
        _collider.excludeLayers = _enemyLayer;
        while (!DurationTickTimer.Expired(Runner))
        {
            yield return new WaitForFixedUpdate();
        }
        _collider.excludeLayers = 0;

        _rb.Rigidbody.velocity = new Vector2(0, _rb.Rigidbody.velocity.y); // reset velocity except y
        _isRolling = false;
        yield return new WaitForFixedUpdate();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetNickName(string nick)
    {
        if (nick == string.Empty)
        {
            NickName = $"플레이어 {Player.AsIndex}";
        }
        else
        {
            NickName = nick;
        }
    }

    private void NickNameChanged()
    {
        string _ = $"[{Player.AsIndex}] {NickName}";
        if (otherStatusPanel) otherStatusPanel.SetName(_);

        if (lobbyUI) lobbyUI.SetNick(Player, _);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetClass(int classTypeInt)
    {
        CharacterClass = classTypeInt;
        RPC_UpdateClass(classTypeInt);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateClass(int classTypeInt)
    {
        UpdateCharacterClass(classTypeInt);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void RPC_CreateDurationIndicator(float duration, string name)
    {
        durationIndicator.CreateDurationIndicator(duration, name);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_OnPlayerHit(BossAttack.AttackData attackData)
    {
        CurrentHealth -= attackData.damage;
        if (attackData.isApplyKnockback)
        {
            StartCoroutine(ApplyKnockback(attackData.knockbackDirection, knockbackForce));
        }
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            // Revive when player lifes remain
            if (PlayerLifes - 1 > 0)
            {
                CurrentHealth = MaxHealth;
                PlayerLifes--;
            }
            else
            {
                PlayerLifes--;
                OtherPanelHpZero();
                Runner.Despawn(GetComponent<NetworkObject>());
            }

        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ApplyBind(float duration)
    {
        StartCoroutine(ApplyBind(duration));
    }

    // RPC function to apply aura buffs
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_OnPlayerInSupporterAura(Aura.AuraBuffs auraBuffs)
    {
        Buff auraBuff = new()
        {
            type = (int)BuffTypes.Aura,
            stacks = 0,
            startTime = Runner.RemoteRenderTime,
            duration = 2f,
        };
        // Debug.Log($"Aura buff is exist: {!Equals(buffs.GetBuff(BuffTypes.Aura), default(Buff))}");
        Buff buff = buffs.GetBuff(BuffTypes.Aura);
        if (buff.type == 0) buffs.SetBuff(auraBuff);
        buff = buffs.GetBuff(BuffTypes.Aura);
        CharacterStatMultiplier attackSppedMultiplier = new()
        {
            name = "AttackSpeed",
            value = auraBuffs.attackSpeedMultiplier,
            buff = buff
        };
        CharacterStatMultiplier speedMultiplier = new()
        {
            name = "Speed",
            value = auraBuffs.moveSpeedMultiplier,
            buff = buff
        };
        CharacterStatMultiplier damageMultiplier = new()
        {
            name = "Damage",
            value = auraBuffs.damageMultiplier,
            buff = buff
        };
        // Debug.Log($"Indexs: {GetMultiplierIndex("AttackSpeed", buff)} {GetMultiplierIndex("Speed", buff)} {GetMultiplierIndex("Damage", buff)}");
        CharacterStatMultipliers.Set(GetMultiplierIndex("AttackSpeed", buff), attackSppedMultiplier);
        CharacterStatMultipliers.Set(GetMultiplierIndex("Speed", buff), speedMultiplier);
        CharacterStatMultipliers.Set(GetMultiplierIndex("Damage", buff), damageMultiplier);

        // Apply health regen
        if (Equals(HealthRegenIntervalTickTimer, default(CustomTickTimer)) || HealthRegenIntervalTickTimer.Expired(Runner))
        {
            HealthRegenIntervalTickTimer = CustomTickTimer.CreateFromSeconds(Runner, auraBuffs.healthRegenInterval);
            CurrentHealth += auraBuffs.healthRegen;
        }
    }

    private int GetMultiplierIndex(string name, Buff buff)
    {
        try
        {
            int index = CharacterStatMultipliers
                .Select((x, i) => new { multiplier = x, index = i })
                .Where(x => x.multiplier.name == name && Equals(x.multiplier.buff.type, buff.type))
                .First().index;
            return index;
        }
        catch
        {
            int index;
            for (index = 0; index < CharacterStatMultipliers.Length; index++)
            {
                if (Equals(CharacterStatMultipliers.Get(index), default(CharacterStatMultiplier)))
                {
                    break;
                }
            }
            return index;
        }
    }

    public void ApplyMultipliers()
    {
        var attackSpeedMultipliers = new List<CharacterStatMultiplier>();
        var speedMultipliers = new List<CharacterStatMultiplier>();
        var damageMultipliers = new List<CharacterStatMultiplier>();

        for (int i = 0; i < CharacterStatMultipliers.Length; i++)
        {
            var multiplier = CharacterStatMultipliers.Get(i);
            if (Equals(multiplier, default(CharacterStatMultiplier)))
            {
                continue;
            }
            if (multiplier.name == "AttackSpeed")
            {
                attackSpeedMultipliers.Add(multiplier);
            }
            else if (multiplier.name == "Speed")
            {
                speedMultipliers.Add(multiplier);
            }
            else if (multiplier.name == "Damage")
            {
                damageMultipliers.Add(multiplier);
            }
        }

        attackSpeedMultiplier = attackSpeedMultipliers.Count > 0 ?
            attackSpeedMultipliers.Select(x => x.value)
                .Aggregate((x, y) => x * y) : 1;
        speedMultiplier = speedMultipliers.Count > 0 ?
            speedMultipliers.Select(x => x.value)
                .Aggregate((x, y) => x * y) : 1;
        damageMultiplier = damageMultipliers.Count > 0 ?
            damageMultipliers.Select(x => x.value)
                .Aggregate((x, y) => x * y) : 1;
    }

    public void ResetAttackHit()
    {
        weapon.rangeObject.GetComponent<PlayerAttack>().isHit = false;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RemoveMultipliers(Buff buff)
    {
        for (int i = 0; i < CharacterStatMultipliers.Length; i++)
        {
            var multiplier = CharacterStatMultipliers.Get(i);
            Debug.Log($"{multiplier.buff.type} {buff.type}");
            if (Equals(multiplier, default(CharacterStatMultiplier)))
            {
                continue;
            }
            if (multiplier.buff.type == buff.type)
            {
                CharacterStatMultipliers.Set(i, default);
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ToggleConsoleFocus()
    {
        IsConsoleFocused = !IsConsoleFocused;
    }

    // TEST RPC FUNCTION
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_GetTickDeltaBetweenClients(NetworkString<_16> nick)
    {
        Debug.Log($"{nick} - Tick delta:  {(float)Runner.LocalRenderTime * Runner.TickRate - CurrentServerTick}");
    }

    private void ClassChanged()
    {
        UpdateCharacterClass(CharacterClass);
        if (lobbyUI) lobbyUI.SetClass(Player, CharacterClass);
        if (otherStatusPanel)
        {
            otherStatusPanel.SetClass(CharacterClass);
        }
    }

    private void HPChanged()
    {
        if (otherStatusPanel)
        {
            otherStatusPanel.SetHP(CurrentHealth, MaxHealth);
        }

        if (healthBar)
        {
            healthBar.fillAmount = CurrentHealth / MaxHealth;
        }
    }

    private void LifeChanged()
    {
        if (otherStatusPanel)
        {
            otherStatusPanel.SetLife(PlayerLifes);
        }

        if (lifeUI)
        {
            lifeUI.SetLife(PlayerLifes);
        }
    }

    public void OtherPanelUpdate()
    {
        if (otherStatusPanel)
        {
            otherStatusPanel.SetClass(CharacterClass);
            otherStatusPanel.SetName($"{NickName}");
            otherStatusPanel.SetHP(CurrentHealth, MaxHealth);
        }
    }

    public void OtherPanelHPUpdate()
    {
        if (otherStatusPanel)
        {
            otherStatusPanel.SetHP(CurrentHealth, MaxHealth);
        }
    }

    public void OtherPanelHpZero()
    {
        if (otherStatusPanel)
        {
            otherStatusPanel.SetHP(0, MaxHealth);
            otherStatusPanel.SetLife(0);
        }

        if (healthBar)
        {
            healthBar.fillAmount = 0;
        }

        if (lifeUI)
        {
            lifeUI.SetLife(0);
        }
    }

    void ReadyStatusChanged()
    {
        if (lobbyUI) lobbyUI.SetReady(Player, isReady);
    }

    public void UpdateLobbyUI()
    {
        NickNameChanged();
        ClassChanged();
        ReadyStatusChanged();
        if (lobbyUI) lobbyUI.UpdateLeader();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetReadyStatus(bool _isReady)
    {
        isReady = _isReady;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_AllReadyAndStart()
    {
        BGMmanager.Instance.playBossBGM();
        try
        {
            FindAnyObjectByType<LobbyUIController>().gameObject.SetActive(false);
        }
        finally
        {

        }
    }
}
