using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private ball _prefabball;
    [SerializeField] private PhysxBall _prefabPhysxBall;
    [SerializeField] private TestBossMonster _prefabBossMonster;

    [Networked] private TickTimer delay { get; set; }

    private NetworkCharacterController _ncc;
    private CharacterController _cc;
    private Transform _gc;
    private Transform _groundSprite;
    private Vector3 _forward;

    private void Awake()
    {
        _ncc = GetComponent<NetworkCharacterController>();
        _cc = GetComponent<CharacterController>();
        _gc = transform.Find("groundCheck").gameObject.GetComponent<Transform>();
        _groundSprite = GameObject.Find("Static Sprite").GetComponent<Transform>();
        _forward = transform.up;
    }

    private bool IsCheckGrounded()
    {
        // CharacterController의 isGronded가 true일 때는 Raycast 없이 true를 반환
        if (_cc.isGrounded) return true;
        // Raycast를 위한 Ray 생성
        var ray = new Ray(_gc.position + Vector3.up * 0.1f, Vector3.down);
        // Raycast 거리
        var maxDistance = 0.51f;
        // Debug용 Raycast
        Debug.DrawRay(_gc.position + Vector3.up * 0.1f, ray.direction * maxDistance, Color.red);
        // Raycast
        return Physics.Raycast(ray, maxDistance, LayerMask.GetMask("_groundLayer"));


    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            // _ncc.Grounded = IsCheckGrounded();
            _ncc.Grounded = true;

        }
        if (GetInput(out NetworkInputData data))
        {

            data.direction.Normalize();
            _ncc.Move(5 * data.direction * Runner.DeltaTime);

            if (data.direction.sqrMagnitude > 0)
                _forward = data.direction;

            if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
            {
                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabball,
                      transform.position + _forward,
                      Quaternion.LookRotation(_forward),
                      Object.InputAuthority,
                      (runner, o) =>
                      {
                          // Initialize the Ball before synchronizing it
                          o.GetComponent<ball>().Init();
                      });
                }
                else if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1))
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabPhysxBall,
                      transform.position + _forward + Vector3.up * 3,
                      Quaternion.LookRotation(_forward),
                      Object.InputAuthority,
                      (runner, o) =>
                      {
                          o.GetComponent<PhysxBall>().Init(10 * _forward);
                      });
                }
                else if (data.buttons.IsSet(NetworkInputData.KEYBOARD_P))
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabBossMonster,
                      transform.position + _forward,
                      Quaternion.LookRotation(forward: Vector3.forward),
                      Object.InputAuthority,
                      (runner, o) =>
                      {
                          o.GetComponent<TestBossMonster>().Init();
                      });

                }
            }
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("OnCollisionEnter2D");
    }
}