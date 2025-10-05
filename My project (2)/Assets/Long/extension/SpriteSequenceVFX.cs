using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSequenceVFX : MonoBehaviour
{
    public Sprite[] frames;
    public float fps = 24f;
    public bool loop = false;
    public bool destroyOnEnd = true;

    SpriteRenderer sr;
    float frameTime;
    int idx = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (frames == null || frames.Length == 0)
        {
            Debug.LogWarning("No frames set for SpriteSequenceVFX on " + name);
            enabled = false;
            return;
        }
        frameTime = 1f / fps;
    }

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
                    if (destroyOnEnd) Destroy(gameObject);
                    yield break;
                }
            }
            yield return new WaitForSeconds(frameTime);
        }
    }
}
