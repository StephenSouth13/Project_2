using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundEntry
{
    public string key;
    public AudioClip clip;
    public float volume = 1f;
    public bool spatial = false; // true => 3D (spatialBlend=1), false => 2D
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public SoundEntry[] sounds;
    public int poolSize = 16;
    [SerializeField] private AudioMixerGroup sfxGroupOutput;

    Dictionary<string, SoundEntry> map;
    List<AudioSource> pool;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Đặt DontDestroyOnLoad ngay sau khi gán Instance
           // DontDestroyOnLoad(gameObject);

            // Khởi tạo map
            map = new Dictionary<string, SoundEntry>();
            foreach (var s in sounds)
            {
                if (!string.IsNullOrEmpty(s.key) && s.clip != null)
                {
                    map[s.key] = s;
                }
            }

            // Khởi tạo pool
            pool = new List<AudioSource>(poolSize);
            for (int i = 0; i < poolSize; i++)
            {
                var go = new GameObject("SfxSrc");
                go.transform.parent = transform;
                var src = go.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.spatialBlend = 1f; // Mặc định 3D, sẽ bị ghi đè bởi Play/Play2D

                src.outputAudioMixerGroup = sfxGroupOutput;
                pool.Add(src);
            }
        }
        else
        {
            // Hủy bản sao
            Destroy(gameObject);
            return;
        }
    }

    AudioSource GetFreeSource()
    {
        for (int i = 0; i < pool.Count; i++)
            if (!pool[i].isPlaying) return pool[i];

        // If all busy, expand pool a bit
        var go = new GameObject("SfxSrc_extra");
        go.transform.parent = transform;
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.spatialBlend = 1f;
        pool.Add(src);
        return src;
    }

    public void Play(string key, Vector3 position, float volume = 1f)
    {
        if (!map.TryGetValue(key, out var e) || e.clip == null) return;
        var src = GetFreeSource();
        src.transform.position = position;
        src.spatialBlend = e.spatial ? 1f : 0f;
        src.PlayOneShot(e.clip, e.volume * volume);
    }

    public void Play2D(string key, float volume = 1f)
    {
        if (!map.TryGetValue(key, out var e) || e.clip == null) return;
        var src = GetFreeSource();
        src.transform.position = Vector3.zero;
        src.spatialBlend = 0f;
        src.PlayOneShot(e.clip, e.volume * volume);
    }
}