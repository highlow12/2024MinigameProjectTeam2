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
    
    void End(VideoPlayer _vp)
    {
        Debug.Log("OK!");
        Skip();
    }

    public void Skip()
    {
        SceneManager.LoadScene("MainMenu2");
    }
}
