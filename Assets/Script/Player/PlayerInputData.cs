using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerButtons
{
    Jump = 0,
    Dash = 1,
    Roll = 2,
    Attack = 3,
    Skill = 4,
}
public struct PlayerInputData : INetworkInput
{
    public NetworkButtons buttons;
    public Vector2 direction;
}
