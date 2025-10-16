using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Character : MonoBehaviourPun
{
    public CharacterStatus status = new CharacterStatus(); // Thành phần trạng thái nhân vật

    public event System.Action<float, float> OnHealthChanged; // Sự kiện khi máu thay đổi

    [Header("Components")]
    public AnimCharacter animCharacter;
    void Awake()
    {
        animCharacter = GetComponentInChildren<AnimCharacter>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        status.Init(); 
        OnHealthChanged?.Invoke(status.currentHealth, status.health); // Khởi tạo thanh máu
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && photonView.IsMine)
        {
            TakeDamage(20f);
            Debug.Log("Character took damage. Current Health: " + status.currentHealth);
        }
        if(Input.GetKeyDown(KeyCode.J) && photonView.IsMine)
        {
            Attack();
        }
    }
    public void Attack() // cần thêm logic tấn công
    {
        animCharacter.PlayTriggerAttack(); // Phát hoạt ảnh tấn công
        photonView.RPC("PlayTriggerAttack", RpcTarget.Others); // Đồng bộ hoạt ảnh tấn công cho các client khác
    }
    public void TakeDamage(float damage)
    {
        if (photonView.IsMine == false) return; // Chỉ xử lý nếu đây là nhân vật của người chơi hiện tại
        animCharacter.PlayTrigerDamaged(); // Phát hoạt ảnh bị thương
        photonView.RPC("PlayTrigerDamaged", RpcTarget.Others); // Đồng bộ hoạt ảnh bị thương cho các client khác
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
        // Thêm logic chết ở đây (ví dụ: phát hiệu ứng, tắt nhân vật, v.v.)
        animCharacter.PlayTriggerDead(); // Phát hoạt ảnh chết
        photonView.RPC("PlayTriggerDead", RpcTarget.Others); // Đồng bộ hoạt ảnh chết cho các client khác
        StartCoroutine(DeslayDestroy(0.5f)); // Đợi 0.5 giây rồi destroy
    }
    IEnumerator DeslayDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Photon.Pun.PhotonNetwork.Destroy(gameObject);
    }
    [PunRPC]
    void SyncHealth(float currentHealthSync, float maxHealth)
    {
        status.SetCurrentHealth(currentHealthSync);
        status.health = maxHealth;
        OnHealthChanged?.Invoke(status.currentHealth, status.health);
    }
}
