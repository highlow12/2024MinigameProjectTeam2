using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte JUMP = 1;
    public const byte JUMPUP = 2;
    public const byte DASH = 3;
    public const byte KEYBOARD_P = 4;
    public NetworkButtons buttons;
    public Vector3 direction;
}