using System.Collections;
using UnityEngine;

// Gắn vào Prefab "Cột sáng rơi" (WeaponDropBeam)
public class SimpleRespawnEffect : MonoBehaviour
{
    [Header("Prefabs")]
    // Kéo Prefab "Vũ khí" (cây kiếm, búa...) vào đây
    public GameObject itemToSpawnPrefab;
    // Kéo Prefab "Hiệu ứng nổ" (ImpactVFX) vào đây
    public GameObject impactVfxPrefab;

    [Header("Audio")]
    public AudioClip respawnSfx; // Âm thanh khi va chạm

    [Header("Movement")]
    public float fallDuration = 0.8f; // Thời gian rơi
    public Vector3 startOffset = new Vector3(0, 10f, 0); // Vị trí bắt đầu (cách 10f)

    AudioSource audioSource;

    void Awake()
    {
        // Thêm component AudioSource để sẵn sàng chơi âm thanh
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // Âm thanh 3D
    }

    // Coroutine này được gọi bởi "WeaponSpawner"
    // Nó trả về "GameObject" (vũ khí đã spawn) qua một callback
    public IEnumerator DoRespawn(Vector3 targetPosition, System.Action<GameObject> onWeaponSpawned = null)
    {
        // 1. Đặt vị trí ban đầu (ở trên trời)
        transform.position = targetPosition + startOffset;

        // 2. Bắt đầu di chuyển (rơi xuống)
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
        transform.position = to; // Đảm bảo đến đúng vị trí

        // 3. TẠO RA HIỆU ỨNG NỔ (ImpactVFX)
        if (impactVfxPrefab)
        {
            Instantiate(impactVfxPrefab, to, Quaternion.identity);
        }

        // 4. TẠO RA VŨ KHÍ (Item)
        GameObject spawnedWeapon = null;
        if (itemToSpawnPrefab)
        {
            spawnedWeapon = Instantiate(itemToSpawnPrefab, to, Quaternion.identity);
        }

        // 5. CHƠI ÂM THANH
        // Dùng PlayClipAtPoint vì object này sắp bị hủy
        if (respawnSfx)
        {
            AudioSource.PlayClipAtPoint(respawnSfx, to);
        }

        // 6. GỌI CALLBACK
        // Báo cho script gọi nó (WeaponSpawner) biết là đã spawn xong
        onWeaponSpawned?.Invoke(spawnedWeapon);

        // 7. TỰ HỦY
        // Nhiệm vụ của "cột sáng" đã xong
        Destroy(gameObject);
    }
}