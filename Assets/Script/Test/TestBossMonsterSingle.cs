using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TestBossMonsterSingle : MonoBehaviour
{
    private Rigidbody2D _rb;
    public Collider2D attackRange;
    public GameObject attackRangeIndicator;
    public GameObject followTarget;
    private Image _healthBar;
    private Image _parryTimeIndicator;
    public float maxHealth = 1000.0f;
    public float currentHealth = 1000.0f;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        _healthBar = GameObject.FindWithTag("BossHealthUI").GetComponent<Image>();
        _parryTimeIndicator = GameObject.FindWithTag("ParryTimeUI").GetComponent<Image>();
        attackRange = transform.GetChild(0).gameObject.GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        attackRangeIndicator = transform.GetChild(1).gameObject;
        StartCoroutine(SetTarget());
        StartCoroutine(Attack());
        StartCoroutine(Move());
    }

    void Update()
    {
        _healthBar.fillAmount = currentHealth / maxHealth;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("MeleeAttack"))
        {
            currentHealth -= col.gameObject.GetComponent<Attack>().damage;
            StartCoroutine(Hit());
        }
    }

    IEnumerator Hit()
    {
        animator.SetTrigger("isHit");
        yield return null;
    }

    IEnumerator JumpAttack()
    {
        animator.SetTrigger("doJumpAttack");
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        animator.SetTrigger("doAttack");
        attackRangeIndicator.SetActive(true);
        float parryTime = 0f;
        _parryTimeIndicator.fillAmount = 0f;
        while (_parryTimeIndicator.fillAmount < 1.0f)
        {
            parryTime += Time.deltaTime;
            _parryTimeIndicator.fillAmount = parryTime / 0.4f;
            yield return null;
        }
        attackRange.enabled = true;
        yield return new WaitForSeconds(0.3f);
        attackRange.enabled = false;
        attackRangeIndicator.SetActive(false);
        _parryTimeIndicator.fillAmount = 0.0f;
        var attackCooldown = Random.Range(2.0f, 4.0f);
        yield return new WaitForSeconds(attackCooldown);
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        if (followTarget == null)
        {
            yield return new WaitForSeconds(1.0f);
            StartCoroutine(Move());
        }
        float distance = transform.position.x - followTarget.transform.position.x;
        Vector3 scale = transform.localScale;
        if (distance > 0)
        {
            scale.x = -3;
            transform.localScale = scale;
        }
        else
        {
            scale.x = 3;
            transform.localScale = scale;
        }
        animator.SetInteger("walkState", 1);
        while (Math.Abs(distance) > 2.0f)
        {
            Vector3 targetPos = followTarget.transform.position;
            targetPos.y = transform.position.y;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, 0.01f);

            distance = transform.position.x - followTarget.transform.position.x;
            yield return null;
        }
        animator.SetInteger("walkState", 0);
        var moveCooldown = Random.Range(1.0f, 1.5f);
        yield return new WaitForSeconds(moveCooldown);
        StartCoroutine(Attack());
    }

    IEnumerator SetTarget()
    {
        yield return new WaitForSeconds(1.0f);
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            foreach (var player in players)
            {
                if (followTarget == null)
                {
                    followTarget = player;
                }
                else
                {
                    if (Vector2.Distance(transform.position, player.transform.position) < Vector2.Distance(transform.position, followTarget.transform.position))
                    {
                        followTarget = player;
                    }
                }
            }
        }
        StartCoroutine(SetTarget());
    }
}
