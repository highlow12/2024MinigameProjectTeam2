using UnityEngine;

[DefaultExecutionOrder(-1)]
public class BGMmanager : MonoBehaviour
{
    float volume = 0f;
    public static BGMmanager Instance = null;

    AudioSource bgm;
    public AudioClip[] clip;

    private void Awake()
    {
        bgm = gameObject.GetComponent<AudioSource>();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetVolume(HandleSettings.Instance.masterVolume, HandleSettings.Instance.backgroundVolume);
        Play(0);
    }

    public void Play(int index)
    {
        if (index >= clip.Length) return;
        bgm.clip = clip[index];
        bgm.volume = volume;
        bgm.loop = true;
        bgm.Play();
    }

    public void Stop()
    {
        bgm.Stop();
    }

    public void playBossBGM()
    {
        Play(1);
    }

    public void stopBossBGM()
    {
        bgm.Stop();
        Play(0);
    }

    public void SetVolume(float master, float value)
    {
        volume = master * value / 81;
        Debug.Log(volume);
        if (bgm) bgm.volume = volume;
    }

    public bool isBossBGM()
    {
        return bgm.clip == clip[1];
    }
}
