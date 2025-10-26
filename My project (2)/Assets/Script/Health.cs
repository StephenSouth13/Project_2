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