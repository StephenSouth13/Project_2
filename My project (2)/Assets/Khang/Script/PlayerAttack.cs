using UnityEngine;
using Photon.Pun;


public class PlayerAttack : MonoBehaviourPun
{
    // ... (Giữ nguyên các khai báo khác)
    [Header("Attack Settings")]
    public int damage = 20;

    [Header("HitBox Collider")]
    public Collider2D swordHitBoxCollider;
    private bool hasHit = false;

    [Header("Cooldown")]
    public float attackRate = 1f;
    private float nextAttackTime = 0f; // Bắt đầu từ 0 để cho phép tấn công ngay

    [Header("Components")]
    public Animator characterAnimator;

    // Thêm vào đầu script, trong phần khai báo biến
    [Header("Attack VFX")]
    public GameObject retroImpactVFXPrefab; // Sẽ kéo Prefab vào đây
    public Transform vfxSpawnPoint; // Sẽ kéo GameObject rỗng làm điểm xuất hiện vào đây

    // ĐỊNH NGHĨA KEY SFX "chém trượt"
    private const string ATTACK_SFX_KEY = "Missed";

    [Header("DarkSaber Attack")]
    public GameObject darkSaberPrefab;
    public Transform darkSaberSpawnPoint;

    [Header("Shield Defense")]
    public Collider2D shieldCollider; // Collider vật lý của khiên (nếu có)
    public GameObject shieldVFXPrefab; // Prefab của VFX khiên (Đã có animation)
    public Transform shieldVFXSpawnPoint; // Vị trí xuất hiện VFX

    [Header("Dash Settings")]
    // Bạn cần gắn script quản lý di chuyển (ví dụ: PlayerMovement) vào đây
    public MonoBehaviour playerMovementScript;
    // Nếu bạn muốn đặt cooldown riêng cho Dash:
    public float dashCooldown = 1f;
    private float nextDashTime = 0f;

    // ĐỊNH NGHĨA KEY SFX "đỡ đòn" (Tùy chọn)
    private const string DEFENSE_SFX_KEY = "Shield_Block";

    void Start()
    {
        //anim = GetComponentInChildren<Animator>();
        //if (anim == null) Debug.LogError("Animator not found for PlayerAttack!");
        // Giờ dùng characterAnimator.SetTrigger("Attack")
        if (characterAnimator == null) Debug.LogError("Animator NOT assigned in Inspector!");
    }

