using System.Collections;
using System.Collections.Generic;
using Fusion;
using System.Linq;
using UnityEngine;

public class BindSword : NetworkBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField]
    private NetworkMecanimAnimator _mechanimAnimator;
    [Networked] public bool P_Drop { get; set; } = false;
    [Networked] public bool P_Bind { get; set; } = false;
    [Networked] public bool P_Idle { get; set; } = false;
    [Networked] public bool IsApplyingBind { get; set; } = false;
    private Coroutine despawnCoroutine;
    private float life = 3;
    public float bindDuration = 3;
    public float drainAmount = 10;
    [HideInInspector]
    public float damagePerTick;
    [HideInInspector]
    public NetworkObject boss;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _mechanimAnimator = GetComponent<NetworkMecanimAnimator>();
        drainAmount = Runner.ActivePlayers.Count() switch
        {
            1 => 1.5f,
            2 => 3,
            3 => 7,
            _ => 1
        };
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Render()
    {
        if (_mechanimAnimator == null)
        {
            return;
        }
        _mechanimAnimator.Animator.SetBool("Drop", P_Drop);
        _mechanimAnimator.Animator.SetBool("Bind", P_Bind);
        _mechanimAnimator.Animator.SetBool("Idle", P_Idle);
        base.Render();
    }

    public override void FixedUpdateNetwork()
    {
        if (_rb == null)
        {
            return;
        }
        if (!P_Drop && _rb.velocity.y < -5)
        {
            P_Drop = true;
        }
        if (!P_Bind && !P_Drop && (_rb.gravityScale == 0))
        {
            despawnCoroutine ??= StartCoroutine(Despawn());
        }
        if (transform.position.y < -20 || transform.position.x > 30)
        {
            // Despawn immediately if out of bounds
            Runner.Despawn(Object);
        }
    }

    public IEnumerator ApplyHorizontalMovement()
    {
        P_Idle = true;
        CustomTickTimer duariotn = CustomTickTimer.CreateFromSeconds(Runner, 3);
        int extraMovementTick = 0;
        while (!duariotn.Expired(Runner))
        {
            if (_rb == null)
            {
                yield return new WaitForFixedUpdate();
            }
            if (P_Bind)
            {
                if (extraMovementTick >= 5)
                {
                    yield break;
                }
                extraMovementTick++;
            }
            _rb.MovePosition(_rb.position + new Vector2(0.7f, 0));
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator DrainLife(PlayerControllerNetworked player)
    {
        CustomTickTimer duration = CustomTickTimer.CreateFromSeconds(Runner, bindDuration);
        BossMonsterNetworked boss = this.boss.GetComponent<BossMonsterNetworked>();
        BossAttack.AttackData attackData = new()
        {
            damage = damagePerTick,
            knockbackDirection = Vector2.zero,
            isApplyKnockback = false
        };
        while (Runner != null && !duration.Expired(Runner)) /* null check */
        {
            yield return new WaitForFixedUpdate();
            try
            {
                boss.CurrentHealth = Mathf.Min(boss.CurrentHealth + drainAmount, boss.maxHealth);
                player.transform.position = new Vector3(transform.position.x, player.transform.position.y, player.transform.position.z);
                player.RPC_OnPlayerHit(
                    attackData
                );
            }
            catch
            {
                Runner.Despawn(Object);
            }
        }
        if (Runner != null)
        {
            Runner.Despawn(Object);
        }
    }

    IEnumerator Despawn()
    {
        CustomTickTimer lifeTimer = CustomTickTimer.CreateFromSeconds(Runner, life);
        while (Runner != null && !lifeTimer.Expired(Runner)) /* null check */
        {
            if (P_Bind)
            {
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
        if (Runner != null && lifeTimer.Expired(Runner)) /* null check */
        {
            Runner.Despawn(Object);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _rb.velocity = Vector2.zero;
            _rb.gravityScale = 0;
            P_Drop = false;
        }
        if (collision.gameObject.CompareTag("Player") && !P_Bind)
        {
            PlayerControllerNetworked player = collision.gameObject.GetComponent<PlayerControllerNetworked>();
            if (!player.IsBinded)
            {
                if (player.CharacterClass == (int)CharacterClassEnum.Tank)
                {
                    if (player.weapon.isDraw)
                    {
                        player.Skill();
                        Runner.Despawn(Object);
                        return;
                    }
                }
                P_Bind = true;
                player.RPC_ApplyBind(bindDuration);
                StartCoroutine(DrainLife(player));
            }
        }
    }

}
