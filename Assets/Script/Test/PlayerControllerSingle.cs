using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerSingle : MonoBehaviour
{

    private Rigidbody2D _rb;
    private Animator _anim;
    private Collider2D _collider;
    public GameObject groundCheck;
    public Dictionary<string, float> skillList;
    public DurationIndicator durationIndicator;
    public BuffIndicator buffIndicator;
    public Image healthBar;
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
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        UpdateCharacterClass(characterClass);
    }

    private bool IsCheckGrounded()
    {
        // Raycast config 
        float maxDistance = 0.1f;
        Vector2 position = groundCheck.transform.position;
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
        healthBar.fillAmount = currentHealth / maxHealth;
        var direction = Input.GetAxis("Horizontal").CompareTo(0);

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
        if (isGrounded) // 지상에서 점프
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpPower);
            _anim.SetTrigger("Jump");
            currentJumpCount++;
        }
        else if (currentJumpCount > 0 && currentJumpCount < maxJumpCount) // N단 점프
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpPower * 0.8f); // 점프력 감소
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
            _anim.SetTrigger("Roll"); // 구르기 애니메이션
            StartCoroutine(GetInvincible(0.5f)); // 무적 0.5초
            _collider.excludeLayers = enemyLayer; // 보스 충돌 무시
            float cooldown = skillList["Roll"];
            float rollLength = 0.5f;
            durationIndicator.CreateDurationIndicator(cooldown, "Roll");  // 쿨다운 표시
            yield return new WaitForSeconds(rollLength);
            _collider.excludeLayers = 0;
            yield return new WaitForSeconds(cooldown - rollLength);
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
            _anim.SetTrigger("Block"); // 방패 애니메이션
            float cooldown = skillList["Parry"];
            float parryLength = 0.25f;
            durationIndicator.CreateDurationIndicator(0.25f, "Parry");
            durationIndicator.CreateDurationIndicator(cooldown, "ParryCD"); // 쿨다운 표시
            yield return new WaitForSeconds(parryLength);
            isParrying = false;
            yield return new WaitForSeconds(cooldown - parryLength);
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
        durationIndicator.CreateDurationIndicator(duration, "Invincible"); // 지속시간 표시
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }

    void UpdateCharacterClass(int characterClass)
    {
        CharacterClass.ChangeClass(characterClass, gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("BossAttack")) // 보스 공격에 맞았을 때
        {
            if (isParrying)
            {
                StartCoroutine(GetInvincible(2.0f)); // 2초간 무적
                _rb.AddForce(new Vector2(-50f * transform.localScale.x, 30), ForceMode2D.Impulse); // 넉백
            }
            else if (!isInvincible)
            {
                _anim.SetTrigger("Hurt"); // 피격 애니메이션
                // 디버프 추가 테스트
                buffIndicator.AddBuff(DeBuffTypes.Burn); // 화상 디버프 추가
                buffIndicator.AddBuff(DeBuffTypes.Blind); // 실명 디버프 추가
                buffIndicator.AddBuff(DeBuffTypes.Undead); // 언데드 디버프 추가
                currentHealth -= col.gameObject.GetComponent<BossAttack>().damage; // 데미지
            }

        }
        // 벽 충돌 관련 기능들은 로직 수정 필요 함
        if (col.gameObject.CompareTag("Wall"))
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y); // 벽에 부딪히면 속도 0
            _rb.AddForce(new Vector2(-25f * transform.localScale.x, 30), ForceMode2D.Impulse); // 넉백
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // 벽 충돌 관련 기능들은 로직 수정 필요 함
        if (col.gameObject.CompareTag("Wall"))
        {
            _rb.AddForce(new Vector2(0, -10), ForceMode2D.Impulse); // 벽에 붙어있으면 중력으로 떨어짐
        }
    }



}