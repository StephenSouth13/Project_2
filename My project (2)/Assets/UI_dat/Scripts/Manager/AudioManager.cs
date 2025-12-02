using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviourPun
{
    public AudioSource soundEFX, music;
    public AudioClip[] sfxAudioClip, musicAudioClip;
    public AudioClip[] characterBaseClip;
    public AudioClip[] hitClip;
    public Slider musicSlider;
    public Slider soundEFX_Slider;
    public static AudioManager instance;

    private float currentValueMusic;
    private float currentValueSoundEFX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        SetValueSlider();
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        musicSlider.value = 1f;
        soundEFX_Slider.value = 1f;
        PlayIndexMusic(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayIndexMusic(int index)
    {
        if(index < musicAudioClip.Length)
        {
            music.clip = musicAudioClip[index];
            music.Play();
        }
    }
    public void PlayIndexSoundEFX(int index)
    {
        if(index < sfxAudioClip.Length)
        {
            soundEFX.clip = sfxAudioClip[index];
            soundEFX.PlayOneShot(soundEFX.clip);
        }
    }
    public void PlayNameMusic(string name)
    {

        AudioClip clip = Array.Find(musicAudioClip, c => c.name == name);
        if (clip != null)
        {
            music.clip = clip;
            music.PlayOneShot(clip);
        }
    }
    [PunRPC]
    public void PlayNameSoundEFX(string name)
    {

        AudioClip clip = Array.Find(sfxAudioClip, c => c.name == name);
        if (clip != null)
        {
            soundEFX.clip = clip;
            soundEFX.PlayOneShot(clip);
        }
    }
    public void ToggleSoundEFX()
    {
        PlayIndexSoundEFX(0);
        if (soundEFX.mute = !soundEFX.mute)
        {
            currentValueSoundEFX = soundEFX_Slider.value;
            
            soundEFX_Slider.value = 0;
        }
        else
        {
            soundEFX_Slider.value = currentValueSoundEFX;

        }

    }
    public void ToggleMusic()
    {
        PlayIndexSoundEFX(0);
        
        if (music.mute = !music.mute)
        {
            currentValueMusic = musicSlider.value;

            musicSlider.value = 0;
        }
        else
        {
            musicSlider.value = currentValueMusic;
        }
        
    }
    public void SetValueSlider()
    {
        if (musicSlider != null)
        {
            musicSlider.value = music.volume;
            musicSlider.onValueChanged.AddListener(SetChangeVolumeMusic);
        }

        if (soundEFX_Slider != null)
        {
            soundEFX_Slider.value = soundEFX.volume;
            soundEFX_Slider.onValueChanged.AddListener(SetChangeVolumeSoundEFX);
        }
    }
    public void SetChangeVolumeMusic(float sliderValue)
    {
        // sliderValue vẫn từ 0 -> 1
        music.volume = sliderValue * 0.25f; // giới hạn âm lượng tối đa còn 0.25
    }

    public void SetChangeVolumeSoundEFX(float sliderValue)
    {
        soundEFX.volume = sliderValue ;
    }

    public void PlayIndexSoundEFXCharacterBaseOneShot(int index)
    {
        if(index < characterBaseClip.Length)
        {
            soundEFX.clip = characterBaseClip[index];
            soundEFX.PlayOneShot(soundEFX.clip);
        }
    }
    public void PlayIndexSoundEFXCharacterBase(int index)
    {
        if(index < characterBaseClip.Length)
        {
            soundEFX.clip = characterBaseClip[index];
            soundEFX.PlayOneShot(soundEFX.clip);
        }
    }
    public void PlayIndexSoundEFXHit(int index)
    {
        if(index < hitClip.Length)
        {
            soundEFX.clip = hitClip[index];
            soundEFX.PlayOneShot(soundEFX.clip);
        }
    }
    // void OnEnable()
    // {
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }

    // void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    // void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     // tìm lại slider trong scene mới
    //     musicSlider = GameObject.Find("Music_Slider").GetComponent<Slider>();
    //     soundEFX_Slider = GameObject.Find("Sound_Slider").GetComponent<Slider>();

    //     GameObject Tmusic = GameObject.Find("Music_toggle").GetComponent<GameObject>();
    //     Toggle toggle = Tmusic.GetComponent<Toggle>();
    //     toggle.onValueChanged.AddListener((bool isOn) =>
    //     {
    //         ToggleMusic();
    //     });
    //     GameObject TSound = GameObject.Find("Music_toggle").GetComponent<GameObject>();
    //     Toggle sound = TSound.GetComponent<Toggle>();
    //     sound.onValueChanged.AddListener((bool isOn) =>
    //     {
    //         ToggleSoundEFX();
    //     });
    //     SetValueSlider(); // gán lại listener
    // }
}