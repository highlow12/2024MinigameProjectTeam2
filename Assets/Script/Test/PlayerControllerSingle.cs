using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerSingle : MonoBehaviour
{
    private GameObject _durationUI;
    private GameObject _durationIndicator;
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
    public int currentJumpCount = 0;
    public Items.Weapon weapon;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        _durationIndicator = Resources.Load<GameObject>("Duration");
        _durationUI = GameObject.FindWithTag("DurationUI");
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
        if (isGrounded && _rb.velocity.y == 0)
        {
            currentJumpCount = 0;
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
            int newCharacterClass = characterClass + 1;
            if (newCharacterClass > 2)
            {
                newCharacterClass = 0;
            }
            UpdateCharacterClass(newCharacterClass);
            Debug.Log($"CharacterClass: {characterClass}, moveSpeed: {moveSpeed}, attackSpeed: {attackSpeed}, maxHealth: {maxHealth}, weapon: {weapon}");
        }
    }

    IEnumerator Jump()
    {
        if (isGrounded)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpPower);
            _anim.SetTrigger("Jump");
            currentJumpCount++;
        }
        else if (currentJumpCount > 0 && currentJumpCount < maxJumpCount)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpPower * 0.8f);
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
            StartCoroutine(GetInvincible(0.5f));
            _collider.excludeLayers = enemyLayer;
            CreateDurationIndicator(4f, "Roll");
            yield return new WaitForSeconds(0.5f);
            _collider.excludeLayers = 0;
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
            CreateDurationIndicator(2.25f, "Parry");
            yield return new WaitForSeconds(0.25f);
            isParrying = false;
            yield return new WaitForSeconds(2f);
            isParryCoolDown = false;
        }
        else
        {
            yield return null;
        }
    }

    IEnumerator GetInvincible(float duration)
    {
        if (isInvincible)
        {
            yield break;
        }
        isInvincible = true;
        CreateDurationIndicator(duration, "Invincible");
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }

    void CreateDurationIndicator(float maxDuration, string name = "")
    {
        GameObject durationIndicator = Instantiate(_durationIndicator, _durationUI.transform);
        var indicatorComponent = durationIndicator.GetComponent<DurationIndicator>();
        indicatorComponent.maxDuration = maxDuration;
        indicatorComponent.skillName = name;

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
                StartCoroutine(GetInvincible(2.0f));
                _rb.AddForce(new Vector2(-50f * transform.localScale.x, 30), ForceMode2D.Impulse);
            }
            else if (!isInvincible)
            {
                _anim.SetTrigger("Hurt");
                currentHealth -= col.gameObject.GetComponent<BossAttack>().damage;
            }

        }
        if (col.gameObject.CompareTag("Wall"))
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
            _rb.AddForce(new Vector2(-25f * transform.localScale.x, 30), ForceMode2D.Impulse);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            _rb.AddForce(new Vector2(0, -10), ForceMode2D.Impulse);
        }
    }

}