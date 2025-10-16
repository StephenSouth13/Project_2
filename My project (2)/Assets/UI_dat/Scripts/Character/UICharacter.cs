using UnityEngine;
using UnityEngine.UI;

public class UICharacter : MonoBehaviour
{
    public Slider healthBar1;
    public Slider healthBar2;
    public Character character1;
    public Character character2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthBar1.value = 1f;
        healthBar2.value = 1f;
    }

    // Update is called once per frame
    void UpdateHealthBar1(float currentHealth, float maxHealth)
    {
        if (healthBar1 != null)
        {
            healthBar1.value = currentHealth / maxHealth;
        }
    }
    void UpdateHealthBar2(float currentHealth, float maxHealth)
    {
        if (healthBar2 != null)
        {
            healthBar2.value = currentHealth / maxHealth;
        }
    }
    public void Setcharacter(Character c1, Character c2)
    {
        if (character1 != null || character2 != null)
        {
            character1.OnHealthChanged -= UpdateHealthBar1; // Hủy đăng ký sự kiện cũ s1
            character2.OnHealthChanged -= UpdateHealthBar2; // Hủy đăng ký sự kiện cũ s2

        }
        character1 = c1;
        character2 = c2;
        if (character1 != null)
        {
            character1.OnHealthChanged += UpdateHealthBar1; // Đăng ký sự kiện mới s1
            UpdateHealthBar1(character1.status.currentHealth, character1.status.health); // Cập nhật thanh máu ngay lập tức
        }
        if (character2 != null)
        {
            character2.OnHealthChanged += UpdateHealthBar2; // Đăng ký sự kiện mới s2
            UpdateHealthBar2(character2.status.currentHealth, character2.status.health); // Cập nhật thanh máu ngay lập tức
        }
    }
}
