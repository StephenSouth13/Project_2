using Photon.Pun;
using UnityEngine;

public class CombatHitbox : MonoBehaviourPun
{
    [Header("Attack State")] // nên để ở CombatCharacter?
    bool isAttacking = false;
    float attackTimer = 0f;
    float attackCooldown = 1f; // Thời gian giữa các đòn tấn công
    [Header("Attack Point")]
    public Transform attackPoint;
    [Header("Attack Zone")]
    public Vector2 zoneSize = new Vector2(1f, 1f);
    public float angle = 0f;
    public LayerMask targetLayers;

    void Update()
    {
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                DetectInRange();
            }
        }
    }
    public void StartAttack() => isAttacking = true;
    public void StopAttack()
    {
        Debug.Log("Stop Attack");
        isAttacking = false;
        attackTimer = 0f;
    }
    public void DetectInRange()
    {
        Collider2D[] hit = Physics2D.OverlapBoxAll(attackPoint.position, zoneSize, angle, targetLayers);
        foreach (Collider2D target in hit)
        {
            var combat = target.GetComponent<CombatCharacter>();
            if (combat == null) continue;
            if (combat.photonView.IsMine)
            {
                continue; // Không tấn công chính mình
            }
            var pv = target.GetComponent<PhotonView>();
            if (pv != null)
            {
                pv.RPC("TakeDamage", pv.Owner, 10f);
            }

        }
    }
    void OnDrawGizmosSelected() // Vẽ vùng tấn công trong Scene view
    {
        if (attackPoint == null) return;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);
        rot = attackPoint.rotation * rot;
        if(transform.localScale.x < 0)
        {
            rot *= Quaternion.Euler(0f, 0f, 180f); // Lật góc nếu nhân vật quay trái
        }
        Gizmos.matrix = Matrix4x4.TRS(attackPoint.position, rot, Vector3.one); // Thiết lập ma trận để vẽ theo vị trí và góc của attackPoint
        Gizmos.DrawWireCube(Vector3.zero, zoneSize); // Vẽ hình chữ nhật biểu diễn vùng tấn công
        Gizmos.matrix = Matrix4x4.identity; // Reset ma trận để không ảnh hưởng đến các Gizmos khác

    }
}
