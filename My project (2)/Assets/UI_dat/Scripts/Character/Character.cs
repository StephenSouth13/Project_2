using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterStatus status = new CharacterStatus();

    public event System.Action<float,float> OnHealthChanged;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        status.Init();
        OnHealthChanged?.Invoke(status.currentHealth, status.health);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(20f);
            Debug.Log("Character took damage. Current Health: " + status.currentHealth);
        }
    }
    public void TakeDamage(float damage)
    {
        status.TakeDamage(damage);
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
        Photon.Pun.PhotonNetwork.Destroy(gameObject);
    }
}
