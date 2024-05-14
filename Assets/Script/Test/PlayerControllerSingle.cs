using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerSingle : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _anim;
    private Collider2D _collider;
    private Image _healthBar;
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    public bool isGrounded;
    public bool isRollCoolDown;
    public bool isParryCoolDown;
    public bool isParrying;
    public bool isInvincible;
    public float moveSpeed = 3.0f;
    public float attackSpeed = 1.0f;
    public float maxHealth = 100.0f;
    public float currentHealth;
    public float jumpPower = 5.0f;
    public int characterClass = 0;
    public int maxJumpCount = 2;
    public int currentJumpCount = 1;
    public Items.Weapon weapon;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _healthBar = GameObject.FindWithTag("CharacterHealthUI").GetComponent<Image>();
        UpdateCharacterClass(characterClass);
    }

    private bool IsCheckGrounded()
    {
        // Raycast config 
        float maxDistance = 0.1f;
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        // Raycast
        RaycastHit2D hit = Physics2D.Raycast(position, direction, maxDistance, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }
        // Debug용 Raycast
        Debug.DrawRay(position, direction * maxDistance, Color.red);
        return false;


    }


    private void Update()
    {
        isGrounded = IsCheckGrounded();
        if (isGrounded)
        {
            currentJumpCount = 1;
        }
        _anim.SetBool("Grounded", isGrounded);
        _anim.SetFloat("AirSpeedY", _rb.velocity.y);
        _healthBar.fillAmount = currentHealth / maxHealth;
        var direction = -Input.GetAxis("Horizontal").CompareTo(0);

        if (direction != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = direction;
            transform.localScale = scale;
            _anim.SetInteger("AnimState", 1);
            _rb.velocity = new Vector2(direction * moveSpeed, _rb.velocity.y);

        }
        else
        {
            _anim.SetInteger("AnimState", 0);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(weapon.Attack(_anim, gameObject.transform));
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Jump());
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Roll());
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartCoroutine(Parry());
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            switch (characterClass)
            {
                case (int)CharacterClass.CharacterClassEnum.Warrior:
                    UpdateCharacterClass((int)CharacterClass.CharacterClassEnum.Archer);
                    break;
                case (int)CharacterClass.CharacterClassEnum.Archer:
                    UpdateCharacterClass((int)CharacterClass.CharacterClassEnum.Tank);
                    break;
                case (int)CharacterClass.CharacterClassEnum.Tank:
                    UpdateCharacterClass((int)CharacterClass.CharacterClassEnum.Warrior);
                    break;
            }
            Debug.Log($"CharacterClass: {characterClass}, moveSpeed: {moveSpeed}, attackSpeed: {attackSpeed}, maxHealth: {maxHealth}, weapon: {weapon}");
        }
    }

    IEnumerator Jump()
    {
        if (currentJumpCount < maxJumpCount)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpPower);
            _anim.SetTrigger("Jump");
            currentJumpCount++;

        }
        yield return null;
    }

    IEnumerator Roll()
    {
        if (isGrounded && !isRollCoolDown)
        {
            isRollCoolDown = true;
            _anim.SetTrigger("Roll");
            isInvincible = true;
            _collider.excludeLayers = enemyLayer;
            yield return new WaitForSeconds(0.5f);
            _collider.excludeLayers = 0;
            isInvincible = false;
            yield return new WaitForSeconds(3.5f);
            isRollCoolDown = false;
        }
        yield return null;
    }

    IEnumerator Parry()
    {
        if (!isParryCoolDown)
        {
            isParryCoolDown = true;
            isParrying = true;
            _anim.SetTrigger("Block");
            yield return new WaitForSeconds(0.25f);
            isParrying = false;
            yield return new WaitForSeconds(1.0f);
            isParryCoolDown = false;
        }
        else
        {
            yield return null;
        }
    }

    IEnumerator GetInvincible()
    {
        isInvincible = true;
        yield return new WaitForSeconds(2.0f);
        isInvincible = false;
    }

    void UpdateCharacterClass(int characterClass)
    {
        CharacterClass.ChangeClass(characterClass, gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("BossAttack"))
        {
            if (isParrying)
            {
                StartCoroutine(GetInvincible());
                _rb.AddForce(new Vector2(-50f * transform.localScale.x, 30), ForceMode2D.Impulse);
            }
            else if (!isInvincible)
            {
                _anim.SetTrigger("Hurt");
                currentHealth -= col.gameObject.GetComponent<BossAttack>().damage;
            }

        }
    }
}