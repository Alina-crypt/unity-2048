using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] Image soundOnIcon;
    [SerializeField] Image soundOffIcon;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    private bool muted = false;
    [SerializeField] private VolumeSettings volumeSettings;

    private bool isUpdating = false;

    private void Start()
    {
      if (!PlayerPrefs.HasKey("muted"))
        {
            PlayerPrefs.SetInt("muted", 0);
            Load();
        }
 
        else
        {
            Load();
        }
        UpdateButtonIcon();
        AudioListener.pause = muted;
    }
    public void OnButtonPress()
    {
        if (muted == false)
        {
            muted = true;
            AudioListener.pause = true;

        }
        else
        {
            muted = false;
            AudioListener.pause = false;
        }
        Save();
        UpdateButtonIcon();

    }
    

    private void UpdateButtonIcon()
    {
        if (muted == false)
        {
        soundOnIcon.enabled = true;
            soundOffIcon.enabled = false;
        }
        else
        {
            soundOnIcon.enabled = false;
            soundOffIcon.enabled = true;
        }
    }
  

    public void SetMuteState(bool mute, bool updateSliders = false)
    {
        muted = mute;
        AudioListener.pause = muted;
        Save();
        UpdateButtonIcon();

        if (updateSliders)
        {
            if (muted)
            {
                musicSlider.value = 0f;
                SFXSlider.value = 0f;
            }
            else
            {
                musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
                SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            }
        }
    }


    private void Load()
    {
        muted = PlayerPrefs.GetInt("muted") == 1;
    }
    private void Save()
    {
        PlayerPrefs.SetInt("muted", muted ? 1 : 0);

    }


}

