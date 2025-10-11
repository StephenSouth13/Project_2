Tuyệt — mình sẽ cung cấp bộ code Unity (C#) hoàn chỉnh để hiện thực hoá cơ chế weapon spawn / player spawn kiểu “vũ khí/nhân vật rơi từ trời xuống rồi phát nổ VFX khi chạm đất” cho Thiên Hùng Ca.
Mình viết rõ cả prefab/scene setup, script và gợi ý animation/sprite-sheet để bạn kéo thẳng vào Unity và test.

🚀 Tổng quan
gg doc: https://docs.google.com/document/d/1ZsVT1hOL9F2-MVMMQfAKY8E7U-1GrSl86zh0LBSviMk/edit?usp=sharing

Mục tiêu: Spawn item (weapon) từ trên trời, di chuyển xuống với easing/gravity, khi chạm đất:

phát VFX (sprite sequence hoặc ParticleSystem),

phát âm thanh,

bật collider cho người chơi nhặt,

(tùy chọn) rung camera / tạo impact forces.

Tương tự cho player respawn: hiệu ứng rơi → impact → invulnerability window.

🧩 Prefab / Asset Setup (trong Unity)

WeaponSpawnPrefab (GameObject)

Child Visual có SpriteRenderer (hiển thị weapon sprite).

Child VFX (empty) chứa SpriteSequenceVFX (script) or ParticleSystem.

Collider2D (disabled initially) — để player pick up after impact.

Rigidbody2D (optional) — nếu muốn physics fall (mình dùng LERP in code so Rigidbody optional).

Attach script: WeaponSpawner.cs.

PlayerRespawnPrefab (GameObject) / or reuse WeaponSpawnPrefab but different VFX.

VFX for respawn (dragon-cloud, aura).

Script: RespawnEffect.cs.

Audio: AudioClip for spawn/impact.

Camera: attach optional CameraShake script.

✅ Script 1 — WeaponSpawner.cs
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


Ghi chú: SpawnAt(Vector3) bạn gọi khi muốn spawn (ví dụ GameManager gọi Instantiate(prefab); prefab.SpawnAt(targetPos);).

✅ Script 2 — SpriteSequenceVFX.cs (dùng cho sprite-frame VFX)

Dùng khi bạn xuất VFX thành PNG sequence (frames) để animate VFX.

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


Cách dùng: tạo prefab VFX_SpearImpact chứa SpriteRenderer + SpriteSequenceVFX, kéo mảng frames (Impact_01..Impact_06).

✅ Script 3 — CameraShake (optional)
using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    Vector3 originalPos;
    void Awake() => originalPos = transform.localPosition;

    public void Shake(float duration, float magnitude)
    {
        StopAllCoroutines();
        StartCoroutine(DoShake(duration, magnitude));
    }

    IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp01(percentComplete);
            float x = (Random.value * 2f - 1f) * magnitude * damper;
            float y = (Random.value * 2f - 1f) * magnitude * damper;
            transform.localPosition = originalPos + new Vector3(x, y, 0);
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}

✅ Script 4 — SimpleRespawnEffect.cs (player respawn)
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


Use-case: when player respawns, disable input, start coroutine StartCoroutine(respawnEffect.DoRespawn(spawnPos, () => { enable input; start invul timer; }));

🔧 Example: GameManager snippet to spawn weapon
public class GameManager : MonoBehaviour
{
    public WeaponSpawner weaponPrefab; // assign prefab (with WeaponSpawner component)
    public Transform arenaCenter;

    public void SpawnWeaponAtRandom()
    {
        Vector3 target = GetRandomArenaPosition();
        var go = Instantiate(weaponPrefab.gameObject);
        var sp = go.GetComponent<WeaponSpawner>();
        sp.SpawnAt(target);
    }

    Vector3 GetRandomArenaPosition()
    {
        // implement based on your arena bounds
        return arenaCenter.position + new Vector3(Random.Range(-3f, 3f), 0, 0);
    }
}

🧭 Unity Workflow checklist

Create sprite frames for VFX (weapon impact frames), put into VFX_SpearImpact prefab with SpriteSequenceVFX.

Create Weapon prefab:

Add Collider2D (set isTrigger = true if using trigger pickup).

Add WeaponSpawner script and assign visual, vfxPrefab (the prefab made above), impactSfx.

GameManager calls Instantiate(weaponPrefab) and SpawnAt(targetPos).

Pickup logic: your player script listens for OnTriggerEnter2D(Collider2D other) and checks other.CompareTag("WeaponPickup") to pick up.

Respawn: call SimpleRespawnEffect.DoRespawn() on the player prefab when respawning.

🖼️ Gợi ý kỹ thuật cho sprite/VFX

VFX frames: export 6–10 frames: Impact_01.png … Impact_06.png, transparent background, centered.

Sprite sheet: bạn có thể xuất thẳng sprite sheet, hoặc upload frames and assign to SpriteSequenceVFX.

Scale & Pivot: set sprite pivot to Bottom Center so it aligns to ground.

✅ Tóm tắt ngắn

WeaponSpawner cho chuyển động rơi (LERP/easing) → gọi OnImpact → spawn SpriteSequenceVFX/Particle → enable collider → play sfx → (camera shake).

SimpleRespawnEffect cho player respawn tương tự.

Chuẩn bị VFX frames hoặc ParticleSystem; đặt vào prefab; kết nối vào script.