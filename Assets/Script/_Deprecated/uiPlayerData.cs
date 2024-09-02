using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uiPlayerData : MonoBehaviour
{
    [Header("Gravity")]
    [HideInInspector] public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
    [HideInInspector] public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).
                                                 //Also the value the player's rigidbody2D.gravityScale is set to.
    [Space(5)]
    public float fallGravityMult;
    public InputField fallGravityMult_;//Multiplier to the player's gravityScale when falling.
    public float maxFallSpeed;
    public InputField maxFallSpeed_;//Maximum fall speed (terminal velocity) of the player when falling.
    [Space(5)]
    public float fastFallGravityMult;
    public InputField fastFallGravityMult_;//Larger multiplier to the player's gravityScale when they are falling and a downwards input is pressed.
                                    //Seen in games such as Celeste, lets the player fall extra fast if they wish.
    public float maxFastFallSpeed;
    public InputField maxFastFallSpeed_;//Maximum fall speed(terminal velocity) of the player when performing a faster fall.

    [Space(20)]

    [Header("Run")]
    public float runMaxSpeed;
    public InputField runMaxSpeed_;//Target speed we want the player to reach.
    public float runAcceleration;
    public InputField runAcceleration_;//The speed at which our player accelerates to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
    [HideInInspector] public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
    public float runDecceleration;
    public InputField runDecceleration_;//The speed at which our player decelerates from their current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
    [HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
    [Space(5)]
    [Range(0f, 1)] public float accelInAir;
    public InputField accelInAir_;//Multipliers applied to acceleration rate when airborne.
    [Range(0f, 1)] public float deccelInAir;
    public InputField deccelInAir_;
    [Space(5)]
    public bool doConserveMomentum = true;

    [Space(20)]

    [Header("Jump")]
    public float jumpHeight;
    public InputField jumpHeight_;//Height of the player's jump
    public float jumpTimeToApex;
    public InputField jumpTimeToApex_;//Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
    [HideInInspector] public float jumpForce; //The actual force applied (upwards) to the player when they jump.

    [Header("Both Jumps")]
    public float jumpCutGravityMult;
    public InputField jumpCutGravityMult_;//Multiplier to increase gravity if the player releases thje jump button while still jumping
    [Range(0f, 1)] public float jumpHangGravityMult;
    public InputField jumpHangGravityMult_;//Reduces gravity while close to the apex (desired max height) of the jump
    public float jumpHangTimeThreshold;
    public InputField jumpHangTimeThreshold_;//Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
    [Space(0.5f)]
    public float jumpHangAccelerationMult;
    public InputField jumpHangAccelerationMult_;
    public float jumpHangMaxSpeedMult;
    public InputField jumpHangMaxSpeedMult_;

    [Header("Wall Jump")]
    public Vector2 wallJumpForce;
    public InputField wallJumpForceX;
    public InputField wallJumpForceY;//The actual force (this time set by us) applied to the player when wall jumping.
    [Space(5)]
    [Range(0f, 1f)] public float wallJumpRunLerp;
    public InputField wallJumpRunLerp_;//Reduces the effect of player's movement while wall jumping.
    [Range(0f, 1.5f)] public float wallJumpTime;
    public InputField wallJumpTime_;//Time after wall jumping the player's movement is slowed for.
    public bool doTurnOnWallJump;
    public Toggle doTurnOnWallJump_;//Player will rotate to face wall jumping direction

    [Space(20)]

    [Header("Slide")]
    public float slideSpeed;
    public InputField slideSpeed_;
    public float slideAccel;
    public InputField slideAccel_;

    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime;
    public InputField coyoteTime_;//Grace period after falling off a platform, where you can still jump
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime;
    public InputField jumpInputBufferTime_;//Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

    [Space(20)]

    [Header("Dash")]
    public int dashAmount;
    public InputField dashAmount_;
    public float dashSpeed;
    public InputField dashSpeed_;
    public float dashSleepTime;
    public InputField dashSleepTime_;//Duration for which the game freezes when we press dash but before we read directional input and apply a force
    [Space(5)]
    public float dashAttackTime;
    public InputField dashAttackTime_;
    [Space(5)]
    public float dashEndTime;
    public InputField dashEndTime_;//Time after you finish the inital drag phase, smoothing the transition back to idle (or any standard state)
    public Vector2 dashEndSpeed;
    public InputField dashEndSpeedX;
    public InputField dashEndSpeedY;//Slows down player, makes dash feel more responsive (used in Celeste)
    [Range(0f, 1f)] public float dashEndRunLerp;
    public InputField dashEndRunLerp_;//Slows the affect of player movement while dashing
    [Space(5)]
    public float dashRefillTime;
    public InputField dashRefillTime_;
    [Space(5)]
    [Range(0.01f, 0.5f)] public float dashInputBufferTime;
    public InputField dashInputBufferTime_;


    //Unity Callback, called when the inspector updates
    private void OnValidate()
    {
        //Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

        //Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
        gravityScale = gravityStrength / Physics2D.gravity.y;

        //Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        //Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        #region Variable Ranges
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion
    }
}
