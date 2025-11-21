using System.Collections;
using Photon.Pun;
using UnityEngine;

public class CombatCharacter : MonoBehaviourPun
{
    public CharacterStatus status = new CharacterStatus();
    public event System.Action<float, float> OnHealthChanged; // Sự kiện khi máu thay đổi

    int comboStep = 0;
    float lastClickTime = 0f;
    float comboResetTime = 1.2f;
    [Header("Components")]
    private AnimCharacter animCharacter;
    [Header("Combat Settings")]
    public float attackSpeed = 1.0f; // số lần đánh mỗi giây 1:0 là mặt định - sẽ lấy atack speed từ CharacterStatus
    private float attackCooldown = 0f; // thời gian chờ giữa các lần đánh

    void Awake()
    {
        status.Init(); // Khởi tạo trạng thái nhân vật
        animCharacter = GetComponent<AnimCharacter>();
        OnHealthChanged?.Invoke(status.currentHealth, status.GetMaxHealth()); // Khởi tạo thanh máu

    }
    void Start()
    {
        attackSpeed = status.GetDexterity();
    }
    void Update()
    {
        ControllPlayer();

    }
    void ControllPlayer()
    {
        
        if (status.IsDead())
        {
            return; // Nếu đã chết thì không làm gì cả
        }
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
        if (photonView.IsMine == false) return; // Chỉ xử lý nếu đây là nhân vật của người chơi hiện tại
        if (Input.GetKeyDown(KeyCode.J) && attackCooldown <= 0f)
        {
            float timeSceneLastClick = Time.time - lastClickTime;
            if (timeSceneLastClick > comboResetTime)
            {
                comboStep = 0;
            }
            lastClickTime = Time.time;

            if (comboStep == 0)
            {
                animCharacter.PlayAttack(comboStep);
                photonView.RPC("PlayAttack", RpcTarget.Others, comboStep);
                comboStep = 1;
            }
            else if (comboStep == 1)
            {
                animCharacter.PlayAttack(comboStep);
                photonView.RPC("PlayAttack", RpcTarget.Others, comboStep);
                comboStep = 2;
            }
            else if (comboStep == 2)
            {
                animCharacter.PlayAttack(comboStep);
                photonView.RPC("PlayAttack", RpcTarget.Others, comboStep);
                comboStep = 0;
            }
            attackCooldown = status.GetAttackCooldown();   
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            


            animCharacter.PlayTriggerDash();
            photonView.RPC("PlayTriggerDash", RpcTarget.Others);
        }
    }
    [PunRPC]
    public void TakeDamage(int viewId,float damage)
    {
        if (photonView.ViewID != viewId)
        {
            Debug.Log("ViewID không khớp, không nhận sát thương");
            return;
        } 
        
        photonView.RPC("PlayTrigerDamaged", RpcTarget.Others); // Đồng bộ hoạt ảnh bị thương cho các client khác
        animCharacter.PlayTrigerDamaged();
        status.TakeDamage(damage);
        photonView.RPC("SyncHealth", RpcTarget.Others, status.currentHealth);
        OnHealthChanged?.Invoke(status.currentHealth, status.GetMaxHealth());
        if (status.IsDead())
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log("Character has died.");
        Time.timeScale = 0.2f; // làm chậm thời gian khi chết
        CameraZoom.instance.ShowKOPanel(true); // Hiển thị bảng KO
        photonView.RPC("PlayTriggerDead", RpcTarget.Others); // Đồng bộ hoạt ảnh chết cho các client khác
        animCharacter.PlayTriggerDead();

        StartCoroutine(DieSequence());
    }
    IEnumerator DieSequence()
    {
        // chờ 2 giây realtime trước khi destroy
        yield return new WaitForSecondsRealtime(2f);

        if (photonView.IsMine)
        {   
            PhotonNetwork.Destroy(gameObject);
            GameEndManager.instance.photonView.RPC("SetGlobalTimeScale", RpcTarget.All, 0f);
        }
    }
    [PunRPC]
    public void SyncHealth(float currentHealth ) // Đồng bộ máu giữa các client
    {
        status.SetCurrentHealth(currentHealth);
        OnHealthChanged?.Invoke(status.currentHealth, status.GetMaxHealth());
    }
    
}
