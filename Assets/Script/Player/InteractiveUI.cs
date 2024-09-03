using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractiveUI : MonoBehaviour
{
    public PlayerControllerNetworked localPlayerController;
    NetworkRunner _runner;
    NetworkObject _localPlayerObject;

    void Start()
    {
        _runner = NetworkRunner.Instances.First();
    }

    void Update()
    {
        if (_localPlayerObject) return;
        if (_runner.TryGetPlayerObject(_runner.LocalPlayer, out _localPlayerObject) && localPlayerController == null)
        {
            localPlayerController = _localPlayerObject.GetComponent<PlayerControllerNetworked>();
        }
    }

    public void OnBackButtonClicked()
    {
        if (!_runner) return;
        _runner.Disconnect(_runner.LocalPlayer);
        Destroy(_runner.gameObject);
        SceneManager.LoadScene("RoomSelect");
    }

    public void OnClassButtonClicked(int classTypeInt)
    {
        localPlayerController.RPC_SetClass(classTypeInt);
    }
}
