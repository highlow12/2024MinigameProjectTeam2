using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSceneChage : MonoBehaviour
{
    [SerializeField] Transform door;
    [SerializeField] Transform player;
    [SerializeField] string SceneName;
    public int offset = 2;
    void Update()
    {
        var d = door.position - player.position;
        if (d.x < -offset)
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}
