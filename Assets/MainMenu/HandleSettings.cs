using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleSettings : MonoBehaviour
{
    public static HandleSettings Instance;

    public float masterVolume = 9;
    public float backgroundVolume = 5;
    public float effectVolume = 5;

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

    public float LoadConf(VolumeKind kind, ref float _var, float defaultValue)
    {
        return LoadConf(GetKey(kind), ref _var, defaultValue);
    }

    public float LoadConf(string key, ref float _var, float defaultValue)
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

    public int LoadConf(string key, ref int _var, int defaultValue)
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

    public bool LoadConf(string key, ref bool _var, bool defaultValue)
    {
        Debug.Log($"BOBOBOBOBOBLLLL {PlayerPrefs.GetInt(key)}");
        int _def = defaultValue ? 1 : 0;
        if (PlayerPrefs.HasKey(key))
        {
            _var = PlayerPrefs.GetInt(key) >= 1 ? true : false;

        }
        else
        {
            PlayerPrefs.SetInt(key, _def);
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
        Debug.Log($"{key} {value}");
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        return value;
    }

    int SetConf(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
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

    public void SetResolution(int _index, int _width, int _height, bool _fullscreen)
    {
        if (_width < 1280) _width = 1280;
        if (_height < 720) _height = 720;

        resolutionIndex = _index;
        width = _width;
        height = _height;
        fullscreen = _fullscreen;

        SetConf("resolutionIndex", resolutionIndex);
        SetConf("width", width);
        SetConf("height", height);
        SetConf("fullscreen", fullscreen);
        
        Screen.SetResolution(width, height, fullscreen);
    }
}
