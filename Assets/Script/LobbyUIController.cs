using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIController : MonoBehaviour
{
    NetworkRunner runner;
    [SerializeField] TMP_Text roomName;
    [SerializeField] Button readyButton;
    TMP_Text btnTxt;
    public List<LobbyPlayerStatus> statuses = new List<LobbyPlayerStatus>();
    bool[] readys;

    void OnEnable()
    {
        btnTxt = readyButton.gameObject.GetComponentInChildren<TMP_Text>();

        readys = new bool[statuses.Count];
        Array.Fill(readys, false);

        runner = NetworkRunner.Instances.First();
        if (statuses.Count > 0)
        {
            UpdateLeader();
        }

        if (runner.IsServer)
        {
            SetReady(runner.LocalPlayer, true);
        }

        roomName.text = runner.SessionInfo.Name;
    }

    public void ReadyMe()
    {
        if (runner.IsServer)
        {
            if (readys.Where(x => !x).Count() - (3 - runner.ActivePlayers.Count()) > 0) return;
            foreach (PlayerRef player in runner.ActivePlayers)
            {
                if (runner.TryGetPlayerObject(player, out NetworkObject __))
                {
                    __.GetComponent<PlayerControllerNetworked>().RPC_AllReadyAndStart();
                }
                runner.SessionInfo.IsOpen = false;
            }
        }
        else if (runner.TryGetPlayerObject(runner.LocalPlayer, out NetworkObject player))
        {
            PlayerControllerNetworked controller = player.GetComponent<PlayerControllerNetworked>();
            SetReady(runner.LocalPlayer, !controller.isReady);
            btnTxt.text = !controller.isReady ? "준비 취소" : "준비";
            controller.RPC_SetReadyStatus(!controller.isReady);
        }
    }

    public void SetReady(PlayerRef player, bool _isReady)
    {
        int i = GetIndex(player);
        if (i != -1)
        {
            statuses[i].SetReady(_isReady);
            readys[i] = _isReady;
        }

        if (runner.IsServer)
        {
            if (readys.Where(x => !x).Count() - (3 - runner.ActivePlayers.Count()) > 0)
            {
                btnTxt.text = "대기 중";
            }
            else
            {
                btnTxt.text = "전투 시작";
            }
        }

        BGMmanager.instance.playBossBGM();
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

    public void UpdateLeader()
    {
        PlayerRef[] refs = runner.ActivePlayers.ToArray();
        for (int i = 0; i < refs.Length; i++)
        {
            if (runner.TryGetPlayerObject(refs[i], out NetworkObject player))
            {
                PlayerControllerNetworked comp = player.GetComponent<PlayerControllerNetworked>();
                statuses[i].SetLeader(comp.isLeader);
            }
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

    public void Reset()
    {
        foreach (LobbyPlayerStatus stat in statuses)
        {
            stat.Reset();
        }

        PlayerRef[] refs = runner.ActivePlayers.ToArray();
        for (int i = 0; i < refs.Length; i++)
        {
            if (runner.TryGetPlayerObject(refs[i], out NetworkObject player))
            {
                PlayerControllerNetworked comp = player.GetComponent<PlayerControllerNetworked>();
                comp.UpdateLobbyUI();
            }
        }
    }
}
