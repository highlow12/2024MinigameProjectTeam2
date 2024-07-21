using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : PoolAble
{
    public float projectileSpeed = 0.0f;
    public float damage;
    public float range;
    private bool isFired = false;
    private Vector3 firePos;
    Rigidbody2D _rb;
    Animator _anim;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    void Start()
    {

    }

    void Update()
    {
        // Release on sync by fusion after initial release
        if (projectileSpeed == 0.0f && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        if (projectileSpeed != 0.0f && !isFired)
        {
            isFired = true;
            firePos = transform.position;
            firePos.y = 0.0f;
        }
        if (isFired)
        {
            _anim.SetFloat("Velocity", _rb.velocity.magnitude);
            Vector3 currentPos = transform.position;
            currentPos.y = 0.0f;
            if (Vector3.Distance(firePos, currentPos) > range)
            {
                ReleaseObject();
            }
        }
        if (transform.localScale.x < 0)
        {
            _rb.AddForce(Vector2.left * projectileSpeed, ForceMode2D.Impulse);
        }
        else
        {
            _rb.AddForce(Vector2.right * projectileSpeed, ForceMode2D.Impulse);
        }



    }
    // Need to implement damage logic in boss script
    void OnTriggerEnter2D(Collider2D other)
    {
        if (projectileSpeed == 0.0f)
        {
            return;
        }
        if (other.gameObject.CompareTag("Boss"))
        {
            other.gameObject.GetComponent<BossMonsterNetworked>().CurrentHealth -= damage;
            ReleaseObject();
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            ReleaseObject();
        }
    }
}
