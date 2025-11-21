using UnityEngine;

public class PlayerModel 
{
    public float health { get; private set; }
    public float maxHealth { get; private set; }
    
    public bool IsDead => health <= 0;
    
    public PlayerModel(float maxHealth)
    {
        this.maxHealth = maxHealth;
        this.health = maxHealth;
    }
    
    public void TakeDamage(float damage)
    {
        if(damage < 0) return;
        health -= damage;
        if (health < 0) health = 0;
    }
    
    public void Heal(float amount)
    {
        if(amount < 0) return;
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }
    
    public Vector3 CalculateMovement(float horizontalInput, float verticalInput, 
        float speed, float deltaTime)
    {
        return new Vector3(horizontalInput, verticalInput, 0f) * speed * deltaTime;
    }
    public float Calculate_Skill_Damage(int character_level,float weapon_damage) 
    {
        
        float base_damage = character_level * 10;
        float final_damage = base_damage + weapon_damage;
        if (Is_Critical_Hit()) {
        final_damage = final_damage * 2;
        }
        return final_damage;
        
    }
    public bool Is_Critical_Hit()
    {
        int n = Random.Range(1, 10);
        if(n > 5) return true;
        else return false;
    }

}
