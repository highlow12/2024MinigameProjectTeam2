using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject pausePanel;
    public SettingPanel settingPanel;

    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] Toggle fullscreen;

    Resolution[] resolutions;
    
    // Start is called before the first frame update
    void Start()
    {
        pausePanel.SetActive(false);
        resolutions = Screen.resolutions;
        foreach (Resolution res in resolutions)
        {
            TMP_Dropdown.OptionData opt = new TMP_Dropdown.OptionData
            {
                text = $"{res.width} x {res.height}"
            };
            dropdown.options.Add(opt);

            Debug.Log($"{res.width} x {res.height}");
        }

        dropdown.value = HandleSettings.Instance.resolutionIndex;
    }

    public void SetResolution()
    {
        Resolution res = resolutions[dropdown.value];
        HandleSettings.Instance.SetResolution(dropdown.value, res.width, res.height, fullscreen.isOn);
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
