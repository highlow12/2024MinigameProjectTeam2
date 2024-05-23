using Fusion;
using Fusion.Addons.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.PointerEventData;
using UnityEngine.EventSystems;

public class PlayerMovementRigidbodyNetwork : NetworkBehaviour
{
    NetworkRigidbody2D _rb;
    PlayerInputConsumer _input;

    #region
    [SerializeField] private LayerMask _groundLayer;

    [Space]

    [SerializeField] float _speed = 10f;
    [SerializeField] float _jumpForce = 10f;
    [SerializeField] float _DoubleJumpForce = 8f;
    [SerializeField] float _maxVelocity = 8f;

    [SerializeField] float _dashDuration = 0.5f; // 대쉬 지속 시간
    [SerializeField] float _dashDistance = 3.0f; // 대쉬 거리

    [Space]

    [SerializeField] private float fallMultiplier = 3.3f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Space]

    [SerializeField] Vector2 _groundHorizontalDragVector = new Vector2(.1f, 1);
    [SerializeField] Vector2 _airHorizontalDragVector = new Vector2(.98f, 1);
    [SerializeField] Vector2 _horizontalSpeedReduceVector = new Vector2(.95f, 1);
    [SerializeField] Vector2 _verticalSpeedReduceVector = new Vector2(1, .95f);

    [Space]

    bool IsGrounded = false;
    bool _isDashing = false;

    float _jumpBufferThreshold = .2f;
    float _jumpBufferTime;

    float _rollBufferThreshold = .2f;
    float _rollBufferTime;

    
    float CoyoteTimeThreshold = .1f;
    float TimeLeftGrounded;
    bool CoyoteTimeCD;
    bool WasGrounded;
    bool hasDoubleJumped;

    [SerializeField]
    Vector3 Velocity;
    [SerializeField]
    Vector3 GroundcheckPosition;
    #endregion


    // Start is called before the first frame update
    void Awake()
    {
        _rb = gameObject.GetComponent<NetworkRigidbody2D>();
        _input = gameObject.GetComponent<PlayerInputConsumer>();
    }

    // Update is called once per frame
    void DetectGroundAndWalls()
    {
        WasGrounded = IsGrounded;
        IsGrounded = default;
        
        IsGrounded = (bool)Runner.GetPhysicsScene2D().OverlapBox((Vector2)transform.position + (Vector2)GroundcheckPosition, Vector2.one * .85f, 0, _groundLayer);
        if (IsGrounded)
        {
            CoyoteTimeCD = false;
            hasDoubleJumped = false;
            return;
        }

        if (WasGrounded)
        {
            if (CoyoteTimeCD)
            {
                CoyoteTimeCD = false;
            }
            else
            {
                TimeLeftGrounded = Runner.SimulationTime;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        inputTask();

        Velocity = _rb.Rigidbody.velocity;
    }
    void inputTask()
    {
        var dir = _input.dir.normalized;
        UpdateMovement(dir.x);
        Jump(_input.pressed.IsSet(PlayerButtons.Jump));
        BetterJumpLogic(_input.pressed.IsSet(PlayerButtons.Jump));
        Roll(_input.pressed.IsSet(PlayerButtons.Roll));
        /*if (GetInput<PlayerInputData>(out var input))
        {
            input.direction.Normalize();
            
            UpdateMovement(input.direction.x);
            //Debug.Log(input.direction.x);

            Jump(input.buttons.IsSet(PlayerInputData.JUMP));

            BetterJumpLogic(input.buttons.IsSet(PlayerInputData.JUMP));

            Roll(input.buttons.IsSet(PlayerInputData.DASH));
            
        }*/
    }
    void UpdateMovement(float input)
    {
        if (_isDashing) return;

        DetectGroundAndWalls();

        if (input<0)
        {
            //Reset x velocity if start moving in oposite direction.
            if (_rb.Rigidbody.velocity.x > 0 && IsGrounded)
            {
                _rb.Rigidbody.velocity *= Vector2.up;
            }
            _rb.Rigidbody.AddForce(_speed * Vector2.left, ForceMode2D.Force);
        }
        else if (input>0)
        {
            //Reset x velocity if start moving in oposite direction.
            if (_rb.Rigidbody.velocity.x < 0 && IsGrounded)
            {
                _rb.Rigidbody.velocity *= Vector2.up;
            }
            _rb.Rigidbody.AddForce(_speed * Vector2.right, ForceMode2D.Force);
        }
        else
        {
            //Different horizontal drags depending if grounded or not.
            if (IsGrounded)
                _rb.Rigidbody.velocity *= _groundHorizontalDragVector;
            else
                _rb.Rigidbody.velocity *= _airHorizontalDragVector;
        }

        LimitSpeed();
    }

    private void LimitSpeed()
    {
        //Limit horizontal velocity
        if (Mathf.Abs(_rb.Rigidbody.velocity.x) > _maxVelocity)
        {
            _rb.Rigidbody.velocity *= _horizontalSpeedReduceVector;
        }

        if (Mathf.Abs(_rb.Rigidbody.velocity.y) > _maxVelocity * 2)
        {
            _rb.Rigidbody.velocity *= _verticalSpeedReduceVector;
        }
    }

    
    private void Jump(bool jump)
    {

        //Jump
        if (jump|| CalculateJumpBuffer())
        {
            void _jump(float __jumpForce)
            {
                _rb.Rigidbody.velocity *= Vector2.right; //Reset y Velocity
                _rb.Rigidbody.AddForce(Vector2.up * __jumpForce, ForceMode2D.Impulse);
                CoyoteTimeCD = true;

            }
            void advanced_jump(float jumpHeight, float timeToApex)
            {
                float gravity = (2 * jumpHeight) / Mathf.Pow(timeToApex, 2);
                float initialJumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);

                _rb.Rigidbody.velocity = new Vector2(_rb.Rigidbody.velocity.x, initialJumpVelocity);
                Physics.gravity = new Vector3(0, -gravity, 0);
                CoyoteTimeCD = true;
            }
            if (!IsGrounded && jump)
                {
                    if (!hasDoubleJumped)
                    {
                        _jump(_DoubleJumpForce);
                        hasDoubleJumped = true;
                    }

                    _jumpBufferTime = Runner.SimulationTime;
                }

                if (IsGrounded || CalculateCoyoteTime())
                {
                //_jump(_jumpForce);
                advanced_jump(10, 0.5f);
                }
                
            
        }
    }

    private bool CalculateJumpBuffer()
    {
        return (Runner.SimulationTime <= _jumpBufferTime + _jumpBufferThreshold) && IsGrounded;
    }

    private bool CalculateCoyoteTime()
    {
        return (Runner.SimulationTime <= TimeLeftGrounded + CoyoteTimeThreshold);
    }

    private void BetterJumpLogic(bool input)
    {
        if (IsGrounded) { return; }
        if (_rb.Rigidbody.velocity.y < 0)
        {
            if (_rb.Rigidbody.velocity.y > 0 && !input)
            {
                _rb.Rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Runner.DeltaTime;
            }
            else
            {
                _rb.Rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Runner.DeltaTime;
            }
        }
        
    }

    private void Roll(bool dash)
    {if (dash || CalculateRollBuffer())
        {
            if (_isDashing || !IsGrounded && dash)
            {
                _rollBufferTime = Runner.SimulationTime;
            }

            if (IsGrounded && (dash) && !_isDashing) // IsGrounded는 플레이어가 땅에 있는지 확인하는 메서드라고 가정
            {
                StartCoroutine("DashCoroutine");
            }
        }
    }

    private bool CalculateRollBuffer()
    {
        return (Runner.SimulationTime <= _rollBufferTime + _rollBufferThreshold) && IsGrounded;
    }

    private IEnumerator DashCoroutine()
    {
        _isDashing = true;

        Vector2 dashDirection = _rb.Rigidbody.velocity.normalized; // 오브젝트가 보고 있는 방향 (오른쪽)
        _rb.Rigidbody.velocity = dashDirection * Vector2.right * _dashDistance / _dashDuration;

        yield return new WaitForSeconds(_dashDuration);

        _rb.Rigidbody.velocity = Vector2.zero; // 대쉬 후 속도 초기화
        _isDashing = false;
    }
}