    void Update()
    {
        // QUAN TRỌNG: Chỉ client sở hữu mới xử lý Input
        if (!photonView.IsMine) return;

        // KIỂM TRA INPUT và COOLDOWN
        if (Input.GetKeyDown(KeyCode.J)) 
        {
            Debug.Log("Player Attack Initiated!");
            PerformAttack(1);
        }
        else if (Input.GetKeyDown(KeyCode.K)) //&& Time.time >= nextAttackTime) // Bạn có thể thêm cooldown nếu muốn
        {
            Debug.Log("Player DarkSaber Attack Initiated (K)!");
            PerformAttack(2); // GỌI PERFORM ATTACK VỚI INDEX 2
        }
        else if(Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Player Defense Initiated (I)");
            PerformDefense();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Player Dash Initiated (L)");
            PerformDash();
        }

        float dt = Time.deltaTime;
        Debug.Log("ABCXYZ" + dt );
        Debug.Log($"ABCXYZ: {dt}");
        
        int frame = Time.frameCount;
        Debug.LogFormat("ABCXYZ - Delta Time: {0}, Frame: {1}", dt, frame);
    }
    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        
    }
    private void LateUpdate()
    {
        
    }

    void PerformAttack(int attackIndex)
    {
        // 1. Đặt Cooldown
        nextAttackTime = Time.time + 1f / attackRate;

        // 2. Kích hoạt Animation
        //if (characterAnimator != null) // Dùng tên biến mới
        //{
        //    characterAnimator.SetTrigger("Attack");
        //}
        //Debug.Log("Player đánh!");
        //Rpc_PlaySFX(ATTACK_SFX_KEY); // Gọi trực tiếp hàm RPC để phát SFX ngay lập tức
        if (characterAnimator != null) // Dùng tên biến mới
        {
            if (attackIndex == 1)
            {
                characterAnimator.SetTrigger("Attack"); // Trigger cho đòn chém cũ
            }
            else if (attackIndex == 2)
            {
                // Trigger cho đòn phóng phi tiêu (Cần tạo Trigger này trong Unity Animator)
                characterAnimator.SetTrigger("DarkSaberAttack");
            }
        }

        Debug.Log($"Player đánh đòn {attackIndex}!");
        Rpc_PlaySFX(ATTACK_SFX_KEY);
    }

    // =========================================================
    // HÀM RPC (ĐỒNG BỘ MẠNG)
    // =========================================================
   
    void Rpc_PlaySFX(string sfxKey)
    {
        // TẤT CẢ client nhận lệnh này và phát âm thanh qua AudioManager
        if (AudioManager.Instance != null)
        {
            // Âm thanh 2D (Play2D) thường dùng cho SFX của Player/UI
            AudioManager.Instance.Play2D(sfxKey);
        }
    }


    // =========================================================
    // HÀM GÂY SÁT THƯƠNG (Được gọi bởi Animation Event)
    // =========================================================
    public void DealDamage()
    {
        // ... (Giữ nguyên logic HitBox và IsMine check)
        if (!photonView.IsMine) return;

        if (swordHitBoxCollider != null)
        {
            swordHitBoxCollider.enabled = true;
            hasHit = false;
            Debug.Log("HitBox Activated.");
        }
    }

    public void DisableHitBox()
    {
        if (swordHitBoxCollider != null)
        {
            swordHitBoxCollider.enabled = false;
            Debug.Log("HitBox Deactivated.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"5. OnTriggerEnter2D Fired! Va chạm với: {other.gameObject.name}");

        // Kiểm tra điều kiện chặn (null/disabled/already hit)
        if (swordHitBoxCollider == null || swordHitBoxCollider.enabled == false || hasHit)
        {
            Debug.Log("6. OnTriggerEnter2D Aborted: HitBox null/disabled/already hit.");
            return;
        }

        // 1. Chỉ xử lý va chạm với Enemy
        if (other.CompareTag("Enemy"))
        {
            // Tạm thời Destroy đối tượng (theo code cũ)
            Destroy(other.gameObject);

            hasHit = true; // Đánh dấu đã trúng để tránh gây sát thương nhiều lần (multi-hit)
        }
    }

    public void SpawnAttackVFX()
    {

        // Kiểm tra Prefab và Spawn Point
        if (retroImpactVFXPrefab == null)
        {
            Debug.LogWarning("Retro Impact VFX Prefab is not assigned in the Inspector!");
            return;
        }

        if (vfxSpawnPoint == null)
        {
            Debug.LogError("VFX Spawn Point is not assigned! Cannot spawn VFX.");
            return;
        }

        Vector3 spawnPosition = vfxSpawnPoint.position;

        GameObject vfx = Instantiate(retroImpactVFXPrefab, spawnPosition, Quaternion.identity);

        float direction = transform.localScale.x;

        // Áp dụng lật
        Vector3 currentVFXScale = vfx.transform.localScale;
        vfx.transform.localScale = new Vector3(
            currentVFXScale.x * direction, // Dùng 'direction' đã tính
            currentVFXScale.y,
            currentVFXScale.z
        );

    }

    public void SpawnDarkSaber()
    {
        if (darkSaberPrefab == null || darkSaberSpawnPoint == null) return;
        float direction = Mathf.Sign(transform.localScale.x);

        GameObject darkSaber = Instantiate(
            darkSaberPrefab,
            darkSaberSpawnPoint.position,
            Quaternion.identity
        );

        DarkSaber DarkSaber = darkSaber.GetComponent<DarkSaber>();
        if (DarkSaber != null)
        {
            DarkSaber.Initialize(direction);
        }
    }  
    
    void PerformDefense()
    {
        nextAttackTime = Time.time + 1f/attackRate;
        if (characterAnimator != null)
        {
            characterAnimator.SetTrigger("Defense");
        }
    }

    // TRONG PlayerAttack.cs

    // HÀM GỌI BỞI ANIMATION EVENT
    public void SpawnShieldVFX()
    {
        if (shieldVFXPrefab == null || shieldVFXSpawnPoint == null)
        {
            Debug.LogError("Shield VFX Prefab or Spawn Point is missing!");
            return;
        }

        // 1. Tạo VFX tại vị trí
        GameObject vfx = Instantiate(
            shieldVFXPrefab,
            shieldVFXSpawnPoint.position,
            Quaternion.identity, // Giữ nguyên xoay
            shieldVFXSpawnPoint
        );

        // 2. Lật VFX theo hướng nhân vật (Rất quan trọng)
        float direction = Mathf.Sign(transform.localScale.x);

        Vector3 currentVFXScale = vfx.transform.localScale;
        vfx.transform.localScale = new Vector3(
            currentVFXScale.x * direction,
            currentVFXScale.y,
            currentVFXScale.z
        );

        // (VFX này phải có script AutoDestroy.cs để tự hủy sau 1-2 giây)
    }

    // Bạn cũng có thể thêm hàm sau để điều khiển Collider:
    public void EnableShieldCollider()
    {
        if (shieldCollider != null) shieldCollider.enabled = true;
    }

    public void DisableShieldCollider()
    {
        if (shieldCollider != null) shieldCollider.enabled = false;
    }

    void PerformDash()
    {
        // 1. Đặt Cooldown riêng cho Dash (để không bị spam)
        nextDashTime = Time.time + dashCooldown;

        // 2. Kích hoạt Animation Dash (Cần Trigger "Dash" trong Animator)
        if (characterAnimator != null)
        {
            characterAnimator.SetTrigger("Dash");
        }

        
    }



}