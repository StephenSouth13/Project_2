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
    public float attackRate = 2f;
    private float nextAttackTime = 0f; // Bắt đầu từ 0 để cho phép tấn công ngay

    private Animator anim;

    // ĐỊNH NGHĨA KEY SFX "chém trượt"
    private const string ATTACK_SFX_KEY = "Missed";

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim == null) Debug.LogError("Animator not found for PlayerAttack!");
    }

    void Update()
    {
        // QUAN TRỌNG: Chỉ client sở hữu mới xử lý Input
        if (!photonView.IsMine) return;

        // KIỂM TRA INPUT và COOLDOWN
        if (Input.GetKeyDown(KeyCode.J)) //&& Time.time >= nextAttackTime)
        {
            Debug.Log("Player Attack Initiated!");
            PerformAttack();
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

    void PerformAttack()
    {
        // 1. Đặt Cooldown
        nextAttackTime = Time.time + 1f / attackRate;

        // 2. Kích hoạt Animation
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }
        Debug.Log("Player đánh!");
        Rpc_PlaySFX(ATTACK_SFX_KEY); // Gọi trực tiếp hàm RPC để phát SFX ngay lập tức
        
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
}