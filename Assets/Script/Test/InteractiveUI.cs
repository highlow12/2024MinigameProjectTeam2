using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

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
        if (_localPlayerObject == null)
        {
            _runner.TryGetPlayerObject(_runner.LocalPlayer, out _localPlayerObject);
        }
        else if (localPlayerController == null)
        {
            localPlayerController = _localPlayerObject.GetComponent<PlayerControllerNetworked>();
        }
    }

    public void OnClassButtonClicked(int classTypeInt)
    {
        localPlayerController.RPC_SetClass(classTypeInt);
    }

}
