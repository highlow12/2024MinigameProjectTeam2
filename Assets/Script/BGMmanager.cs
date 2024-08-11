using UnityEngine;

public class BGMmanager : MonoBehaviour
{
    float volume = 0f;
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
            bgm = gameObject.AddComponent<AudioSource>();
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

    private void Play(int index)
    {
        if (index >= clip.Length) return;
        bgm.clip = clip[index];
        bgm.volume = volume;
        bgm.loop = true;
        bgm.Play();
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
        bgm.volume = volume;
    }

    public bool isBossBGM()
    {
        return bgm.clip == clip[1];
    }
}
