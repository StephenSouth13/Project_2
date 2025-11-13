using System.Collections.Generic;
using NUnit.Framework;
using Photon.Pun;
using UnityEngine;

public class CombatHitbox : MonoBehaviourPun
{
    // hợp tác với EFXManager để spawn tính đame. trước mắt sẽ getdame trước sau đó sẽ truyền zoneSize vào để khớp với Anim
    [Header("Attack State")] // nên để ở CombatCharacter?
    public bool isAttacking = false;
    float attackTimer = 0f; // Tạm thời chưa dùng đến
    float attackCooldown = 1f; // Tạm thời chưa dùng đến
    [Header("Attack Point")]

    public Transform attackPoint;

    [Header("Attack Zone")]
    public Vector2 zoneSize = new Vector2(1f, 1f);
    public float angle = 0f;
    public LayerMask targetLayers;

    [Header("components_Parent")]
    private PhotonView pv;
    private CombatCharacter combatCharacter;

    [Header("system")]
    Collider2D[] overlapBuffer = new Collider2D[10]; // Bộ đệm để lưu trữ các collider phát hiện được
    private HashSet<int> alreadyHitTargets = new HashSet<int>(); // Lưu trữ các mục tiêu đã bị đánh trúng trong lần tấn công hiện tại
    float damageAmount; // Lượng sát thương
    void Awake()
    {
        pv = GetComponentInParent<PhotonView>();
        combatCharacter = GetComponentInParent<CombatCharacter>();
    }
    void Start()
    {
        if(combatCharacter != null)
        {
            damageAmount = combatCharacter.status.GetAttackPower();
        }
    }
    void Update()
    {
        if (isAttacking)
        {
            
            DetectInRange();
            
        }
    }
    public void StartAttack()
    {
        if (!pv.IsMine) return; // Chỉ máy sở hữu mới xử lý
        Debug.Log("Start Attack");
        isAttacking = true;
        alreadyHitTargets.Clear(); // Xóa danh sách mục tiêu đã bị đánh trúng trước đó
    }

    public void StopAttack()
    {
        if (!pv.IsMine) return;
        Debug.Log("Stop Attack");
        isAttacking = false;
        alreadyHitTargets.Clear(); // Xóa danh sách mục tiêu đã bị đánh trúng trước đó
    }

    public void DetectInRange()
    {
        if(pv.IsMine == false) return; // Chỉ xử lý nếu đây là nhân vật của người chơi hiện tại
        overlapBuffer = Physics2D.OverlapBoxAll(attackPoint.position, zoneSize, angle ,targetLayers);
        int count = overlapBuffer.Length;
        for (int i = 0; i < count; i++)
        {
            var target = overlapBuffer[i];
            if (target == null) continue;
            var targetPv = target.GetComponent<PhotonView>();
            int targetId = (targetPv != null) ? targetPv.ViewID : target.gameObject.GetInstanceID(); // Sử dụng ViewID nếu có, nếu không thì dùng Id của InstanceID
            if (alreadyHitTargets.Contains(targetId))
            {
                continue; // Bỏ qua nếu mục tiêu đã bị đánh trúng trong lần tấn công này
            }
            if (targetPv != null && pv != null && targetPv.ViewID == pv.ViewID)
            {
                continue; // Bỏ qua nếu là cùng một người chơi
            }
            if(targetPv != null)
            {
                if (targetPv == null || targetPv.ViewID <= 0)
                {
                    Debug.Log("Target PhotonView không hợp lệ.");
                    continue;
                }
                var known = PhotonNetwork.GetPhotonView(targetPv.ViewID);
                if (known == null)
                {
                    Debug.Log("Target PhotonView không được biết đến trong mạng.");
                    continue;
                }
                Debug.Log("Hit " + target.name);
                alreadyHitTargets.Add(targetId); // Đánh dấu mục tiêu đã bị đánh trúng
                targetPv.RPC("TakeDamage", RpcTarget.All,targetPv.ViewID, damageAmount); // Gọi RPC TakeDamage trên đối tượng bị tấn công
            }
            // else
            // {
            //     // Xử lý đối tượng không có PhotonView nếu cần thiết
            // }
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
