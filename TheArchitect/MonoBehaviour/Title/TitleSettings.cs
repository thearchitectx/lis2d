using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSettings : MonoBehaviour
{
    [SerializeField] private Slider m_SliderSoundMain;
    [SerializeField] private Slider m_SliderSoundMusic;
    [SerializeField] private Slider m_SliderSoundNSFW;
    [SerializeField] private Toggle m_ToggleTrophyNotifications;
    [SerializeField] private InputField m_InputTargetFramerate;

    public UnityEngine.Events.UnityEvent onApply = new UnityEngine.Events.UnityEvent();

    void Start()
    {
        this.m_SliderSoundMain.onValueChanged.AddListener(value => AudioListener.volume = value);
    }

    public void ReadFromPrefs()
    {
        this.m_SliderSoundMain.value = GameSettings.GetVolumeMain();
        this.m_SliderSoundMusic.value = GameSettings.GetVolumeMusic();
        this.m_SliderSoundNSFW.value = GameSettings.GetVolumeNFSW();
        this.m_ToggleTrophyNotifications.isOn = GameSettings.GetTrophyNotifications();
        this.m_InputTargetFramerate.text = GameSettings.GetTargetFPS().ToString();
    }

    public void Apply()
    {
        GameSettings.SetVolumeMain(this.m_SliderSoundMain.value);
        GameSettings.SetVolumeMusic(this.m_SliderSoundMusic.value);
        GameSettings.SetVolumeNFSW(this.m_SliderSoundNSFW.value);
        GameSettings.SetTrophyNotifications(this.m_ToggleTrophyNotifications.isOn);
        int fps;
        if (int.TryParse(this.m_InputTargetFramerate.text, out fps))
            GameSettings.SetTargetFPS( fps );

        GameSettings.ApplyPrefs();
        ReadFromPrefs();

        onApply.Invoke();
    }

    public void Revert()
    {
        GameSettings.Revert();
        GameSettings.ApplyPrefs();
        ReadFromPrefs();
    }
}
