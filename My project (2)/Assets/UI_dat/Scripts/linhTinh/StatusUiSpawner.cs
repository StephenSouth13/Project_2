using UnityEngine;
using UnityEngine.UI;
public class StatusUiSpawner : MonoBehaviour
{
    public bool IsDame;
    public bool IsHp;
    public bool IsSpeed;
    public bool IsAttackSpeed;
    public GameObject characterPrefab; // sẽ làm 1 sript tổng để instance nó .
    public Transform content;
    public GameObject Icon_prefab;
    public Sprite On;
    public Sprite Off;

    int maxSpawn = 10;
    int valueOn;
    int valueOff;
    void Start()
    {
        Init();
    }
    void Init() // chỉnh thêm là gọi 1 lần bằng button get character của button đó và bùm
    {
        CombatCharacter baseStatus = characterPrefab.GetComponent<CombatCharacter>();
        if (IsDame)
        {
            valueOn = baseStatus.status.attackDamage;
        }
        if (IsHp)
        {
            valueOn = baseStatus.status.strength;
        }
        if (IsAttackSpeed)
        {
            valueOn = baseStatus.status.dexterity;
        }
        if (IsSpeed)
        {
            valueOn = baseStatus.status.speed;
        }
        valueOff = maxSpawn - valueOn;

        if (valueOn > 0)
        {
            SpawnIcon(Icon_prefab, On, valueOn);
        }
        if (valueOff > 0)
        {
            SpawnIcon(Icon_prefab, Off, valueOff);
        }
    }
    void SpawnIcon(GameObject prefab, Sprite on_off, int value)
    {
        for (int i = 0; i < value; i++)
        {
            GameObject icon = Instantiate(prefab, content);
            Image getImage = icon.GetComponentInChildren<Image>();
            if(getImage != null)
            {
                getImage.sprite = on_off;
            }
        }
    }

    
}
