using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    HandleSettings settings;
    GameObject panel;

    GameObject VolumePanel;
    GameObject DisplayPanel;

    Scrollbar master;
    TMP_Text masterValueLabel;

    Scrollbar background;
    TMP_Text backgroundValueLabel;

    Scrollbar effect;
    TMP_Text effectValueLabel;

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
        DisplayPanel = Find<GameObject>("DisplayPanel");

        master = Find<Scrollbar>("MasterScrollBar");
        masterValueLabel = Find<TMP_Text>("MasterValueLabel");

        background = Find<Scrollbar>("BackgroundScrollBar");
        backgroundValueLabel = Find<TMP_Text>("BackgroundValueLabel");

        effect = Find<Scrollbar>("EffectScrollBar");
        effectValueLabel = Find<TMP_Text>("EffectValueLabel");

        master.onValueChanged.AddListener(OnMasterValueChanged);
        background.onValueChanged.AddListener(OnBackgroundValueChanged);
        effect.onValueChanged.AddListener(OnEffectValueChanged);

        gameObject.SetActive(false);
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
        VolumePanel.SetActive(false);
        DisplayPanel.SetActive(false);
    }

    public void VolumePanelShow() {
        HideAllPanel();
        VolumePanel.SetActive(true);
    }

    public void DisplayPanelShow() {
        HideAllPanel();
        DisplayPanel.SetActive(true);
    }

    public void ActiveToggle()
    {
        panel.gameObject.SetActive(!panel.gameObject.activeSelf);
    }
}
