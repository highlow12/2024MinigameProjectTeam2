using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Fusion;
using Fusion.Sockets;
using Fusion.Addons.Physics;

using System;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;

public class NetworkManager : SimulationBehaviour, INetworkRunnerCallbacks
{
    public string nickName;
    private NetworkRunner _runner;
    [SerializeField] string sceneName = "BossMonsterTest.unity";
    [SerializeField] private InputManager inputManager;
    [SerializeField] private NetworkPrefabRef bossPrefab;
    [SerializeField] private NetworkPrefabRef playerInfosProviderPrefab;
    [SerializeField] private NetworkPrefabRef characterPrefab;
    [SerializeField] private GameObject otherStatusPrefab;
    [SerializeField] private GameObject debugPanel;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();

    private void Awake()
    {
        // Fix the target frame rate to 64 (tick rate of the server)
        Application.targetFrameRate = 64;
    }

    public async void StartGame(GameMode mode, string sessionName, string nick)
    {
        nickName = nick;
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
        var physicsConfig = gameObject.AddComponent<RunnerSimulatePhysics2D>();
        // client side prediction을 사용하도록 설정합니다.
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
            SessionName = sessionName,
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
            runner.SetPlayerObject(player, networkCharacterObject);
            _spawnedCharacters.Add(player, networkCharacterObject);
        }
            
        runner.TryGetPlayerObject(player, out NetworkObject playerObj);
        PlayerControllerNetworked controller = playerObj.GetComponent<PlayerControllerNetworked>();
        controller.player = player;
        if (runner.LocalPlayer != player)
        {
            Canvas canvas = FindAnyObjectByType<Canvas>();
            GameObject other = Instantiate(otherStatusPrefab, canvas.gameObject.transform);
            OtherStatusPanel osp = other.GetComponent<OtherStatusPanel>();
            controller.otherStatusPanel = osp;
            other.SetActive(true);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            Destroy(networkObject.GetComponent<PlayerControllerNetworked>().otherStatusPanel.gameObject);
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        inputManager.OnInput(runner, input);
    }

    #region Not used callbacks
    public void OnConnectedToServer()
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

    public void OnDisconnectedFromServer()
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

    public void OnConnectedToServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }
    #endregion
}
