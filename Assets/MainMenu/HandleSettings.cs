using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleSettings : MonoBehaviour
{
    public static HandleSettings Instance;

    public float masterVolume = 1;
    public float backgroundVolume = 0.5f;
    public float effectVolume = 0.5f;

    public int resolutionIndex = 0;
    public int width = 1280;
    public int height = 720;
    public bool fullscreen = false;

    public static string GetKey(VolumeKind kind)
    {
        if (VolumeKind.Master == kind) return "masterVol";
        if (VolumeKind.Background == kind) return "bgmVol";
        if (VolumeKind.SFX == kind) return "sfxVol";

        return "";
    }

    float LoadConf(VolumeKind kind, ref float _var, float defaultValue)
    {
        return LoadConf(GetKey(kind), ref _var, defaultValue);
    }

    float LoadConf(string key, ref float _var, float defaultValue)
    {
        if (PlayerPrefs.HasKey(key))
        {
            float vol = PlayerPrefs.GetFloat(key);
            _var = vol;
        }
        else
        {
            PlayerPrefs.SetFloat(key, defaultValue);
        }

        return _var;
    }

    int LoadConf(string key, ref int _var, int defaultValue)
    {
        if (PlayerPrefs.HasKey(key))
        {
            int value = PlayerPrefs.GetInt(key);
            _var = value;
        }
        else
        {
            PlayerPrefs.SetInt(key, defaultValue);
        }

        return _var;
    }

    bool LoadConf(string key, ref bool _var, bool defaultValue)
    {
        if (PlayerPrefs.HasKey(key))
        {
            if (bool.TryParse(PlayerPrefs.GetString(key), out bool value))
            {
                _var = value;
            }
            else
            {
                _var = false;
            }
        }
        else
        {
            PlayerPrefs.SetString(key, defaultValue.ToString());
        }

        return _var;
    }

    float SetConf(VolumeKind kind, float value)
    {
        return SetConf(GetKey(kind), value);
    }

    float SetConf(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        return value;
    }

    bool SetConf(string key, bool value)
    {
        PlayerPrefs.SetString(key, value.ToString());
        return value;
    }

    int SetConf(string key, int value)
    {
        PlayerPrefs.SetString(key, value.ToString());
        return value;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadConf(VolumeKind.Master, ref masterVolume, masterVolume);
            LoadConf(VolumeKind.Background, ref backgroundVolume, backgroundVolume);
            LoadConf(VolumeKind.SFX, ref effectVolume, effectVolume);

            LoadConf("resolutionIndex", ref resolutionIndex, resolutionIndex);
            LoadConf("width", ref width, width);
            LoadConf("height", ref height, height);
            LoadConf("fullscreen", ref fullscreen, fullscreen);
        }
    }

    void Start()
    {
        SetVolume(VolumeKind.Master, masterVolume);
        SetVolume(VolumeKind.Background, backgroundVolume);
        SetVolume(VolumeKind.SFX, effectVolume);

        SetResolution(resolutionIndex, width, height, fullscreen);
    }

    public float GetVolume(VolumeKind kind)
    {
        if (kind == VolumeKind.Master) return masterVolume;
        else if (kind == VolumeKind.Background) return backgroundVolume;
        else if (kind == VolumeKind.SFX) return effectVolume;
        else return -1;
    }

    public void SetVolume(VolumeKind kind, float value)
    {
        Debug.Log($"{kind} {value}");
        if (kind == VolumeKind.Master) masterVolume = value;
        else if (kind == VolumeKind.Background) backgroundVolume = value;
        else if (kind == VolumeKind.SFX) effectVolume = value;

        SetConf(kind, value);
        BGMmanager.instance.SetVolume(masterVolume, backgroundVolume);
        SFXManager.instance.SetVolume(masterVolume, effectVolume);
    }

    public void SetResolution(int index, int width, int height, bool fullscreen)
    {
        if (width < 1280) width = 1280;
        if (height < 720) height = 720;
        SetConf("resolutionIndex", index);
        SetConf("width", width);
        SetConf("height", height);
        SetConf("fullscreen", fullscreen);
        
        Screen.SetResolution(width, height, fullscreen);
    }
}
