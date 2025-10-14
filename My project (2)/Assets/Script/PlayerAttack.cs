//using UnityEngine;
//using Photon.Pun; // Cần thiết cho Multiplayer

//public class PlayerAttack : MonoBehaviourPun // Kế thừa MonoBehaviourPun
//{
//    [Header("Attack Settings")]
//    public int damage = 20;


//    [Header("HitBox Collider")]
//    public Collider2D swordHitBoxCollider; // Kéo và thả Box Collider 2D vào đây
//    private bool hasHit = false;

//    [Header("Cooldown")]
//    public float attackRate = 2f;
//    private float nextAttackTime = 0f;

//    private Animator anim;
//    // PhotonView đã có sẵn từ MonoBehaviourPun, không cần khai báo lại private PhotonView photonView;

//    void Start()
//    {
//        // Lấy Animator (giả sử nó nằm trên đối tượng con như đã sửa ở PlayerController)
//        anim = GetComponentInChildren<Animator>();
//        if (anim == null) Debug.LogError("Animator not found for PlayerAttack!");
//    }

//    void Update()
//    {
//        // 1. CHỈ XỬ LÝ INPUT TẤN CÔNG CHO CHÍNH NGƯỜI CHƠI NÀY
//        //if (!photonView.IsMine) return; //tạm comment để test

//        //if (Time.time >= nextAttackTime)
//        //{
//            if (Input.GetKeyDown(KeyCode.J))
//            {
//                Debug.Log("Player Attack Initiated!");
//                PerformAttack();
//               // nextAttackTime = Time.time + 1f / attackRate;
//           // }
//            }
//    }

//    void PerformAttack()
//    {
//        // 2. KÍCH HOẠT ANIMATION
//        if (anim != null)
//        {
//            // Set Trigger "Attack" (Phải có parameter này trong Animator)
//            anim.SetTrigger("Attack");
//        }

//        Debug.Log("Player đánh!");

//        // **LƯU Ý:** Logic gây sát thương (DealDamage) KHÔNG nằm ở đây nữa,
//        // mà nó sẽ được gọi bởi Animation Event tại khung hình chạm.
//    }

//    // =========================================================
//    // HÀM GÂY SÁT THƯƠNG (Được gọi bởi Animation Event)
//    // =========================================================

//    public void DealDamage() // Tên hàm này phải khớp với Animation Event
//    {
//        if (!photonView.IsMine) return;

//        if (swordHitBoxCollider != null)
//        {
//            swordHitBoxCollider.enabled = true; // BẬT HitBox (Trigger)
//            hasHit = false; // Reset cờ hit
//            Debug.Log("HitBox Activated.");
//        }
//    }



//    public void DisableHitBox() // Gọi từ Animation Event (End Swing)
//    {
//        if (swordHitBoxCollider != null)
//        {
//            swordHitBoxCollider.enabled = false; // TẮT HitBox
//            Debug.Log("HitBox Deactivated.");
//        }
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        // 1. Kiểm tra nếu HitBox chưa được kích hoạt hoặc đã hit rồi thì BỎ QUA.
//        // Giả sử swordHitBoxCollider.enabled = true chỉ khi đang swing.
//        Debug.Log($"5. OnTriggerEnter2D Fired! Va chạm với: {other.gameObject.name}");
//        if (swordHitBoxCollider == null || swordHitBoxCollider.enabled == false || hasHit) return;
//        {
//            // Log này giúp xác định va chạm xảy ra, nhưng bị chặn bởi điều kiện nào đó.
//            Debug.Log("6. OnTriggerEnter2D Aborted: HitBox null/disabled/already hit.");
//            return;
//        }
//        // 2. Chỉ xử lý va chạm của HitBox với Enemy
//        if (other.CompareTag("Enemy"))
//        {
//            // Kiểm tra đối tượng va chạm có phải là HitBox không (Không cần thiết nếu Player chỉ có 1 trigger)
//            // Nếu Player có nhiều trigger, cách xử lý tốt nhất là tạo script riêng cho HitBox!

//            // Lấy PhotonView của đối thủ
//            PhotonView enemyPV = other.GetComponent<PhotonView>();


//            if (enemyPV != null)
//            {
//                // GỌI RPC TAKE_DAMAGE ĐỂ ĐỒNG BỘ SÁT THƯƠNG
//                // Kiểm tra xem Enemy có phải là của người chơi khác không, hoặc gọi RPC trên Enemy PV.
//                // enemyPV.RPC("TakeDamage", RpcTarget.All, damage); // Sử dụng RpcTarget.All để tất cả client đồng bộ máu.
//                Debug.Log($"7. Va chạm thành công! Đã tìm thấy PhotonView của Enemy: {other.gameObject.name}. Gửi RPC...");
//                // Tốt hơn nên gọi RPC cho Enemy và chỉ gửi sát thương
//                enemyPV.RPC("TakeDamage", RpcTarget.All, damage);

//                Debug.Log($"Hit {other.gameObject.name} via Collider Trigger. Gây sát thương: {damage}");

//                // Đánh dấu đã hit
//                hasHit = true;

//                // Tắt HitBox ngay sau khi đánh trúng
//                DisableHitBox();
//            }
//        }
//        else
//        {
//            Debug.LogError($"LỖI 8: Enemy ({other.gameObject.name}) thiếu PhotonView component. RPC không thể gửi!"); // LOG 8
//        }
//    }

//}

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

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim == null) Debug.LogError("Animator not found for PlayerAttack!");
    }

    void Update()
    {
        // 1. CHỈ XỬ LÝ INPUT TẤN CÔNG CHO CHÍNH NGƯỜI CHƠI NÀY (Bỏ comment khi dùng)
        // if (!photonView.IsMine) return; 

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
            anim.SetTrigger("2_Attack");
        }
        Debug.Log("Player đánh!");
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

        // Logic bên dưới chỉ chạy khi HitBox được BẬT và chưa hit

        // 1. Chỉ xử lý va chạm với Enemy
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
        // Lỗi 8 ban đầu của bạn được đặt ở ELSE của IF(enemyPV != null)
        // Nhưng nếu đối tượng không phải Enemy, nó cũng không nên báo lỗi này.
        // Tôi đã xóa khối ELSE lỗi đó.
    }
}