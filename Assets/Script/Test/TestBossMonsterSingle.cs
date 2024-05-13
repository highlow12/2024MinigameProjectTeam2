using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBossMonsterSingle : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(Attack());
        StartCoroutine(Move());
    }

    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "AttackObject")
        {
            Destroy(col.gameObject);
            StartCoroutine(Hit());
        }
    }

    IEnumerator Hit()
    {
        animator.SetBool("isHit", true);
        yield return new WaitForSeconds(0.417f);
        animator.SetBool("isHit", false);
    }

    IEnumerator JumpAttack()
    {
        animator.SetBool("doJumpAttack", true);
        yield return new WaitForSeconds(0.417f);
        animator.SetBool("doJumpAttack", false);
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        var attackCooldown = Random.Range(1.0f, 3.0f);
        yield return new WaitForSeconds(attackCooldown);
        StartCoroutine(JumpAttack());
    }

    IEnumerator Move()
    {
        var moveCooldown = Random.Range(1.0f, 5.0f);
        yield return new WaitForSeconds(moveCooldown);
        StartCoroutine(Move());
    }
}
