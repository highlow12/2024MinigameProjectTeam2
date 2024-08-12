using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    HandleSettings settings;
    GameObject panel;

    GameObject VolumePanel;
    Button VolumeNavButton;
    TMP_Text VolumeNavButtonLabel;
    GameObject DisplayPanel;
    Button DisplayNavButton;
    TMP_Text DisplayNavButtonLabel;

    Scrollbar master;
    TMP_Text masterValueLabel;

    Scrollbar background;
    TMP_Text backgroundValueLabel;

    Scrollbar effect;
    TMP_Text effectValueLabel;

    Color32 ButtonDefaultColor = new Color32(97, 126, 97, 255);
    Color32 ButtonActiveColor = new Color32(231, 239, 241, 255);

    IEnumerable<T> Finds<T>() where T : UnityEngine.Object {
        return FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    T Find<T>(string name) where T : UnityEngine.Object {
        return Finds<T>().Where(t => t.name == name).First();
    }

    void Awake()
    {
        settings = Find<GameObject>("SettingsObj").GetComponent<HandleSettings>();
        panel = Find<GameObject>("SettingPanel");
        VolumePanel = Find<GameObject>("VolumePanel");
        VolumeNavButton = Find<Button>("VolumeNavButton");
        VolumeNavButtonLabel = Find<TMP_Text>("VolumeNavButtonLabel");
        DisplayPanel = Find<GameObject>("DisplayPanel");
        DisplayNavButton = Find<Button>("DisplayNavButton");
        DisplayNavButtonLabel = Find<TMP_Text>("DisplayNavButtonLabel");

        // master = Find<Scrollbar>("MasterScrollBar");
        masterValueLabel = Find<TMP_Text>("MasterValueLabel");

        // background = Find<Scrollbar>("BackgroundScrollBar");
        backgroundValueLabel = Find<TMP_Text>("BackgroundValueLabel");

        // effect = Find<Scrollbar>("EffectScrollBar");
        effectValueLabel = Find<TMP_Text>("EffectValueLabel");

        // master.onValueChanged.AddListener(OnMasterValueChanged);
        // background.onValueChanged.AddListener(OnBackgroundValueChanged);
        // effect.onValueChanged.AddListener(OnEffectValueChanged);
    }

    void OnEnable()
    {
        VolumePanelShow();
    }

    void OnMasterValueChanged(float value) {
        float volume = (float)Math.Round(value, 2) * 100;
        settings.masterVolume = volume;
        masterValueLabel.SetText($"({volume}%)");
    }

    void OnBackgroundValueChanged(float value) {
        float volume = (float)Math.Round(value, 2) * 100;
        settings.backgroundVolume = volume;
        backgroundValueLabel.SetText($"({volume}%)");
    }

    void OnEffectValueChanged(float value) {
        float volume = (float)Math.Round(value, 2) * 100;
        settings.effectVolume = volume;
        effectValueLabel.SetText($"({volume}%)");
    }

    void HideAllPanel() {
        ColorBlock vol = VolumeNavButton.colors;
        vol.normalColor = ButtonDefaultColor;

        VolumePanel.SetActive(false);
        VolumeNavButton.colors = vol;
        VolumeNavButtonLabel.color = ButtonDefaultColor;
        
        DisplayPanel.SetActive(false);
        DisplayNavButton.colors = vol;
        DisplayNavButtonLabel.color = ButtonDefaultColor;
    }

    public void VolumePanelShow() {
        HideAllPanel();

        ColorBlock vol = VolumeNavButton.colors;
        vol.normalColor = ButtonActiveColor;
        VolumeNavButton.colors = vol;

        VolumePanel.SetActive(true);
        VolumeNavButtonLabel.color = ButtonActiveColor;
    }

    public void DisplayPanelShow() {
        HideAllPanel();

        ColorBlock vol = DisplayNavButton.colors;
        vol.normalColor = ButtonActiveColor;
        DisplayNavButton.colors = vol;

        DisplayPanel.SetActive(true);
        DisplayNavButtonLabel.color = ButtonActiveColor;
    }

    public void ActiveToggle()
    {
        panel.gameObject.SetActive(!panel.gameObject.activeSelf);
    }
}
