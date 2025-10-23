using Unity.Mathematics;
using UnityEngine;
[System.Serializable]
public class CharacterStatus
{
    [Header("Base Stats")]
    public float health = 100f;
    public float attackPower = 10f;
    public float defense = 5f;
    public float speed = 5f;
    [HideInInspector] public float currentHealth { get; private set; }
    public void SetCurrentHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0, health);
    }
    public void Init()
    {
        currentHealth = health;
    }
    public void TakeDamage(float damage)
    {
        float actualDamage = Mathf.Max(damage - defense, 0);
        currentHealth -= actualDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, health);
    }
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, health);
    }
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
    
}
