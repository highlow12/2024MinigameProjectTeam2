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
    

    #region
    [SerializeField] private LayerMask _groundLayer;

    [Space]

    [SerializeField] float _speed = 10f;
    [SerializeField] float _jumpForce = 10f;
    [SerializeField] float _maxVelocity = 8f;

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

    float _jumpBufferThreshold = .2f;
    float _jumpBufferTime;

    
    float CoyoteTimeThreshold = .1f;
    float TimeLeftGrounded;
    bool CoyoteTimeCD;
    bool WasGrounded;

    [SerializeField]
    Vector3 Velocity;
    [SerializeField]
    Vector3 GroundcheckPosition;
    #endregion


    // Start is called before the first frame update
    void Awake()
    {
        _rb = gameObject.GetComponent<NetworkRigidbody2D>();
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
        if (GetInput<PlayerInputData>(out var input))
        {
            input.direction.Normalize();
            UpdateMovement(input.direction.x);
            //Debug.Log(input.direction.x);

            Jump(input.buttons.IsSet(PlayerInputData.JUMP));

            BetterJumpLogic(input.buttons.IsSet(PlayerInputData.JUMP));
        }
    }
    void UpdateMovement(float input)
    {
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
            
                if (!IsGrounded && jump)
                {
                    _jumpBufferTime = Runner.SimulationTime;
                }

                if (IsGrounded || CalculateCoyoteTime())
                {
                    _rb.Rigidbody.velocity *= Vector2.right; //Reset y Velocity
                    _rb.Rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
                    CoyoteTimeCD = true;
                    
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
}
