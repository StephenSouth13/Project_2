using System.Collections;
using UnityEngine;

// Gắn vào Prefab "Hiệu ứng nổ" (ImpactVFX)
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSequenceVFX : MonoBehaviour
{
    public Sprite[] frames;
    public float fps = 24f;
    public bool loop = false; // Luôn để là false
    public bool destroyOnEnd = true; // Luôn để là true

    SpriteRenderer sr;
    float frameTime;
    int idx = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (frames == null || frames.Length == 0)
        {
            Debug.LogWarning("Không có frames cho VFX on " + name);
            enabled = false;
            return;
        }
        frameTime = 1f / fps;
    }

    // Tự động chạy khi được Instantiate
    void OnEnable()
    {
        idx = 0;
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        while (true)
        {
            sr.sprite = frames[idx];
            idx++;
            if (idx >= frames.Length)
            {
                if (loop)
                {
                    idx = 0;
                }
                else
                {
                    // Đã chạy hết animation
                    if (destroyOnEnd) Destroy(gameObject); // Tự hủy
                    yield break;
                }
            }
            yield return new WaitForSeconds(frameTime);
        }
    }
}