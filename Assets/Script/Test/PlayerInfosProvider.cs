using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class PlayerInfosProvider : NetworkBehaviour
{
    public static PlayerInfosProvider Instance;

    void Awake()
    {
        Instance = this;
    }

    [System.Serializable]
    public struct PlayerInfoStruct : INetworkStruct
    {
        public NetworkString<_16> nickName;
        public PlayerRef playerRef;
    }
    [Networked]
    [Capacity(4)]
    NetworkArray<PlayerInfoStruct> PlayerInfos => default;

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_SetNickName(PlayerRef player)
    {
        PlayerInfoStruct playerInfo = new()
        {
            // Access the nickName from the match making singleton object
            nickName = Runner.gameObject.GetComponent<NetworkManager>().nickName,
            playerRef = player
        };
        PlayerInfos.Set(Runner.ActivePlayers.Count() - 1, playerInfo);
        Debug.Log($"Player {player} has been assigned the nickname {playerInfo.nickName}");
    }
}
