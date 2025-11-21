using Unity.Mathematics;
using UnityEngine;
[System.Serializable]
public class CharacterStatus
{
    #region reference
        [Header("Attributes(1-10)")]
        [Range(1, 10)] public int attackDamage = 5;
        [Range(1, 10)] public int strength = 5;
        [Range(1, 10)] public int dexterity = 5;
        [Range(1, 10)] public int speed = 5;

        [Header("Base Stats")]
        float baseHealth = 150f;
        float baseAttackPower = 20f;
        float baseDefense = 5f;
        float baseAttackSpeed = 0.8f;
        float baseSpeed = 7.5f;
        [HideInInspector] public float currentHealth { get; private set; }

    #endregion
    #region Funtion
    public void SetCurrentHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0, GetMaxHealth());
    }
    public void Init() // khởi tạo trạng thái nhân vật - hiện tai chỉ khởi tạo máu
    {
        currentHealth = GetMaxHealth();
    }
    public void Heal(float amount) // dùng để hồi máu
    {
        currentHealth = Mathf.Min(currentHealth + amount, GetMaxHealth());
    }
    public bool IsDead() // kiểm tra nhân vật đã chết chưa
    {
        return currentHealth <= 0;
    }
    public void TakeDamage(float rawDamage)
    {
        float def = GetDefense();
        float actualDamage = Mathf.Max(rawDamage - def, 0.1f);
        currentHealth = Mathf.Clamp(currentHealth - actualDamage, 0f, GetMaxHealth());
        Debug.Log("Character took raw damage: " + rawDamage +"|Def: " + def + " | after defense: " + actualDamage + " | current health: " + currentHealth);
    }
    #endregion
    #region GetValue

        // Strength -> Hp(60%) - defense (40%)
    float StrengthFactor()
    {
        return (strength - 1) * 0.3f;
    }

    public float GetMaxHealth()
    {
        float strF = StrengthFactor();
        // 60% ảnh hưởng vào máu
        float hp = baseHealth * (1f + strF * 0.6f);
        return Mathf.Max(hp, 1f); // tránh 0 hoặc âm
    }

    public float GetDefense()
    {
        float strF = StrengthFactor();
        // 40% ảnh hưởng vào phòng thủ
        float def = baseDefense * (1f + strF * 0.4f);
        return Mathf.Max(def, 0f); // tránh âm
    }

    public float GetAttackPower()
    {
        return baseAttackPower * (1f + (attackDamage - 1) * 0.1f);
    }
    public float GetMoveSpeed()
    {
        float move = baseSpeed * (1f + (speed - 1) * 0.05f); // mỗi điểm ±5%
        return move;
    }
    
    public float GetDexterity()
    {
        float dexMul = 1f + (dexterity - 1) * 0.3f; // mỗi điểm ±30%
        float dexOut = baseAttackSpeed * Mathf.Abs(dexMul);
        return dexOut;
    }
    // Cooldown đánh: dex càng cao → cooldown càng ngắn
    public float GetAttackCooldown()
    {
        return Mathf.Max(1 / GetDexterity(), 0.34f);
    }
    #endregion
}
