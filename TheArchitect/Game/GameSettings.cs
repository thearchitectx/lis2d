using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings 
{
    const string PREF_VOLUME_MAIN = "PREF_VOLUME_MAIN";
    const string PREF_VOLUME_MUSIC = "PREF_VOLUME_MUSIC";
    const string PREF_VOLUME_NFSW = "PREF_VOLUME_NFSW";
    const string PREF_TROPHY_NOTIF = "PREF_TROPHY_NOTIF";
    const string PREF_TARGET_FPS = "PREF_TARGET_FPS";

    public static void SetVolumeMain(float volume)
    {
        PlayerPrefs.SetFloat(PREF_VOLUME_MAIN, Mathf.Clamp01(volume));
    }

    public static void SetVolumeMusic(float volume)
    {
        PlayerPrefs.SetFloat(PREF_VOLUME_MUSIC, Mathf.Clamp01(volume));
    }

    public static void SetVolumeNFSW(float volume)
    {
        PlayerPrefs.SetFloat(PREF_VOLUME_NFSW, Mathf.Clamp01(volume));
    }

    public static void SetTrophyNotifications(bool enabled)
    {
        PlayerPrefs.SetInt(PREF_TROPHY_NOTIF, enabled ? 1 : 0);
    }

    public static void SetTargetFPS(int target)
    {
        PlayerPrefs.SetInt(PREF_TARGET_FPS, target);
    }

    public static float GetVolumeMain()
    {
        return PlayerPrefs.GetFloat(PREF_VOLUME_MAIN, 1f);
    }

    public static float GetVolumeMusic()
    {
        return PlayerPrefs.GetFloat(PREF_VOLUME_MUSIC, 1);
    }

    public static float GetVolumeNFSW()
    {
        return PlayerPrefs.GetFloat(PREF_VOLUME_NFSW, 1);
    }

    public static bool GetTrophyNotifications()
    {
        return PlayerPrefs.GetInt(PREF_TROPHY_NOTIF, 1) != 0;
    }

    public static int GetTargetFPS()
    {
        return PlayerPrefs.GetInt(PREF_TARGET_FPS, 60);
    }

    public static void Revert()
    {
        PlayerPrefs.DeleteKey(PREF_TARGET_FPS);
        PlayerPrefs.DeleteKey(PREF_TROPHY_NOTIF);
        PlayerPrefs.DeleteKey(PREF_VOLUME_MAIN);
        PlayerPrefs.DeleteKey(PREF_VOLUME_MUSIC);
        PlayerPrefs.DeleteKey(PREF_VOLUME_NFSW);
        PlayerPrefs.Save();
    }

    public static void ApplyPrefs()
    {
        Application.targetFrameRate = GetTargetFPS();
        AudioListener.volume = GameSettings.GetVolumeMain();
        PlayerPrefs.Save();
    }
}
