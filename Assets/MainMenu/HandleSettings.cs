using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleSettings : MonoBehaviour
{
    public static HandleSettings Instance;

    public float masterVolume = 1;
    public float backgroundVolume = 0.5f;
    public float effectVolume = 0.5f;

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

    float SetConf(VolumeKind kind, float value)
    {
        return SetConf(GetKey(kind), value);
    }

    float SetConf(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
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
        }
    }

    void Start()
    {
        SetVolume(VolumeKind.Master, masterVolume);
        SetVolume(VolumeKind.Background, backgroundVolume);
        SetVolume(VolumeKind.SFX, effectVolume);
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
        
        BGMmanager.instance.SetVolume(masterVolume, backgroundVolume);
    }
}
