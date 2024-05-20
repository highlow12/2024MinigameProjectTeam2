using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleSettings : MonoBehaviour
{
    public float masterVolume = 1;
    public float backgroundVolume = 0.5f;
    public float effectVolume = 0.5f;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }


    void Start()
    {
        // SceneManager.LoadScene("PlayerMovementTestScne");
    }

    void Update()
    {
        
    }
}
