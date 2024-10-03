using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Ending : MonoBehaviour
{
    VideoPlayer vp;

    void Awake()
    {
        vp = GetComponent<VideoPlayer>();
        vp.loopPointReached += End;
    }

    void Start()
    {
        //BGMmanager.Instance.Stop();
    }
    
    void End(VideoPlayer _vp)
    {
        Debug.Log("OK!");
        Skip();
    }

    public void Skip()
    {
        BGMmanager.Instance.Play(0);
        SceneManager.LoadScene("MainMenu2");
    }
}
