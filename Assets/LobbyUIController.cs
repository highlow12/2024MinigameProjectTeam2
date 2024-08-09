using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    NetworkRunner runner;
    public List<LobbyPlayerStatus> statuses = new List<LobbyPlayerStatus>();

    void Start()
    {
        runner = NetworkRunner.Instances.First();
        if (statuses.Count > 0)
        {
            statuses[0].SetLeader();
        }
    }

    public void ReadyMe()
    {
        if (runner.TryGetPlayerObject(runner.LocalPlayer, out NetworkObject player))
        {
            PlayerControllerNetworked controller = player.GetComponent<PlayerControllerNetworked>();
            SetReady(runner.LocalPlayer, !controller.isReady);
            controller.RPC_SetReadyStatus(!controller.isReady);
        }
        
    }

    public void SetReady(PlayerRef player, bool _isReady)
    {
        int i = GetIndex(player);
        if (i != -1)
        {
            statuses[i].SetReady(_isReady);
        }
    }
    
    public void SetClass(PlayerRef player, int classID)
    {
        int i = GetIndex(player);
        if (i != -1)
        {
            statuses[i].SetClass(classID);
        }
    }

    public void SetNick(PlayerRef player, string nick)
    {
        int i = GetIndex(player);
        if (i != -1)
        {
            statuses[i].SetNick(nick);
        }
    }

    int GetIndex(PlayerRef player)
    {
        PlayerRef[] refs = runner.ActivePlayers.ToArray();
        for (int i = 0; i < refs.Length; i++)
        {
            if (refs[i] == player)
            {
                return i;
            }
        }

        return -1;
    }
}
