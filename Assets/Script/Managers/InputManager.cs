using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class InputManager : SimulationBehaviour
{
    bool _jump = false;
    bool _dash = false;
    bool _roll = false;
    bool _attack = false;
    bool _skill = false;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        _jump = _jump || Input.GetKeyDown(KeyCode.Space);
        _dash = _dash || Input.GetKeyDown(KeyCode.C);
        _roll = _roll || Input.GetKeyDown(KeyCode.LeftShift);
        _attack = _attack || Input.GetKeyDown(KeyCode.Mouse0);
        _skill = _skill || Input.GetKeyDown(KeyCode.R);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new PlayerInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector2.up;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector2.down;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector2.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector2.right;

        /*data.buttons.Set(NetworkInputData.JUMP, _jump);
        Debug.Log($"jump {_jump}");
        _jump = false;
        data.buttons.Set(NetworkInputData.DASH, _dash);
        Debug.Log($"dash {_dash}");
        _dash = false;*/

        data.buttons.Set(PlayerButtons.Jump, _jump);
        _jump = false;
        data.buttons.Set(PlayerButtons.Dash, _dash);
        _dash = false;
        data.buttons.Set(PlayerButtons.Roll, _roll);
        _roll = false;
        data.buttons.Set(PlayerButtons.Attack, _attack);
        _attack = false;
        data.buttons.Set(PlayerButtons.Skill, _skill);
        _skill = false;

        input.Set(data);
    }
}
