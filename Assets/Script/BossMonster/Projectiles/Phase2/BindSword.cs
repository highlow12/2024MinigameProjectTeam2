using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BindSword : NetworkBehaviour
{
    private Rigidbody2D _rb;
    private NetworkMecanimAnimator _mechanimAnimator;
    private bool isDrop = false;
    private bool isBinded = false;
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
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void FixedUpdateNetwork()
    {
        if (!isDrop && _rb.velocity.y < -5)
        {
            isDrop = true;
            _mechanimAnimator.Animator.SetBool("Drop", true);
        }
        if (!isBinded && isDrop && _rb.gravityScale == 0)
        {
            despawnCoroutine ??= StartCoroutine(Despawn());
        }
    }

    IEnumerator DrainLife(PlayerControllerNetworked player)
    {
        CustomTickTimer duration = CustomTickTimer.CreateFromSeconds(Runner, bindDuration);
        BossMonsterNetworked boss = this.boss.GetComponent<BossMonsterNetworked>();
        while (!duration.Expired(Runner))
        {
            yield return new WaitForFixedUpdate();
            boss.CurrentHealth = Mathf.Min(boss.CurrentHealth + drainAmount, boss.maxHealth);
            player.RPC_OnPlayerHit(
                new BossAttack.AttackData
                {
                    damage = damagePerTick,
                    knockbackDirection = Vector2.zero,
                    isApplyKnockback = false
                }
            );
        }
        Runner.Despawn(Object);
    }

    IEnumerator Despawn()
    {
        CustomTickTimer lifeTimer = CustomTickTimer.CreateFromSeconds(Runner, life);
        while (Runner != null && !lifeTimer.Expired(Runner)) /* null check */
        {
            if (isBinded)
            {
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
        if (Runner != null) /* null check */
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
            _mechanimAnimator.Animator.SetBool("Drop", false);
        }
        if (collision.gameObject.CompareTag("Player") && !isBinded)
        {
            isBinded = true;
            _mechanimAnimator.Animator.SetBool("Bind", true);
            PlayerControllerNetworked player = collision.gameObject.GetComponent<PlayerControllerNetworked>();
            if (!player.IsBinded)
            {
                player.RPC_ApplyBind(bindDuration);
                StartCoroutine(DrainLife(player));
            }
        }
    }

}
