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
    void Awake()
    {
        status.Init(); // Khởi tạo trạng thái nhân vật
        animCharacter = GetComponent<AnimCharacter>();
        OnHealthChanged?.Invoke(status.currentHealth, status.GetMaxHealth()); // Khởi tạo thanh máu

    }
    void Start()
    {
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
        if (photonView.IsMine == false) return; // Chỉ xử lý nếu đây là nhân vật của người chơi hiện tại
        if (Input.GetKeyDown(KeyCode.J))
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
        Debug.Log("Character took " + damage + " damage.");
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

        photonView.RPC("PlayTriggerDead", RpcTarget.Others); // Đồng bộ hoạt ảnh chết cho các client khác
        animCharacter.PlayTriggerDead();
        StartCoroutine(DelayedDestroy(1f)); // Chờ 1 giây trước khi hủy đối tượng

    }
    IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
    [PunRPC]
    public void SyncHealth(float currentHealth ) // Đồng bộ máu giữa các client
    {
        status.SetCurrentHealth(currentHealth);
        OnHealthChanged?.Invoke(status.currentHealth, status.GetMaxHealth());
    }
}
