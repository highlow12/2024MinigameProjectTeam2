using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerSingle : MonoBehaviour
{
    private Rigidbody2D _rb;
    public LayerMask groundLayer;
    private Vector3 _forward;
    public bool isGrounded;
    public float moveSpeed = 3.0f;
    public float attackSpeed = 1.0f;
    public float maxHealth = 100.0f;
    public float currentHealth;
    public float jumpPower = 5.0f;
    public int characterClass = 0;
    public Items.Weapon weapon;
    private void Awake()
    {
        _forward = transform.up;
        _rb = GetComponent<Rigidbody2D>();
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
        var direction = new Vector2(-Input.GetAxis("Horizontal"), 0);
        direction.Normalize();
        _rb.velocity = new Vector2(direction.x * moveSpeed, _rb.velocity.y);
        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = direction.x;
            transform.localScale = scale;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(weapon.Attack());
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (characterClass == (int)CharacterClass.CharacterClassEnum.Warrior)
            {
                UpdateCharacterClass((int)CharacterClass.CharacterClassEnum.Archer);
            }
            else if (characterClass == (int)CharacterClass.CharacterClassEnum.Archer)
            {
                UpdateCharacterClass((int)CharacterClass.CharacterClassEnum.Tank);
            }
            else if (characterClass == (int)CharacterClass.CharacterClassEnum.Tank)
            {
                UpdateCharacterClass((int)CharacterClass.CharacterClassEnum.Warrior);
            }
            Debug.Log($"CharacterClass: {characterClass}, moveSpeed: {moveSpeed}, attackSpeed: {attackSpeed}, maxHealth: {maxHealth}, weapon: {weapon}");
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpPower);
        }
    }

    void UpdateCharacterClass(int characterClass)
    {
        CharacterClass.ChangeClass(characterClass, gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
    }
}