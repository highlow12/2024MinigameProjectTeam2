using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Timeline;
using UnityEngine;

public  class BGMmanager : MonoBehaviour
{
    private static BGMmanager Instance = null;
    static public BGMmanager instance
    { 
        get 
        {
            if (Instance == null) { return null; }
            else { return Instance; }
        }

        
    }

    AudioSource bgm;
    public AudioClip[] clip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        if (gameObject.TryGetComponent<AudioSource>(out bgm))
        {
            bgm.clip = clip[0];
            bgm.Play();
        }
        else
        {
            bgm = gameObject.AddComponent<AudioSource>(); ;
            bgm.clip = clip[0];
            bgm.Play();
        }
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
    public bool isBossBGM()
    {
        return bgm.clip == clip[1];
    }
}
