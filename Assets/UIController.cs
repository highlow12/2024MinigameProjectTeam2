using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject pausePanel;
    public SettingPanel settingPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        pausePanel.SetActive(false);    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
        }
    }
}
