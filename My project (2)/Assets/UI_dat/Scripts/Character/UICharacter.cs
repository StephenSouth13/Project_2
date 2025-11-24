using UnityEngine;
using UnityEngine.UI;

public class UICharacter : MonoBehaviour
{
    [Header("Health Bars")]
    public Slider healthBar1;
    public Slider healthBar2;
    [Header("Characters")]
    public CombatCharacter character1;
    public CombatCharacter character2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }
    public void Init()
    {
        healthBar1.value = 1f;
        healthBar2.value = 1f;
        healthBar1.gameObject.SetActive(false);
        healthBar1.gameObject.SetActive(false);
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
    public void Setcharacter(CombatCharacter c1, CombatCharacter c2)
    {
        if (character1 != null)
        {
            character1.OnHealthChanged -= UpdateHealthBar1;
        }
        if (character2 != null)
        {
            character2.OnHealthChanged -= UpdateHealthBar2;
        }
        character1 = c1;
        character2 = c2;
        if (character1 != null)
        {
            character1.OnHealthChanged += UpdateHealthBar1; // Đăng ký sự kiện mới s1
            UpdateHealthBar1(character1.status.currentHealth, character1.status.GetMaxHealth()); // Cập nhật thanh máu ngay lập tức
        }
        if (character2 != null)
        {
            character2.OnHealthChanged += UpdateHealthBar2; // Đăng ký sự kiện mới s2
            UpdateHealthBar2(character2.status.currentHealth, character2.status.GetMaxHealth()); // Cập nhật thanh máu ngay lập tức
        }
    }
    public void SetSingleCharacter(CombatCharacter c, int index)
    {
        Debug.Log("[UICharacter] Setting single character at index: " + index);
        // Hủy đăng ký cũ nếu có
        if (index == 0 && character1 != null)
        {
            character1.OnHealthChanged -= UpdateHealthBar1;
        }
        else if (index == 1 && character2 != null)
        {
            character2.OnHealthChanged -= UpdateHealthBar2;
        }

        // Gán nhân vật mới
        if (index == 0)
        {
            character1 = c;
            if (character1 != null)
            {
                character1.OnHealthChanged += UpdateHealthBar1;
                UpdateHealthBar1(character1.status.currentHealth, character1.status.GetMaxHealth());
                healthBar1.gameObject.SetActive(true);
                healthBar1.value = 1f;
            }
        }
        else if (index == 1)
        {
            character2 = c;
            if (character2 != null)
            {
                character2.OnHealthChanged += UpdateHealthBar2;
                UpdateHealthBar2(character2.status.currentHealth, character2.status.GetMaxHealth());
                healthBar2.gameObject.SetActive(true);
                healthBar2.value = 1f;
            }
        }
    }

}
