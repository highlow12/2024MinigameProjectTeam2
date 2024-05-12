using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInputData : INetworkInput
{
    public const byte JUMP = 1;
    public const byte DASH = 2;

    public NetworkButtons buttons;
    public Vector2 direction;
}
