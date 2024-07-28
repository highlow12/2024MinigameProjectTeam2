using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using TMPro;
using System;
using Fusion.Addons.Physics;
using UnityEngine.SceneManagement;
using System.Linq;

public class MatchMaking : MonoBehaviour
{
    [SerializeField] TMP_InputField sessionName;
    [SerializeField] TMP_InputField nickNameField;
    [SerializeField] private NetworkManager networkManager;

    public void STARTGAME_host() { networkManager.StartGame(GameMode.Host, sessionName.text, nickNameField.text); }
    public void STARTGAME_client() { networkManager.StartGame(GameMode.Client, sessionName.text, nickNameField.text); }
}
