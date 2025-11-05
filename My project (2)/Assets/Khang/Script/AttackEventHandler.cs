using UnityEngine;

public class AttackEventHandler : MonoBehaviour
{
    private PlayerAttack playerAttack;

    void Start()
    {
        // Tìm script PlayerAttack trên đối tượng cha (Player 1)
        playerAttack = GetComponentInParent<PlayerAttack>();
        if (playerAttack == null)
        {
            Debug.LogError("PlayerAttack script not found on the parent (Player 1)! Cannot call VFX.");
        }
    }

    // HÀM CÔNG KHAI (PUBLIC) mà Animation Event có thể thấy
    public void CallSpawnVFX()
    {
        if (playerAttack != null)
        {
            // Gọi hàm thật trên Player 1
            playerAttack.SpawnAttackVFX();
        }
    }

    // Bạn có thể thêm các hàm cầu nối khác nếu cần
    public void CallDealDamage()
    {
        if (playerAttack != null)
        {
            playerAttack.DealDamage();
        }
    }

    public void CallDisableHitBox()
    {
        if (playerAttack != null)
        {
            playerAttack.DisableHitBox();
        }
    }

    public void CallSpawnDarkSaber()
    {
        if (playerAttack != null)
        {
            playerAttack.SpawnDarkSaber();
        }
    }    
}