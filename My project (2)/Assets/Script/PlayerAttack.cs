using UnityEngine;
using Photon.Pun; // Cần thiết cho Multiplayer

public class PlayerAttack : MonoBehaviourPun // Kế thừa MonoBehaviourPun
{
    [Header("Attack Settings")]
    public int damage = 20;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

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
        // 3. TÌM KẺ ĐỊCH TRONG HITBOX
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Lấy PhotonView của đối thủ
            PhotonView enemyPV = enemy.GetComponent<PhotonView>();

            if (enemyPV != null)
            {
                // 4. GỌI RPC TAKE_DAMAGE ĐỂ ĐỒNG BỘ SÁT THƯƠNG
                // Giả sử script nhận sát thương (Health/Combat script của đối thủ)
                // cũng có component PhotonView.
                enemyPV.RPC("TakeDamage", RpcTarget.All, damage);
                Debug.Log($"Hit {enemy.gameObject.name} and called TakeDamage RPC.");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void DisableHitBox()
    {
        // Logic tắt HitBox sẽ được thêm sau, bây giờ chỉ để nó trống
        Debug.Log("HitBox Disabled.");
    }
}