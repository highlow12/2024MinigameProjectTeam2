using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TestBossMonsterSingle : MonoBehaviour
{
    private Rigidbody2D _rb;
    public Image healthBar;
    public DurationIndicator durationIndicator;
    public Collider2D attackRange;
    public GameObject attackRangeIndicator;
    public GameObject followTarget;
    public float maxHealth = 1000.0f;
    public float currentHealth = 1000.0f;
    public float bossScale = 3.0f;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        StartCoroutine(SetTarget());
        StartCoroutine(Attack());
        StartCoroutine(Move());
        transform.localScale = new Vector3(bossScale, bossScale, 1);
    }

    void Update()
    {
        healthBar.fillAmount = currentHealth / maxHealth;
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
        float omenLength = 0.4f;
        float attackLength = 0.3f;
        durationIndicator.CreateDurationIndicator(omenLength, "OmenDuration");
        yield return new WaitForSecondsRealtime(omenLength);
        attackRange.enabled = true;
        yield return new WaitForSecondsRealtime(attackLength);
        attackRange.enabled = false;
        attackRangeIndicator.SetActive(false);
        var attackCooldown = Random.Range(2.0f, 4.0f);
        Debug.Log($"Move after {attackCooldown} seconds");
        durationIndicator.CreateDurationIndicator(attackCooldown, $"MoveAfter{attackCooldown}");
        yield return new WaitForSecondsRealtime(attackCooldown);
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        if (followTarget == null)
        {
            yield return new WaitForSecondsRealtime(1.0f);
            StartCoroutine(Move());
        }
        float distance = transform.position.x - followTarget.transform.position.x;
        Vector3 scale = transform.localScale;
        if (distance > 0)
        {
            scale.x = -bossScale;
            transform.localScale = scale;
        }
        else
        {
            scale.x = bossScale;
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
        Debug.Log("Attack after " + moveCooldown + " seconds");
        durationIndicator.CreateDurationIndicator(moveCooldown, $"AttackAfter{moveCooldown}");
        yield return new WaitForSecondsRealtime(moveCooldown);
        StartCoroutine(Attack());
    }

    IEnumerator SetTarget()
    {
        yield return new WaitForSecondsRealtime(1.0f);
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
