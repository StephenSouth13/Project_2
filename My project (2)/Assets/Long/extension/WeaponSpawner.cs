using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gắn vào một GameObject trống tên là "_WeaponSpawner"
public class WeaponSpawner : MonoBehaviour
{
    [Header("Prefab")]
    // Kéo Prefab "Cột sáng rơi" (WeaponDropBeam) vào đây
    public GameObject weaponDropBeamPrefab;

    [Header("Timing")]
    public float initialDelay = 5f; // Chờ 5s cho lần đầu
    public float spawnInterval = 15f; // Lặp lại mỗi 15s

    [Header("Targeting (Tìm đất)")]
    // Chọn Layer là "Ground" hoặc "Map"
    public LayerMask groundLayer;
    // Phạm vi X ngẫu nhiên (tính từ vị trí của Spawner này)
    public float spawnRangeX = 15f;
    // Độ cao để bắt đầu bắn tia (phải đủ cao trên màn hình)
    public float raycastStartHeight = 20f;

    void Start()
    {
        // Bắt đầu vòng lặp tự động
        InvokeRepeating("AttemptToSpawnWeapon", initialDelay, spawnInterval);
    }

    void AttemptToSpawnWeapon()
    {
        // 1. TÌM VỊ TRÍ X NGẪU NHIÊN
        float randomX = transform.position.x + Random.Range(-spawnRangeX, spawnRangeX);

        // 2. VỊ TRÍ BẮN TIA (trên trời)
        Vector2 rayStart = new Vector2(randomX, transform.position.y + raycastStartHeight);

        // 3. BẮN TIA (Raycast)
        // Bắn 1 tia thẳng xuống (Vector2.down), dài 100f, chỉ tìm layer "groundLayer"
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, 100f, groundLayer);

        // 4. KIỂM TRA KẾT QUẢ
        if (hit.collider != null)
        {
            // Bắn trúng đất! "hit.point" là tọa độ chạm
            Vector3 dropPosition = hit.point;
            SpawnItem(dropPosition);
        }
        else
        {
            // Bắn trượt (rơi vào hố), không làm gì cả
            Debug.Log("Weapon Spawner: Không tìm thấy đất tại X = " + randomX);
        }
    }

    // Hàm này gọi "Cột sáng rơi"
    void SpawnItem(Vector3 dropPosition)
    {
        // 1. Tạo ra cái "Beam"
        GameObject beam = Instantiate(weaponDropBeamPrefab);

        // 2. Ra lệnh cho nó "rơi"
        StartCoroutine(beam.GetComponent<SimpleRespawnEffect>().DoRespawn(dropPosition, (spawnedWeapon) =>
        {
            if (spawnedWeapon != null)
            {
                Debug.Log("Đã giao thành công: " + spawnedWeapon.name);
                // Bạn có thể thêm code ở đây để
                // ví dụ: add vũ khí vào 1 danh sách...
            }
        }));
    }
}