using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class WeaponSpawner : MonoBehaviour
{
    [Header("Spawn motion")]
    public Vector3 startOffset = new Vector3(0, 8f, 0); // start above target
    public float fallDuration = 0.6f;
    public AnimationCurve fallCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Impact")]
    public GameObject vfxPrefab;           // particle or sprite-sequence prefab
    public AudioClip impactSfx;
    public float colliderEnableDelay = 0.05f; // allow pick up after impact
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.15f;

    [Header("References")]
    public SpriteRenderer visual;          // main weapon sprite renderer
    Collider2D pickupCollider;
    AudioSource audioSource;

    Vector3 targetPos;

    void Awake()
    {
        pickupCollider = GetComponent<Collider2D>();
        pickupCollider.enabled = false;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        if (visual == null)
        {
            visual = GetComponentInChildren<SpriteRenderer>();
        }
    }

    public void SpawnAt(Vector3 worldTargetPosition)
    {
        StopAllCoroutines();
        targetPos = worldTargetPosition;
        transform.position = targetPos + startOffset;
        pickupCollider.enabled = false;
        StartCoroutine(FallRoutine());
    }

    IEnumerator FallRoutine()
    {
        Vector3 from = transform.position;
        Vector3 to = targetPos;
        float t = 0f;
        while (t < fallDuration)
        {
            t += Time.deltaTime;
            float norm = Mathf.Clamp01(t / fallDuration);
            float curveVal = fallCurve.Evaluate(norm);
            transform.position = Vector3.LerpUnclamped(from, to, curveVal);
            yield return null;
        }
        transform.position = to;
        OnImpact();
    }

    void OnImpact()
    {
        // play VFX
        if (vfxPrefab)
        {
            var v = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
            v.transform.SetParent(null);
            // optionally auto-destroy in vfx script
        }

        // play sound
        if (impactSfx)
        {
            audioSource.PlayOneShot(impactSfx);
        }

        // enable collider after tiny delay
        StartCoroutine(EnableColliderAfterDelay());

        // camera shake (optional)
        var cam = Camera.main;
        if (cam != null)
        {
            var shaker = cam.GetComponent<CameraShake>();
            if (shaker != null) shaker.Shake(shakeDuration, shakeMagnitude);
        }
    }

    IEnumerator EnableColliderAfterDelay()
    {
        yield return new WaitForSeconds(colliderEnableDelay);
        pickupCollider.enabled = true;
    }

    // helper: auto-spawn for testing in editor
#if UNITY_EDITOR
    [ContextMenu("Test Spawn")]
    void EditorTestSpawn()
    {
        SpawnAt(transform.position);
    }
#endif
}
