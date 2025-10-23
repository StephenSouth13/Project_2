using System.Collections;
using Photon.Pun;
using UnityEngine;

public class CombatCharacter : MonoBehaviourPun
{
    public CharacterStatus status = new CharacterStatus();
    public event System.Action<float, float> OnHealthChanged; // Sự kiện khi máu thay đổi
    [Header("Components")]
    private AnimCharacter animCharacter;
    void Awake()
    {
        status.Init(); // Khởi tạo trạng thái nhân vật
        animCharacter = GetComponent<AnimCharacter>();
    }
    void Start()
    {
        OnHealthChanged?.Invoke(status.currentHealth, status.health); // Khởi tạo thanh máu
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

            animCharacter.PlayTriggerAttack();
            photonView.RPC("PlayTriggerAttack", RpcTarget.Others);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            animCharacter.PlayTriggerAttack2();
            photonView.RPC("PlayTriggerAttack2", RpcTarget.Others);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            animCharacter.PlayTriggerAttack3();
            photonView.RPC("PlayTriggerAttack3", RpcTarget.Others);
        }
        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            animCharacter.PlayTriggerAttack4();
            photonView.RPC("PlayTriggerAttack4", RpcTarget.Others);
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
        photonView.RPC("SyncHealth", RpcTarget.Others, status.currentHealth, status.health);
        OnHealthChanged?.Invoke(status.currentHealth, status.health);
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
        PhotonNetwork.Destroy(gameObject);
    }
    [PunRPC]
    public void SyncHealth(float currentHealth,float health) // Đồng bộ máu giữa các client
    {
        status.SetCurrentHealth(currentHealth);
        status.health = health;
        OnHealthChanged?.Invoke(status.currentHealth, status.health);
    }
}
