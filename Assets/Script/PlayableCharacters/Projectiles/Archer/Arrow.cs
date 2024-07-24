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
            firePos = transform.localPosition;
            firePos.y = 0.0f;
            if (transform.parent.localScale.x < 0)
            {
                // calculate with trigonometry in 2D
                float yVelocity = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.z) * _base.projectileSpeed;
                _rb.velocity = new Vector2(-1 * _base.projectileSpeed, -1 * yVelocity);
            }
            else
            {
                float yVelocity = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.z) * _base.projectileSpeed;
                _rb.velocity = new Vector2(_base.projectileSpeed, yVelocity);
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
    void OnTriggerEnter2D(Collider2D other)
    {
        if (_base.projectileSpeed == 0.0f)
        {
            return;
        }
        if (other.gameObject.CompareTag("Boss"))
        {
            isFired = false;
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            isFired = false;
            _base.ReleaseObject();
        }
    }
}
