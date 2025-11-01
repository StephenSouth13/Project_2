using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // Cần thiết để tương tác với UI Slider

public class VolumeController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; /// Tham chiếu đến AudioMixer
    [SerializeField] private Slider musicSlider; // Tham chiếu đến UI Slider
    [SerializeField] private Slider SFXSlider; // Tham chiếu đến UI Slider


    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
           SetMusicVolume(); // Thiết lập âm lượng mặc định khi không có giá trị lưu trữ
            SetSFXVolume(); // Thiết lập âm lượng mặc định khi không có giá trị lưu trữ
        }
    }

    public void SetMusicVolume()
    {
        // Chuyển đổi giá trị slider (0.0 đến 1.0) sang decibel (-80 đến 0 dB)
        float volume = musicSlider.value;
        audioMixer.SetFloat("music",Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume); // Lưu giá trị âm lượng vào PlayerPrefs
    }

    public void SetSFXVolume()
    {
        // Chuyển đổi giá trị slider (0.0 đến 1.0) sang decibel (-80 đến 0 dB)
        float volume = SFXSlider.value;
        audioMixer.SetFloat("SFXVolume",Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("SFXVolume", volume); // Lưu giá trị âm lượng vào PlayerPrefs
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        SetMusicVolume();
        SetSFXVolume();
    }    
}