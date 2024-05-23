using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerButtons
{
    Jump = 0,
    Roll = 1,
}
public struct PlayerInputData : INetworkInput
{
    public NetworkButtons buttons;
    public Vector2 direction;
}
