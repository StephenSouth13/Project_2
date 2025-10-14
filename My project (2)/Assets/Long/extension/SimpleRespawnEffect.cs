using System.Collections;
using UnityEngine;

public class SimpleRespawnEffect : MonoBehaviour
{
    public GameObject vfxPrefab;
    public AudioClip respawnSfx;
    public float fallDuration = 0.8f;
    public Vector3 startOffset = new Vector3(0, 10f, 0);

    AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public IEnumerator DoRespawn(Vector3 targetPosition, System.Action onArrived = null)
    {
        transform.position = targetPosition + startOffset;
        // falling motion (ease out)
        float t = 0f;
        Vector3 from = transform.position;
        Vector3 to = targetPosition;
        while (t < fallDuration)
        {
            t += Time.deltaTime;
            float norm = Mathf.Clamp01(t / fallDuration);
            float eased = 1 - Mathf.Pow(1 - norm, 3); // easeOutCubic
            transform.position = Vector3.LerpUnclamped(from, to, eased);
            yield return null;
        }
        transform.position = to;

        // spawn VFX
        if (vfxPrefab) Instantiate(vfxPrefab, to, Quaternion.identity);
        if (respawnSfx) audioSource.PlayOneShot(respawnSfx);

        onArrived?.Invoke();
    }
}
