using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public  class BGMmanager : MonoBehaviour
{
    static public BGMmanager instance 
    { 
        get 
        {
            if (instance == null)
            {
                instance = new();
            }
            return instance;
        }
        private set { instance = value; }
    }

    AudioSource bgm;
    AudioClip[] clip;
    private void Start()
    {
        bgm = GetComponent<AudioSource>();
    }
    public void playBossBGM()
    {
        bgm.clip = clip[1];
        bgm.Play();
    }
    public void stopBossBGM()
    {
        bgm.Stop();
        bgm.clip = clip[0];
        bgm.Play();
    }

}
