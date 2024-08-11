using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private static SFXManager Instance = null;
    static public SFXManager instance
    {
        get
        {
            if (Instance == null) { return null; }
            else { return Instance; }
        }


    }

    public AudioSource[] bgms;
    public AudioClip[] clip;
    public float vol = 1;
    Queue<AudioSource> queue = new();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        foreach (AudioSource source in bgms)
        {
            queue.Enqueue(source);
        }
    }

    public void playSFX(AudioClip clip)
    {
        var s = queue.Dequeue();
        s.PlayOneShot(clip, vol);
        queue.Enqueue(s);
    }
}
