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
        public float baseHealth = 100f;
        public float baseAttackPower = 10f;
        public float baseDefense = 5f;
        public float baseAttackCooldown = 1f;
        public float baseSpeed = 5f;
        [HideInInspector] public float currentHealth { get; private set; }

    #endregion
    #region Funtion
    public void SetCurrentHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0, GetMaxHealth());
    }
    public void Init()
    {
        currentHealth = GetMaxHealth();
    }
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, GetMaxHealth());
    }
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
    public void TakeDamage(float rawDamage)
    {
        float def = GetDefense();
        float actualDamage = Mathf.Max(rawDamage - def, 0.1f);
        currentHealth = Mathf.Clamp(currentHealth - actualDamage, 0f, GetMaxHealth());
    }
    #endregion
    #region GetValue
        public float GetAttackPower()
        {
            return baseAttackPower * (1f + (attackDamage - 5) * 0.1f);
        }
        // Strength -> Hp(60%) - defense (40%)
        float StrengthFactor()
        {
            return (strength - 5) * 0.12f; // 1 point = 12%baseLine
        }
        public float GetMaxHealth()
        {
            float strF = StrengthFactor();
            return baseHealth * (1f + strF * 0.6f);
        }
        public float GetDefense()
        {
            float strF = StrengthFactor();
            return baseDefense * (1f * strF * 0.4f);
        }
        public float GetDexterity()
        {
            return 1f + (dexterity - 5) * 0.03f; // 1 point += 3%  
        }
        public float GetMoveSpeed()
        {
            return baseSpeed * (1f * (speed - 5) * 0.1f);
        }
    #endregion
}
