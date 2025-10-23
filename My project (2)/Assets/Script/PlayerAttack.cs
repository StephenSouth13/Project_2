using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPun
{
    [Header("Attack Settings")]
    public int damage = 20;

    [Header("HitBox Collider")]
    public Collider2D swordHitBoxCollider;
    private bool hasHit = false;

    [Header("Cooldown")]
    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    private Animator anim;
    public AudioSource audioSource;
    public AudioClip attackSound;


    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim == null) Debug.LogError("Animator not found for PlayerAttack!");

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Player Attack Initiated!");
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }
        Debug.Log("Player đánh!");
        PlaySFX(attackSound);
    }

    // =========================================================
    // HÀM GÂY SÁT THƯƠNG (Được gọi bởi Animation Event)
    // =========================================================
    public void DealDamage() // Tên hàm này phải khớp với Animation Event
    {
        // QUAN TRỌNG: Chỉ client sở hữu Player mới được khởi động logic sát thương
        if (!photonView.IsMine) return;

        if (swordHitBoxCollider != null)
        {
            swordHitBoxCollider.enabled = true; // BẬT HitBox (Trigger)
            hasHit = false; // Reset cờ hit
            Debug.Log("HitBox Activated.");
        }
    }

    public void DisableHitBox() // Gọi từ Animation Event (End Swing)
    {
        if (swordHitBoxCollider != null)
        {
            swordHitBoxCollider.enabled = false; // TẮT HitBox
            Debug.Log("HitBox Deactivated.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // LOG VÀ KIỂM TRA ĐIỀU KIỆN CHẶN
        Debug.Log($"5. OnTriggerEnter2D Fired! Va chạm với: {other.gameObject.name}");

        // SỬA LỖI: BỎ KHỐI {} KHÔNG CẦN THIẾT
        if (swordHitBoxCollider == null || swordHitBoxCollider.enabled == false || hasHit)
        {
            // Log này chỉ xuất hiện NẾU va chạm xảy ra nhưng bị chặn bởi các cờ
            Debug.Log("6. OnTriggerEnter2D Aborted: HitBox null/disabled/already hit.");
            return; // Lệnh RETURN thoát khỏi hàm nếu điều kiện chặn đúng
        }

        // 1. Chỉ xử lý va chạm với Enemy
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
       
    }

    void PlaySFX(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}