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
    float volume = 0;
    Queue<AudioSource> queue = new();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy(gameObject);
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
        if (queue.Count > 0)
        {
            var s = queue.Dequeue();
            s.PlayOneShot(clip, volume);
            queue.Enqueue(s);
        }
        else
        {
            foreach (AudioSource source in bgms)
            {
                queue.Enqueue(source);
            }
        }
    }

    public void SetVolume(float master, float value)
    {
        volume = master * value / 81;
        foreach (AudioSource source in bgms)
        {
            source.volume = volume;
        }
    }
}
