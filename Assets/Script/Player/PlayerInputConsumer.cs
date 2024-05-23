using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerInputConsumer : NetworkBehaviour
{
    [Networked] public NetworkButtons ButtonPrevious {  get; set; }
    public NetworkButtons pressed {  get; private set; }
    public NetworkButtons released { get; private set; }
    public Vector2 dir { get; private set; }
    public override void FixedUpdateNetwork()
    {
        if (GetInput<PlayerInputData>(out var input) == false) return;

        pressed = input.buttons.GetPressed(ButtonPrevious);
        released = input.buttons.GetReleased(ButtonPrevious);

        ButtonPrevious = input.buttons;

        dir = input.direction;
    }

}
