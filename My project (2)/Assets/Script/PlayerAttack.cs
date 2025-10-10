using UnityEngine;
using Photon.Pun; // Cần thiết cho Multiplayer

public class PlayerAttack : MonoBehaviourPun // Kế thừa MonoBehaviourPun
{
    [Header("Attack Settings")]
    public int damage = 20;
    

    [Header("HitBox Collider")]
    public Collider2D swordHitBoxCollider; // Kéo và thả Box Collider 2D vào đây
    private bool hasHit = false;

    [Header("Cooldown")]
    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    private Animator anim;
    // PhotonView đã có sẵn từ MonoBehaviourPun, không cần khai báo lại private PhotonView photonView;

    void Start()
    {
        // Lấy Animator (giả sử nó nằm trên đối tượng con như đã sửa ở PlayerController)
        anim = GetComponentInChildren<Animator>();
        if (anim == null) Debug.LogError("Animator not found for PlayerAttack!");
    }

    void Update()
    {
        // 1. CHỈ XỬ LÝ INPUT TẤN CÔNG CHO CHÍNH NGƯỜI CHƠI NÀY
        //if (!photonView.IsMine) return; //tạm comment để test

        //if (Time.time >= nextAttackTime)
        //{
            if (Input.GetKeyDown(KeyCode.J))
            {
                Debug.Log("Player Attack Initiated!");
                PerformAttack();
               // nextAttackTime = Time.time + 1f / attackRate;
           // }
        }
    }

    void PerformAttack()
    {
        // 2. KÍCH HOẠT ANIMATION
        if (anim != null)
        {
            // Set Trigger "Attack" (Phải có parameter này trong Animator)
            anim.SetTrigger("Attack");
        }

        Debug.Log("Player đánh!");

        // **LƯU Ý:** Logic gây sát thương (DealDamage) KHÔNG nằm ở đây nữa,
        // mà nó sẽ được gọi bởi Animation Event tại khung hình chạm.
    }

    // =========================================================
    // HÀM GÂY SÁT THƯƠNG (Được gọi bởi Animation Event)
    // =========================================================

    public void DealDamage() // Tên hàm này phải khớp với Animation Event
    {
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
        if (other == swordHitBoxCollider || swordHitBoxCollider.enabled == false) return;
        // Chỉ xử lý va chạm của HitBox mà bạn vừa bật
        if (other.CompareTag("Enemy") || other.GetComponent<Health>() != null)
        {
            if (hasHit) return;
            // Lấy PhotonView của đối thủ (Kiểm tra lại Enemy Layers)
            PhotonView enemyPV = other.GetComponent<PhotonView>();

            if (enemyPV != null)
            {
                // GỌI RPC TAKE_DAMAGE ĐỂ ĐỒNG BỘ SÁT THƯƠNG
                enemyPV.RPC("TakeDamage", RpcTarget.All, damage);
                Debug.Log($"Hit {other.gameObject.name} via Collider Trigger.");

                // Đánh dấu đã hit để tránh gây sát thương lặp lại
                hasHit = true;

                // Tắt HitBox ngay sau khi đánh trúng (để chỉ đánh 1 lần/lượt vung)
                DisableHitBox();
            }
        }
    }
}