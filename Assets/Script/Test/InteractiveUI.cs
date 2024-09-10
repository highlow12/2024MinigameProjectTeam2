using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
<<<<<<< HEAD
=======
using UnityEngine.SceneManagement;
>>>>>>> ff96bb64b01cb27b7fe339336ad1d1fb380b4d1c

public class InteractiveUI : MonoBehaviour
{
    public PlayerControllerNetworked localPlayerController;
    NetworkRunner _runner;
    NetworkObject _localPlayerObject;

    void Start()
    {
        _runner = NetworkRunner.Instances.First();
<<<<<<< HEAD

=======
>>>>>>> ff96bb64b01cb27b7fe339336ad1d1fb380b4d1c
    }

    void Update()
    {
<<<<<<< HEAD
        if (_localPlayerObject == null)
        {
            _runner.TryGetPlayerObject(_runner.LocalPlayer, out _localPlayerObject);
        }
        else if (localPlayerController == null)
=======
        if (_localPlayerObject) return;
        if (_runner.TryGetPlayerObject(_runner.LocalPlayer, out _localPlayerObject) && localPlayerController == null)
>>>>>>> ff96bb64b01cb27b7fe339336ad1d1fb380b4d1c
        {
            localPlayerController = _localPlayerObject.GetComponent<PlayerControllerNetworked>();
        }
    }

<<<<<<< HEAD
=======
    public void OnBackButtonClicked()
    {
        if (!_runner) return;
        _runner.Disconnect(_runner.LocalPlayer);
        Destroy(_runner.gameObject);
        SceneManager.LoadScene("RoomSelect");
    }

>>>>>>> ff96bb64b01cb27b7fe339336ad1d1fb380b4d1c
    public void OnClassButtonClicked(int classTypeInt)
    {
        localPlayerController.RPC_SetClass(classTypeInt);
    }
<<<<<<< HEAD

=======
>>>>>>> ff96bb64b01cb27b7fe339336ad1d1fb380b4d1c
}
