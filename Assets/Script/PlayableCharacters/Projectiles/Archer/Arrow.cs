using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class Arrow : NetworkBehaviour
{

    private bool isFired = false;
    private Vector3 firePos;
    Base _base;
    NetworkRigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<NetworkRigidbody2D>();
        _base = gameObject.GetComponentInParent<Base>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        if (_base.projectileSpeed != 0.0f && !isFired)
        {
            isFired = true;
            firePos = transform.localPosition;
            firePos.y = 0.0f;
            if (transform.parent.localScale.x < 0)
            {
                // calculate with trigonometry in 2D
                float yVelocity = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.z) * _base.projectileSpeed;
                _rb.Rigidbody.velocity = new Vector2(-1 * _base.projectileSpeed, -1 * yVelocity);
            }
            else
            {
                float yVelocity = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.z) * _base.projectileSpeed;
                _rb.Rigidbody.velocity = new Vector2(_base.projectileSpeed, yVelocity);
            }

        }
        if (isFired)
        {
            Vector3 currentPos = transform.localPosition;
            currentPos.y = 0.0f;
            if (Vector3.Distance(firePos, currentPos) > _base.range)
            {
                isFired = false;
                Runner.Despawn(transform.parent.GetComponent<NetworkObject>());
            }
        }


    }



    void OnTriggerEnter2D(Collider2D other)
    {
        if (!HasStateAuthority) return;
        if (_base.projectileSpeed == 0.0f)
        {
            return;
        }
        if (other.gameObject.CompareTag("Boss"))
        {
            PlayerAttack.AttackData attackData = new()
            {
                damage = _base.damage,
                attackType = PlayerAttack.AttackType.ProjectileOrShield,
                hitPosition = other.ClosestPoint(transform.position)
            };
            BossMonsterNetworked boss = other.gameObject.GetComponent<BossMonsterNetworked>();
            boss.Rpc_OnBossHit(attackData);
            Runner.Despawn(transform.parent.GetComponent<NetworkObject>());
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            Runner.Despawn(transform.parent.GetComponent<NetworkObject>());
        }
    }
}
