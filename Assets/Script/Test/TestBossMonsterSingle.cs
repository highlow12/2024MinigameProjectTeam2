using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TestBossMonsterSingle : MonoBehaviour
{
    private Rigidbody2D _rb;
    private List<GameObject> effects = new();
    public GameObject effectPool;
    public CameraMovement cameraMovement;
    public Image healthBar;
    public DurationIndicator durationIndicator;
    public Collider2D attackRange;
    public GameObject attackRangeIndicator;
    public GameObject followTarget;
    public float maxHealth = 1000.0f;
    public float currentHealth = 1000.0f;
    public float bossScale = 3.0f;
    public Animator animator;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        effects = effectPool.GetComponent<EffectPool>().effects;
        StartCoroutine(SetTarget());
        StartCoroutine(Attack());
        StartCoroutine(Move());
        transform.localScale = new Vector3(bossScale, bossScale, 1);
    }

    void Update()
    {
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
        // TODO: Need to implement object pooling for effects
        GameObject hitEffect = effects.Find(e => e.name == "blood");
        ParticleSystem instaEffect = Instantiate(hitEffect, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        instaEffect.Play();
        Destroy(instaEffect, instaEffect.main.duration);
        yield return null;
    }


    public IEnumerator Attack()
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
        yield return null;
    }

    public IEnumerator JumpDashAttack()
    {
        cameraMovement.isBossJumping = true;
        _rb.velocity = Vector2.zero;
        _rb.mass = 1.0f;
        Vector2 targetPos = followTarget.transform.position;
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        float jumpForce = 10.0f;
        float dashForce = 10.0f;
        float dashTime = 0.5f;
        float jumpTime = 0.8f;
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        yield return new WaitForSecondsRealtime(jumpTime);
        _rb.AddForce(direction * dashForce, ForceMode2D.Impulse);
        yield return new WaitForSecondsRealtime(dashTime);
        _rb.mass = 100000.0f;
        cameraMovement.isBossJumping = false;
        yield return null;
    }

    public IEnumerator Move()
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
            transform.position = Vector2.Lerp(transform.position, targetPos, 0.01f);

            distance = transform.position.x - followTarget.transform.position.x;
            yield return null;
        }
        animator.SetInteger("walkState", 0);
        yield return null;
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
