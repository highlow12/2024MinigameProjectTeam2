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
    public float bindDuration = 3;
    public float drainAmount = 10;
    public float damagePerTick = 1.5f;
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
            Runner.Despawn(Object);
        }
    }

    IEnumerator DrainLife(PlayerControllerNetworked player)
    {
        CustomTickTimer life = CustomTickTimer.CreateFromSeconds(Runner, bindDuration);
        // 왜 NullReferenceException이 발생하지?
        // 근데 왜 작동하지?
        BossMonsterNetworked boss = this.boss.GetComponent<BossMonsterNetworked>();
        while (!life.Expired(Runner))
        {
            yield return new WaitForFixedUpdate();
            boss.CurrentHealth += drainAmount;
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
