using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HorizontalBindSwordOmen : NetworkBehaviour
{
    public CustomTickTimer life;
    // Start is called before the first frame update
    void Start()
    {
        life = CustomTickTimer.CreateFromSeconds(Runner, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(GetComponent<NetworkObject>());
        }
    }
}
