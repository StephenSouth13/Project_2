////using UnityEngine;

////public class Health : MonoBehaviour
////{
////    [Header("Health Settings")]
////    public int maxHealth = 100;
////    private int currentHealth;

////    void Start()
////    {
////        currentHealth = maxHealth;
////    }

////    public void TakeDamage(int damage)
////    {
////        currentHealth -= damage;
////        Debug.Log(gameObject.name + " took " + damage + " damage. HP: " + currentHealth);

////        if (currentHealth <= 0)
////        {
////            Die();
////        }
////    }

////    void Die()
////    {
////        Debug.Log(gameObject.name + " has died!");
////        Destroy(gameObject); // Sau này có thể thay bằng anim Die
////    }
////}

//using UnityEngine;
//using Photon.Pun; // Cần thiết để sử dụng [PunRPC]

//public class Health : MonoBehaviourPun // Kế thừa MonoBehaviourPun để sử dụng RPC
//{
//    [Header("Health")]
//    public int maxHP = 100;
//    [SerializeField]
//    private int currentHP;
//    private Animator anim;

//    // Tham chiếu đến các script khác để disable khi chết
//    private PlayerController playerController;
//    private PlayerAttack playerAttack;

//    void Start()
//    {
//        currentHP = maxHP;
//        anim = GetComponentInChildren<Animator>();
//        playerController = GetComponent<PlayerController>();
//        playerAttack = GetComponent<PlayerAttack>();

//        // Hiện tại, tất cả các đối tượng (Player và DummyEnemy) cần có
//        // Health script và PhotonView để RPC hoạt động.
//    }

//    // =========================================================
//    // HÀM NHẬN SÁT THƯƠNG (RPC)
//    // =========================================================
//    [PunRPC]
//    public void TakeDamage(int damage)
//    {
//        // 1. Chỉ Host mới xử lý logic sát thương, sau đó RPC. 
//        // Nhưng vì PlayerAttack.cs đã gọi RPC Target.All, ta chỉ cần xử lý HP ở đây.

//        if (currentHP <= 0) return; // Đã chết thì không nhận sát thương

//        currentHP -= damage;
//        Debug.Log(gameObject.name + " took " + damage + " damage. HP remaining: " + currentHP);

//        // 2. Kích hoạt hoạt hình bị đánh (DAMAGED)
//        if (anim != null)
//        {
//            anim.SetTrigger("Damaged"); // Giả sử bạn có Trigger "Damaged"
//        }

//        // 3. Logic chết
//        if (currentHP <= 0)
//        {
//            Die();
//        }
//    }

//    void Die()
//    {
//        // Ghi lại sự kiện chết
//        Debug.Log(gameObject.name + " has died!");

//        // 1. Play Anim Die
//        if (anim != null)
//        {
//            anim.SetBool("IsDeath", true); // Giả sử bạn có Parameter "IsDeath" (Bool)
//        }

//        // 2. Disable điều khiển
//        if (playerController != null)
//        {
//            playerController.enabled = false;
//        }
//        if (playerAttack != null)
//        {
//            playerAttack.enabled = false;
//        }

//        // 3. (Tạm thời) Vô hiệu hóa Collider/Rigidbody để nhân vật rơi xuống/ngừng va chạm
//        GetComponent<Collider2D>().enabled = false;
//        GetComponent<Rigidbody2D>().simulated = false;

//        // Yêu cầu dự án là "Gọi RPC để sync chết giữa 2 máy"
//        // Vì TakeDamage đã là RPC, Die() cũng đang được sync.
//    }
//}

using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviourPun // ĐÚNG: Kế thừa MonoBehaviourPun
{
    [Header("Health")]
    public int maxHP = 100;
    [SerializeField]
    private int currentHP;
    private Animator anim;

    private PlayerController playerController;
    private PlayerAttack playerAttack;

    void Start()
    {
        currentHP = maxHP;
        anim = GetComponentInChildren<Animator>();

        // Chỉ Player mới có các script này, Enemy sẽ trả về null (và đó là OK)
        playerController = GetComponent<PlayerController>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    // =========================================================
    // HÀM NHẬN SÁT THƯƠNG (RPC)
    // =========================================================
    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (currentHP <= 0) return; // Đã chết thì không nhận sát thương

        currentHP -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. HP remaining: " + currentHP);

        // Kích hoạt hoạt hình bị đánh (DAMAGED)
        if (anim != null)
        {
            anim.SetTrigger("Damaged");
        }

        // Logic chết
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died!");

        // 1. Play Anim Die
        if (anim != null)
        {
            anim.SetBool("IsDeath", true);
        }

        // 2. Disable điều khiển (Chỉ ảnh hưởng đến Player)
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        if (playerAttack != null)
        {
            playerAttack.enabled = false;
        }

        // 3. Vô hiệu hóa Collider/Rigidbody (Cần kiểm tra Null cho an toàn)
        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
        }

        // 4. Hủy đối tượng trên mạng (CHỈ OWNER hoặc MASTER CLIENT được làm)
        // Gọi hàm này sau một khoảng thời gian (delay cho Anim Die) hoặc từ Animation Event
        // Ở đây ta gọi ngay lập tức, nhưng trong game thực tế, bạn nên delay.
        // if (photonView.IsMine) 
        // {
        //     Invoke("CleanUpAfterDeath", 3f); // Ví dụ: delay 3 giây
        // }
    }

    // =========================================================
    // HÀM HỦY ĐỐI TƯỢNG (Được gọi sau Anim Die)
    // =========================================================
    // HÀM NÀY CẦN ĐƯỢC GỌI TỪ ANIMATION EVENT HOẶC SAU MỘT DELAY
    public void CleanUpAfterDeath()
    {
        // CHỈ HỦY nếu đối tượng này thuộc sở hữu của client này
        // (Đây là cách an toàn nhất)
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}