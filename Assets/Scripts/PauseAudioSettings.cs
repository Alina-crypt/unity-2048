using UnityEngine;
using UnityEngine.UI;

public class PauseAudioSettings : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    

    private void Start()
    {
      
    }

    public void SetMusicVolume(float value)
    { 

        
        PlayerPrefs.SetFloat("MusicVolume", value);

    }

    public void SetSFXVolume(float value)
    {

        PlayerPrefs.SetFloat("SFXVolume", value);
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.Save(); 
    }
}
