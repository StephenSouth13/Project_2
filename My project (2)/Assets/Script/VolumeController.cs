using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // Cần thiết để tương tác với UI Slider

public class VolumeController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; // Tham chiếu đến AudioMixer
    [SerializeField] private Slider musicSlider; // Tham chiếu đến UI Slider
    [SerializeField] private Slider sfxSlider; // Tham chiếu đến UI Slider

    public void SetMusicVolume()
    {
        // Chuyển đổi giá trị slider (0.0 đến 1.0) sang decibel (-80 đến 0 dB)
        float volume = musicSlider.value;
        audioMixer.SetFloat("music",Mathf.Log10(volume)*20);
    }

    public void SetSFXVolume()
    {
        // Chuyển đổi giá trị slider (0.0 đến 1.0) sang decibel (-80 đến 0 dB)
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFXVolume",Mathf.Log10(volume)*20);
    }
}