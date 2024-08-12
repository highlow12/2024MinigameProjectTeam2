using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIController : MonoBehaviour
{
    public static MainUIController Instance;
    [SerializeField] bool reUsable = true;

    void Awake()
    {
        if (!reUsable) return;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Application.targetFrameRate = 64;
        }
        else
        {
            Destroy(gameObject);
        }        
    }

    public void SettingPanel()
    {
        FindAnyObjectByType<UIController>().SettingPanel();
    }

    public void Shutdown()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
