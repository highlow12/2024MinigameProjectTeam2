using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : SingletonNetwork<GameManager>
{
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMassage(string massage, RpcInfo info)
    {

    }
}
