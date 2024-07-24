using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Arrow : NetworkBehaviour
{

    private bool isFired = false;
    private Vector3 firePos;
    Base _base;
    Rigidbody2D _rb;
    Animator _anim;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = gameObject.GetComponentInParent<Animator>();
        _base = gameObject.GetComponentInParent<Base>();
    }

    void Start()
    {
    }

    void Update()
    {
        if (_base.projectileSpeed != 0.0f && !isFired)
        {
            isFired = true;
            _anim.SetInteger("ShotType", _base.shotType);
            _anim.SetTrigger("Shot");
            firePos = transform.localPosition;
            firePos.y = 0.0f;
            if (transform.parent.localScale.x < 0)
            {
                _rb.velocity = Vector2.left * _base.projectileSpeed;
            }
            else
            {
                _rb.velocity = Vector2.right * _base.projectileSpeed;
            }

        }
        if (isFired)
        {
            _anim.SetFloat("Velocity", _rb.velocity.magnitude);
            Vector3 currentPos = transform.localPosition;
            currentPos.y = 0.0f;
            if (Vector3.Distance(firePos, currentPos) > _base.range)
            {
                isFired = false;
                _base.ReleaseObject();
            }
        }


    }
    // Need to implement damage logic in boss script
    void OnTriggerEnter2D(Collider2D other)
    {
        if (_base.projectileSpeed == 0.0f)
        {
            return;
        }
        if (other.gameObject.CompareTag("Boss"))
        {
            other.gameObject.GetComponent<BossMonsterNetworked>().CurrentHealth -= _base.damage;
            isFired = false;
            _base.ReleaseObject();
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            isFired = false;
            _base.ReleaseObject();
        }
    }
}
