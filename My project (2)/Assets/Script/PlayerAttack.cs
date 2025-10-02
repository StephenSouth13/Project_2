using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 20;                  // Lượng sát thương gây ra
    public Transform attackPoint;            // Vị trí xuất phát HitBox
    public float attackRange = 0.5f;         // Bán kính HitBox
    public LayerMask enemyLayers;            // Layer kẻ địch

    [Header("Cooldown")]
    public float attackRate = 2f;            // Số đòn / giây
    private float nextAttackTime = 0f;

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.J)) // Nhấn J để đánh (tạm thời)
            {
                PerformAttack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void PerformAttack()
    {
        // Hiện debug trên Scene (Editor)
        Debug.Log("Player Attack!");

        // Tìm các enemy trong HitBox
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Gọi TakeDamage từ Health script
            enemy.GetComponent<Health>()?.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}