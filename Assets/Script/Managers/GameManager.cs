using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;


public class GameManager : SingletonNetwork<GameManager>
{
    public enum GameState
    {
        Lobby, Playing, Loading
    }
    public GameState State {  get; private set; }
    public void SetGameState(GameState state)
    {
        State = state;
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


}
