using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using TMPro;
using Bogus.DataSets;
using System;
using Fusion.Addons.Physics;
using UnityEngine.SceneManagement;
using System.Linq;

public class MatchMaking : MonoBehaviour
{
    [SerializeField] TMP_InputField sessionName;
    [SerializeField] TMP_InputField nickNameField;
    [SerializeField] private NetworkManager networkManager;
    string lobbyName;
    bool sessionNameEmpty = false;

    public void STARTGAME_host() { networkManager.StartGame(GameMode.Host, sessionName.text, nickNameField.text); }
    public void STARTGAME_client() { networkManager.StartGame(GameMode.Client, sessionName.text, nickNameField.text); }
    public void STARTGAME_auto()
    {
        // set random nickname when empty
        if (string.IsNullOrEmpty(nickNameField.text))
        {
            Hacker hacker = new();
            nickNameField.text = $"{hacker.Adjective()} {hacker.Noun()}";
        }

        networkManager.StartGame(GameMode.AutoHostOrClient, sessionName.text, nickNameField.text);
    }

    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene("MainMenu2");
    }
}
