/*
	Created by @DawnosaurDev at youtube.com/c/DawnosaurStudios
	Thanks so much for checking this out and I hope you find it helpful! 
	If you have any further queries, questions or feedback feel free to reach out on my twitter or leave a comment on youtube :D

	Feel free to use this in your own games, and I'd love to see anything you make!
 */

using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("중력")]
    [Tooltip("점프 높이와 점프 시간을 기반으로 하는 원하는 중력(아래로의 힘).")]
    [HideInInspector] public float gravityStrength; // 점프 높이와 점프 시간을 기반으로 하는 원하는 중력(아래로의 힘).
    [Tooltip("플레이어의 중력 강도를 중력의 배수로 설정합니다.\n(ProjectSettings/Physics2D에서 설정).\n플레이어의 rigidbody2D.gravityScale에 설정되는 값입니다.")]
    [HideInInspector] public float gravityScale; // 플레이어의 중력 강도를 중력의 배수로 설정합니다(ProjectSettings/Physics2D에서 설정). 또한 플레이어의 rigidbody2D.gravityScale에 설정되는 값입니다.
    [Space(5)]
    [Tooltip("떨어지는 동안 플레이어의 중력 배수입니다.")]
    public float fallGravityMult; // 떨어지는 동안 플레이어 중력 배수입니다.
    [Tooltip("떨어질 때 플레이어의 최대 낙하 속도(최대 속력)입니다.")]
    public float maxFallSpeed; // 떨어질 때 플레이어의 최대 낙하 속도(최대 속력).
    [Space(5)]
    [Tooltip("플레이어가 떨어지고 아래 입력이 눌린 경우 플레이어 중력 배수의 큰 배수입니다.\nCeleste와 같은 게임에서 볼 수 있으며, 플레이어가 원한다면 추가로 빠르게 떨어질 수 있습니다.")]
    public float fastFallGravityMult; // 플레이어가 떨어지고 아래 입력이 눌린 경우 플레이어 중력 배수의 큰 배수입니다. Celeste와 같은 게임에서 볼 수 있으며, 플레이어가 원한다면 추가로 빠르게 떨어질 수 있습니다.
    [Tooltip("빠른 낙하를 수행할 때 플레이어의 최대 낙하 속도(최대 속력)입니다.")]
    public float maxFastFallSpeed; // 빠른 낙하를 수행할 때 플레이어의 최대 낙하 속도(최대 속력).

    [Space(20)]

    [Header("달리기")]
    [Tooltip("플레이어가 도달하기를 원하는 목표 속도입니다.")]
    public float runMaxSpeed; // 플레이어가 도달하기를 원하는 목표 속도입니다.
    [Tooltip("플레이어가 최대 속도에 가속하는 속도로,\nrunMaxSpeed로 설정하여 속도가 즉시 0으로 감소하도록 설정할 수 있습니다.")]
    public float runAcceleration; // 플레이어가 최대 속도에 가속하는 속도로, runMaxSpeed로 설정하여 속도가 즉시 0으로 감소하도록 설정할 수 있습니다.
    [HideInInspector] public float runAccelAmount; // 플레이어에 적용되는 실제 힘(speedDiff와 곱함).
    [Tooltip("현재 속도에서 플레이어가 감속하는 속도로,\nrunMaxSpeed로 설정하여 속도가 즉시 0으로 감소하도록 설정할 수 있습니다.")]
    public float runDecceleration; // 현재 속도에서 플레이어가 감속하는 속도로, runMaxSpeed로 설정하여 속도가 즉시 0으로 감소하도록 설정할 수 있습니다.
    [HideInInspector] public float runDeccelAmount; // 플레이어에 적용되는 실제 힘(speedDiff와 곱함).
    [Space(5)]
    [Tooltip("공중에서 가속도 비율에 적용되는 곱셈자입니다.")]
    [Range(0f, 1)] public float accelInAir; // 공중에서 가속도 비율에 적용되는 곱셈자입니다.
    [Tooltip("공중에서 감속도 비율에 적용되는 곱셈자입니다.")]
    [Range(0f, 1)] public float deccelInAir;
    [Space(5)]
    [Tooltip("모멘텀을 보존할지 여부를 결정합니다.")]
    public bool doConserveMomentum = true;

    [Space(20)]

    [Header("점프")]
    [Tooltip("플레이어의 점프 높이입니다.")]
    public float jumpHeight; // 플레이어의 점프 높이입니다.
    [Tooltip("점프 힘을 적용하고 원하는 점프 높이에 도달하는 시간입니다.\n이러한 값은 또한 플레이어의 중력 및 점프 힘을 제어합니다.")]
    public float jumpTimeToApex; // 점프 힘을 적용하고 원하는 점프 높이에 도달하는 시간입니다. 이러한 값은 또한 플레이어의 중력 및 점프 힘을 제어합니다.
    [HideInInspector] public float jumpForce; // 플레이어에게 적용되는 실제 힘(위쪽으로 점프할 때).

    [Header("두 종류의 점프")]
    [Tooltip("플레이어가 점프 버튼을 놓을 때 중력을 증가시키는 배수입니다.")]
    public float jumpCutGravityMult; // 플레이어가 점프 버튼을 놓을 때 중력을 증가시키는 배수입니다.
    [Tooltip("점프의 정점(원하는 최대 높이)에 가까울 때 중력을 줄이는 배수입니다.")]
    [Range(0f, 1)] public float jumpHangGravityMult; // 점프의 정점(원하는 최대 높이)에 가까울 때 중력을 줄이는 배수입니다.
    [Tooltip("플레이어가 추가로 '점프 행'을 경험하는 경우입니다.\n플레이어의 velocity.y는 점프 정점에서 가장 0에 가깝습니다(파라볼라 또는 이차 함수의 기울기를 생각해보세요).")]
    public float jumpHangTimeThreshold; // 플레이어가 추가로 '점프 행'을 경험하는 경우입니다. 플레이어의 velocity.y는 점프 정점에서 가장 0에 가깝습니다(파라볼라 또는 이차 함수의 기울기를 생각해보세요).
    [Space(0.5f)]
    [Tooltip("점프 정점에 가까울 때 가속도에 적용되는 배수입니다.")]
    public float jumpHangAccelerationMult;
    [Tooltip("점프 정점에 가까울 때 최대 속도에 적용되는 배수입니다.")]
    public float jumpHangMaxSpeedMult;

    [Header("벽 점프")]
    [Tooltip("플레이어가 벽 점프할 때 적용되는 실제 힘입니다.")]
    public Vector2 wallJumpForce; // 플레이어가 벽 점프할 때 적용되는 실제 힘입니다.
    [Space(5)]
    [Tooltip("벽을 타는 동안 플레이어의 움직임 효과를 줄입니다.")]
    [Range(0f, 1f)] public float wallJumpRunLerp; // 벽을 타는 동안 플레이어의 움직임 효과를 줄입니다.
    [Tooltip("벽 점프 후 일정 시간 동안 플레이어의 움직임을 느리게합니다.")]
    [Range(0f, 1.5f)] public float wallJumpTime; // 벽 점프 후 일정 시간 동안 플레이어의 움직임을 느리게합니다.
    [Tooltip("벽 점프 방향으로 플레이어가 회전합니다.")]
    public bool doTurnOnWallJump; // 벽 점프 방향으로 플레이어가 회전합니다.

    [Space(20)]

    [Header("슬라이드")]
    [Tooltip("슬라이드 속도입니다.")]
    public float slideSpeed;
    [Tooltip("슬라이드 가속도입니다.")]
    public float slideAccel;

    [Header("도움 기능")]
    [Tooltip("플랫폼에서 떨어진 후에도 점프할 수 있는 관대한 시간입니다.")]
    [Range(0.01f, 0.5f)] public float coyoteTime; // 플랫폼에서 떨어진 후에도 점프할 수 있는 관대한 시간
    [Tooltip("점프를 누른 후 요구 사항(예: 지면에 서 있는 것)이 충족되면 자동으로 점프되는 점프 버퍼 시간.")]
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime; // 점프를 누른 후 요구 사항(예: 지면에 서 있는 것)이 충족되면 자동으로 점프되는 점프 버퍼 시간.

    [Space(20)]

    [Header("대시")]
    [Tooltip("대시 가능 횟수입니다.")]
    public int dashAmount;
    [Tooltip("대시 속도입니다.")]
    public float dashSpeed;
    [Tooltip("대시를 누르고 방향 입력을 읽기 전에 게임이 얼마 동안 멈추는지에 대한 지속 시간입니다.")]
    public float dashSleepTime; // 대시를 누르고 방향 입력을 읽기 전에 게임이 얼마 동안 멈추는지에 대한 지속 시간
    [Space(5)]
    [Tooltip("대시 공격에 걸리는 시간입니다.")]
    public float dashAttackTime;
    [Space(5)]
    [Tooltip("초기 드래그 단계를 마친 후 표준 상태(또는 임의의 상태)로의 부드러운 전환을 위해 대시가 끝난 시간입니다.")]
    public float dashEndTime; // 초기 드래그 단계를 마친 후 표준 상태(또는 임의의 상태)로의 부드러운 전환을 위해 대시가 끝난 시간
    [Tooltip("플레이어를 느리게 만들어 대시 반응이 더 좋아지게 합니다.")]
    public Vector2 dashEndSpeed; // 플레이어를 느리게 만들어 대시 반응이 더 좋아지게 합니다(세레스테에서 사용됨).
    [Tooltip("대시 중 플레이어 움직임의 영향을 줄입니다.")]
    [Range(0f, 1f)] public float dashEndRunLerp; // 대시 중 플레이어 움직임의 영향을 줄입니다.
    [Space(5)]
    [Tooltip("대시 가능 횟수를 다시 채우는 데 걸리는 시간입니다.")]
    public float dashRefillTime;
    [Space(5)]
    [Tooltip("대시 입력 버퍼 시간입니다.")]
    [Range(0.01f, 0.5f)] public float dashInputBufferTime;

    // Unity 콜백, Inspector가 업데이트될 때 호출됩니다.
    private void OnValidate()
    {
        // 중력 강도 계산에 사용되는 공식(중력 = 2 * jumpHeight / timeToJumpApex^2)을 사용하여 중력 강도를 계산합니다.
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

        // rigidbody의 중력 스케일을 계산합니다(즉, unity의 중력 값에 대한 중력 강도입니다, project settings/Physics2D 참조).
        gravityScale = gravityStrength / Physics2D.gravity.y;

        // 실행 가속도 및 감속도 힘을 계산합니다. (즉시 가속도를 0으로 설정하려면 runMaxSpeed로 설정하세요).
        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        // 점프 힘 계산에 사용되는 공식(초기 점프 속도 = 중력 * timeToJumpApex)을 사용하여 점프 힘을 계산합니다.
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        #region 변수 범위
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion
    }

    #region COMPONENTS
    public Rigidbody2D RB { get; private set; }
	//Script to handle all player animations, all references can be safely removed if you're importing into your own project.
	//public PlayerAnimator //AnimHandler { get; private set; }
	#endregion

	#region STATE PARAMETERS
	//Variables control the various actions the player can perform at any time.
	//These are fields which can are public allowing for other sctipts to read them
	//but can only be privately written to.
	public bool IsFacingRight { get; private set; }
	public bool IsJumping { get; private set; }
	public bool IsWallJumping { get; private set; }
	public bool IsDashing { get; private set; }
	public bool IsSliding { get; private set; }

	//Timers (also all fields, could be private and a method returning a bool could be used)
	public float LastOnGroundTime { get; private set; }
	public float LastOnWallTime { get; private set; }
	public float LastOnWallRightTime { get; private set; }
	public float LastOnWallLeftTime { get; private set; }

	//Jump
	private bool _isJumpCut;
	private bool _isJumpFalling;

	//Wall Jump
	private float _wallJumpStartTime;
	private int _lastWallJumpDir;

	//Dash
	private int _dashesLeft;
	private bool _dashRefilling;
	private Vector2 _lastDashDir;
	private bool _isDashAttacking;

	#endregion

	#region INPUT PARAMETERS
	private Vector2 _moveInput;

	public float LastPressedJumpTime { get; private set; }
	public float LastPressedDashTime { get; private set; }
	#endregion

	#region CHECK PARAMETERS
	//Set all of these up in the inspector
	[Header("Checks")] 
	[SerializeField] private Transform _groundCheckPoint;
	//Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
	[SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
	[Space(5)]
	[SerializeField] private Transform _frontWallCheckPoint;
	[SerializeField] private Transform _backWallCheckPoint;
	[SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
	[SerializeField] private LayerMask _groundLayer;
	#endregion

    private void Awake()
	{
		RB = GetComponent<Rigidbody2D>();
		//AnimHandler = GetComponent<PlayerAnimator>();
	}

	private void Start()
	{
		SetGravityScale( gravityScale);
		IsFacingRight = true;
	}

	private void Update()
	{
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
		LastOnWallTime -= Time.deltaTime;
		LastOnWallRightTime -= Time.deltaTime;
		LastOnWallLeftTime -= Time.deltaTime;

		LastPressedJumpTime -= Time.deltaTime;
		LastPressedDashTime -= Time.deltaTime;
		#endregion

		#region INPUT HANDLER
		_moveInput.x = Input.GetAxisRaw("Horizontal");
		_moveInput.y = Input.GetAxisRaw("Vertical");

		if (_moveInput.x != 0)
			CheckDirectionToFace(_moveInput.x > 0);

		if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J))
        {
			OnJumpInput();
        }

		if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.C) || Input.GetKeyUp(KeyCode.J))
		{
			OnJumpUpInput();
		}

		if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.K))
		{
			OnDashInput();
		}
		#endregion

		#region COLLISION CHECKS
		if (!IsDashing && !IsJumping)
		{
			//Ground Check
			if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer)) //checks if set box overlaps with ground
			{
				if(LastOnGroundTime < -0.1f)
                {
					//AnimHandler.justLanded = true;
                }

				LastOnGroundTime =  coyoteTime; //if so sets the lastGrounded to coyoteTime
            }		

			//Right Wall Check
			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
					|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping)
				LastOnWallRightTime =  coyoteTime;

			//Right Wall Check
			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
				|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping)
				LastOnWallLeftTime =  coyoteTime;

			//Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
			LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
		}
		#endregion

		#region JUMP CHECKS
		if (IsJumping && RB.velocity.y < 0)
		{
			IsJumping = false;

			_isJumpFalling = true;
		}

		if (IsWallJumping && Time.time - _wallJumpStartTime >  wallJumpTime)
		{
			IsWallJumping = false;
		}

		if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
			_isJumpCut = false;

			_isJumpFalling = false;
		}

		if (!IsDashing)
		{
			//Jump
			if (CanJump() && LastPressedJumpTime > 0)
			{
				IsJumping = true;
				IsWallJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;
				Jump();

				//AnimHandler.startedJumping = true;
			}
			//WALL JUMP
			else if (CanWallJump() && LastPressedJumpTime > 0)
			{
				IsWallJumping = true;
				IsJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;

				_wallJumpStartTime = Time.time;
				_lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

				WallJump(_lastWallJumpDir);
			}
		}
		#endregion

		#region DASH CHECKS
		if (CanDash() && LastPressedDashTime > 0)
		{
			//Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
			Sleep( dashSleepTime); 

			//If not direction pressed, dash forward
			if (_moveInput != Vector2.zero)
				_lastDashDir = _moveInput;
			else
				_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;



			IsDashing = true;
			IsJumping = false;
			IsWallJumping = false;
			_isJumpCut = false;

			StartCoroutine(nameof(StartDash), _lastDashDir);
		}
		#endregion

		#region SLIDE CHECKS
		if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
			IsSliding = true;
		else
			IsSliding = false;
		#endregion

		#region GRAVITY
		if (!_isDashAttacking)
		{
			//Higher gravity if we've released the jump input or are falling
			if (IsSliding)
			{
				SetGravityScale(0);
			}
			else if (RB.velocity.y < 0 && _moveInput.y < 0)
			{
				//Much higher gravity if holding down
				SetGravityScale( gravityScale *  fastFallGravityMult);
				//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, - maxFastFallSpeed));
			}
			else if (_isJumpCut)
			{
				//Higher gravity if jump button released
				SetGravityScale( gravityScale *  jumpCutGravityMult);
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, - maxFallSpeed));
			}
			else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) <  jumpHangTimeThreshold)
			{
				SetGravityScale( gravityScale *  jumpHangGravityMult);
			}
			else if (RB.velocity.y < 0)
			{
				//Higher gravity if falling
				SetGravityScale( gravityScale *  fallGravityMult);
				//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, - maxFallSpeed));
			}
			else
			{
				//Default gravity if standing on a platform or moving upwards
				SetGravityScale( gravityScale);
			}
		}
		else
		{
			//No gravity when dashing (returns to normal once initial dashAttack phase over)
			SetGravityScale(0);
		}
		#endregion
    }

    private void FixedUpdate()
	{
		//Handle Run
		if (!IsDashing)
		{
			if (IsWallJumping)
				Run( wallJumpRunLerp);
			else
				Run(1);
		}
		else if (_isDashAttacking)
		{
			Run( dashEndRunLerp);
		}

		//Handle Slide
		if (IsSliding)
			Slide();
    }

    #region INPUT CALLBACKS
	//Methods which whandle input detected in Update()
    public void OnJumpInput()
	{
		LastPressedJumpTime =  jumpInputBufferTime;
	}

	public void OnJumpUpInput()
	{
		if (CanJumpCut() || CanWallJumpCut())
			_isJumpCut = true;
	}

	public void OnDashInput()
	{
		LastPressedDashTime =  dashInputBufferTime;
	}
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
	{
		RB.gravityScale = scale;
	}

	private void Sleep(float duration)
    {
		//Method used so we don't need to call StartCoroutine everywhere
		//nameof() notation means we don't need to input a string directly.
		//Removes chance of spelling mistakes and will improve error messages if any
		StartCoroutine(nameof(PerformSleep), duration);
    }

	private IEnumerator PerformSleep(float duration)
    {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
		Time.timeScale = 1;
	}
    #endregion

	//MOVEMENT METHODS
    #region RUN METHODS
    private void Run(float lerpAmount)
	{
		//Calculate the direction we want to move in and our desired velocity
		float targetSpeed = _moveInput.x *  runMaxSpeed;
		//We can reduce are control using Lerp() this smooths changes to are direction and speed
		targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

		#region Calculate AccelRate
		float accelRate;

		//Gets an acceleration value based on if we are accelerating (includes turning) 
		//or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ?  runAccelAmount :  runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ?  runAccelAmount *  accelInAir :  runDeccelAmount *  deccelInAir;
		#endregion

		#region Add Bonus Jump Apex Acceleration
		//Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
		if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) <  jumpHangTimeThreshold)
		{
			accelRate *=  jumpHangAccelerationMult;
			targetSpeed *=  jumpHangMaxSpeedMult;
		}
		#endregion

		#region Conserve Momentum
		//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if( doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
		{
			//Prevent any deceleration from happening, or in other words conserve are current momentum
			//You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
			accelRate = 0; 
		}
		#endregion

		//Calculate difference between current velocity and desired velocity
		float speedDif = targetSpeed - RB.velocity.x;
		//Calculate force along x-axis to apply to thr player

		float movement = speedDif * accelRate;

		//Convert this to a vector and apply to rigidbody
		RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

		/*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
	}

	private void Turn()
	{
		//stores scale and flips the player along the x axis, 
		Vector3 scale = transform.localScale; 
		scale.x *= -1;
		transform.localScale = scale;

		IsFacingRight = !IsFacingRight;
	}
    #endregion

    #region JUMP METHODS
    private void Jump()
	{
		//Ensures we can't call Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;

		#region Perform Jump
		//We increase the force applied if we are falling
		//This means we'll always feel like we jump the same amount 
		//(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
		float force =  jumpForce;
		if (RB.velocity.y < 0)
			force -= RB.velocity.y;

		RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		#endregion
	}

	private void WallJump(int dir)
	{
		//Ensures we can't call Wall Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;
		LastOnWallRightTime = 0;
		LastOnWallLeftTime = 0;

		#region Perform Wall Jump
		Vector2 force = new Vector2( wallJumpForce.x,  wallJumpForce.y);
		force.x *= dir; //apply force in opposite direction of wall

		if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
			force.x -= RB.velocity.x;

		if (RB.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
			force.y -= RB.velocity.y;

		//Unlike in the run we want to use the Impulse mode.
		//The default mode will apply are force instantly ignoring masss
		RB.AddForce(force, ForceMode2D.Impulse);
		#endregion
	}
	#endregion

	#region DASH METHODS
	//Dash Coroutine
	private IEnumerator StartDash(Vector2 dir)
	{
		//Overall this method of dashing aims to mimic Celeste, if you're looking for
		// a more physics-based approach try a method similar to that used in the jump

		LastOnGroundTime = 0;
		LastPressedDashTime = 0;

		float startTime = Time.time;

		_dashesLeft--;
		_isDashAttacking = true;

		SetGravityScale(0);

		//We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
		while (Time.time - startTime <=  dashAttackTime)
		{
			RB.velocity = dir.normalized *  dashSpeed;
			//Pauses the loop until the next frame, creating something of a Update loop. 
			//This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
			yield return null;
		}

		startTime = Time.time;

		_isDashAttacking = false;

		//Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
		SetGravityScale( gravityScale);
		RB.velocity =  dashEndSpeed * dir.normalized;

		while (Time.time - startTime <=  dashEndTime)
		{
			yield return null;
		}

		//Dash over
		IsDashing = false;
	}

	//Short period before the player is able to dash again
	private IEnumerator RefillDash(int amount)
	{
		//SHoet cooldown, so we can't constantly dash along the ground, again this is the implementation in Celeste, feel free to change it up
		_dashRefilling = true;
		yield return new WaitForSeconds( dashRefillTime);
		_dashRefilling = false;
		_dashesLeft = Mathf.Min( dashAmount, _dashesLeft + 1);
	}
	#endregion

	#region OTHER MOVEMENT METHODS
	private void Slide()
	{
		//We remove the remaining upwards Impulse to prevent upwards sliding
		if(RB.velocity.y > 0)
		{
		    RB.AddForce(-RB.velocity.y * Vector2.up,ForceMode2D.Impulse);
		}
	
		//Works the same as the Run but only in the y-axis
		//THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
		float speedDif =  slideSpeed - RB.velocity.y;	
		float movement = speedDif *  slideAccel;
		//So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
		//The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

		RB.AddForce(movement * Vector2.up);
	}
    #endregion


    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
	{
		if (isMovingRight != IsFacingRight)
			Turn();
	}

    private bool CanJump()
    {
		return LastOnGroundTime > 0 && !IsJumping;
    }

	private bool CanWallJump()
    {
		return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
			 (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
	}

	private bool CanJumpCut()
    {
		return IsJumping && RB.velocity.y > 0;
    }

	private bool CanWallJumpCut()
	{
		return IsWallJumping && RB.velocity.y > 0;
	}

	private bool CanDash()
	{
		if (!IsDashing && _dashesLeft <  dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
		{
			StartCoroutine(nameof(RefillDash), 1);
		}

		return _dashesLeft > 0;
	}

	public bool CanSlide()
    {
		if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
			return true;
		else
			return false;
	}
    #endregion


    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
		Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
	}
    #endregion
}

// created by Dawnosaur :D
