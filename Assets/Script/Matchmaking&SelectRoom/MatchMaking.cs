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

public class MatchMaking : SimulationBehaviour, INetworkRunnerCallbacks
{
    public string nickName;
    private NetworkRunner _runner;
    [SerializeField] string sceneName = "BossMonsterTest.unity";
    [SerializeField] TMP_InputField sessionName;
    [SerializeField] TMP_InputField nickNameField;
    [SerializeField] private NetworkPrefabRef bossPrefab;
    [SerializeField] private NetworkPrefabRef playerInfosProviderPrefab;
    [SerializeField] private NetworkPrefabRef characterPrefab;
    [SerializeField] private GameObject debugPanel;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();
    bool _jump = false;
    bool _dash = false;
    bool _roll = false;
    bool _attack = false;

    private void Awake()
    {
        // Fix the target frame rate to 64 (tick rate of the server)
        Application.targetFrameRate = 64;
    }

    private void Update()
    {
        _jump = _jump || Input.GetKeyDown(KeyCode.Space);
        _dash = _dash || Input.GetKeyDown(KeyCode.C);
        _roll = _roll || Input.GetKeyDown(KeyCode.LeftShift);
        _attack = _attack || Input.GetKeyDown(KeyCode.Mouse0);
    }

    async void StartGame(GameMode mode)
    {
        nickName = nickNameField.text;
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
        gameObject.AddComponent<RunnerSimulatePhysics2D>();
        // client side prediction을 사용하도록 설정합니다.
        var physicsConfig = GetComponent<RunnerSimulatePhysics2D>();
        physicsConfig.ClientPhysicsSimulation = ClientPhysicsSimulation.SyncTransforms;
        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/" + sceneName));
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName.text,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Create a unique position for the player
            Vector3 spawnPosition;
            NetworkObject networkCharacterObject;
            if (runner.ActivePlayers.Count() == 1)
            {
                spawnPosition = new(0, -4.6f, 0);
                networkCharacterObject = runner.Spawn(bossPrefab, spawnPosition, Quaternion.identity);
                runner.Spawn(playerInfosProviderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                var newPlayerRef = new PlayerRef();
                _spawnedCharacters.Add(newPlayerRef, networkCharacterObject);
                // debugPanel.SetActive(true);
            }


            spawnPosition = new(player.RawEncoded % runner.Config.Simulation.PlayerCount * 1, 1, 0);
            networkCharacterObject = runner.Spawn(characterPrefab, spawnPosition, Quaternion.identity, player);

            // if (player == runner.LocalPlayer)
            // {
            //     CameraMovement cameraMovement = Camera.main.GetComponent<CameraMovement>();
            //     cameraMovement.followTarget = networkCharacterObject;
            // }
            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkCharacterObject);

        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new PlayerInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector2.up;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector2.down;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector2.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector2.right;

        /*data.buttons.Set(NetworkInputData.JUMP, _jump);
        Debug.Log($"jump {_jump}");
        _jump = false;
        data.buttons.Set(NetworkInputData.DASH, _dash);
        Debug.Log($"dash {_dash}");
        _dash = false;*/

        data.buttons.Set(PlayerButtons.Jump, _jump);
        _jump = false;
        data.buttons.Set(PlayerButtons.Dash, _dash);
        _dash = false;
        data.buttons.Set(PlayerButtons.Roll, _roll);
        _roll = false;
        data.buttons.Set(PlayerButtons.Attack, _attack);
        _attack = false;

        input.Set(data);
    }

    public void STARTGAME_host() { StartGame(GameMode.Host); }
    public void STARTGAME_client() { StartGame(GameMode.Client); }


    #region Not used callbacks
    public void OnConnectedToServer(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        //throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        //throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        //throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        //throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        //throw new NotImplementedException();
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        //throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        //throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        //throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        //throw new NotImplementedException();
    }
    #endregion
}
