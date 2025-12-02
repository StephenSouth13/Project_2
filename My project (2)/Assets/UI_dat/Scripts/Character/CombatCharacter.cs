using System.Collections;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Collections.Generic;
using Unity.Mathematics;

public class CombatCharacter : MonoBehaviourPun
{
    public GameObject hitEFX_Prefab;
    public bool isWPressed = false;
    public bool isSPressed = false;
    public bool isADPressed = true;
    public CharacterStatus status = new CharacterStatus();
    public event System.Action<float, float> OnHealthChanged; // Sự kiện khi máu thay đổi

    int comboStep = 0;
    float lastClickTime = 0f;
    float comboResetTime = 1.2f;
    [Header("Components")]
    private AnimCharacter animCharacter;
    private CombatHitbox combatHitbox;
    [Header("Combat Settings")]
    public float attackSpeed = 1.0f; // số lần đánh mỗi giây 1:0 là mặt định - sẽ lấy atack speed từ CharacterStatus
    private float attackCooldown = 0f; // thời gian chờ giữa các lần đánh
    public bool isUsingSkill = false;

    void Awake()
    {
        status.Init(); // Khởi tạo trạng thái nhân vật
        animCharacter = GetComponent<AnimCharacter>();
        combatHitbox = GetComponentInChildren<CombatHitbox>();
        OnHealthChanged?.Invoke(status.currentHealth, status.GetMaxHealth()); // Khởi tạo thanh máu

    }
    void Start()
    {
        attackSpeed = status.GetDexterity();
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
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
        if(combatHitbox == null)
        {
            Debug.Log("chưa gắn combathitbox ở childrend");
            return;
        }
        if (photonView.IsMine == false) return; // Chỉ xử lý nếu đây là nhân vật của người chơi hiện tại
        if (Input.GetKeyDown(KeyCode.J) && attackCooldown <= 0f)
        {
            float timeSceneLastClick = Time.time - lastClickTime;
            if (timeSceneLastClick > comboResetTime)
            {
                comboStep = 0;
            }
            lastClickTime = Time.time;

            if (comboStep == 0)
            {
                setDame(0);
                animCharacter.PlayAttack(comboStep);
                photonView.RPC("PlayAttack", RpcTarget.Others, comboStep);
                comboStep = 1;
            }
            else if (comboStep == 1)
            {
                setDame(0);
                animCharacter.PlayAttack(comboStep);
                photonView.RPC("PlayAttack", RpcTarget.Others, comboStep);
                comboStep = 2;
            }
            else if (comboStep == 2)
            {
                setDame(10f);
                animCharacter.PlayAttack(comboStep);
                photonView.RPC("PlayAttack", RpcTarget.Others, comboStep);
                comboStep = 0;
            }
            attackCooldown = status.GetAttackCooldown();   
        }
        UseSkill();
        // if (Input.GetKeyDown(KeyCode.L))
        // {
            


        //     animCharacter.PlayTriggerDash();
        //     photonView.RPC("PlayTriggerDash", RpcTarget.Others);
        // }
    }
    public void UseSkill()
    {
        CheckPressedToUseSkill();
        if(isUsingSkill) return;
        if(!Input.GetKeyDown(KeyCode.K)) return;

        if(isWPressed)
        {
            setDame(20f);
            
            isUsingSkill = true;
            animCharacter.PlaySkill(0);
            if(!PhotonNetwork.InRoom) return;
            photonView.RPC("PlaySkill", RpcTarget.Others, 0);
            
        }
        else if(isSPressed)
        {
            setDame(15f);
            
            isUsingSkill = true;
            animCharacter.PlaySkill(1);

            if(!PhotonNetwork.InRoom) return;
            photonView.RPC("PlaySkill", RpcTarget.Others, 1);
            
        }
        else if (isADPressed)
        {
            setDame(20f);
            isUsingSkill = true;
            animCharacter.PlaySkill(2);

            if(!PhotonNetwork.InRoom) return;
            photonView.RPC("PlaySkill", RpcTarget.Others, 2);
        }
        
    }
    public void CheckPressedToUseSkill()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            isWPressed = true;
            isSPressed = false;
            isADPressed = false;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            isWPressed = false;
            isSPressed = true;
            isADPressed = false;
        }
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            isWPressed = false;
            isSPressed = false;
            isADPressed = true;
        }
    }
    public void ResetCheckSkill() // trả về kiểu AD
    {
        isWPressed = false;
        isSPressed = false;
        isADPressed = true;
    }
    public void setDame(float dameSet)
    {
        float dameBase = status.GetAttackPower();
        combatHitbox.damageAmount = dameBase + dameSet;

    }
    [PunRPC]
    public void SpawnHitEFX(Vector2 hitPos)
    {
        if(hitEFX_Prefab != null)
        {
            GameObject hit = Instantiate(hitEFX_Prefab, hitPos, quaternion.identity);
            Destroy(hit, 0.3f);
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
        photonView.RPC("playHitSound" , RpcTarget.All);
        photonView.RPC("PlayTrigerDamaged", RpcTarget.Others); // Đồng bộ hoạt ảnh bị thương cho các client khác
        animCharacter.PlayTrigerDamaged();
        status.TakeDamage(damage);
        photonView.RPC("SyncHealth", RpcTarget.Others, status.currentHealth);
        OnHealthChanged?.Invoke(status.currentHealth, status.GetMaxHealth());
        if (status.IsDead())
        {
            Die();
        }
    }
    [PunRPC]
    public void playHitSound()
    {
        AudioManager.instance.PlayIndexSoundEFXHit(0);
    }
    void Die()
    {
        GameEndManager.instance.photonView.RPC("SetBoolKOTime", RpcTarget.All, true);
        
        Debug.Log("Character has died.");
        Time.timeScale = 0.2f; // làm chậm thời gian khi chết
        CameraZoom.instance.ShowKOPanel(true); // Hiển thị bảng KO
        photonView.RPC("PlayTriggerDead", RpcTarget.Others); // Đồng bộ hoạt ảnh chết cho các client khác
        animCharacter.PlayTriggerDead();
        SetLiveCount();
        StartCoroutine(DieSequence());

    }
    IEnumerator DieSequence()
    {
        // chờ 4 giây realtime trước khi destroy
        yield return new WaitForSecondsRealtime(4.5f);

        if (photonView.IsMine)
        {   
            PhotonNetwork.Destroy(gameObject);
            
            GameEndManager.instance.photonView.RPC("SetGlobalTimeScale", RpcTarget.All, 1f);
            GameEndManager.instance.StartCoroutine("DelaySpawn1Player", 0.5f);
        }
        
    }
    public void SetLiveCount()
    {
        if(PhotonNetwork.InRoom && photonView.IsMine)
        {
            Player owner = photonView.Owner;    
            if(owner != null && owner.CustomProperties.ContainsKey("liveCount"))
            {
                int liveCount = (int)owner.CustomProperties["liveCount"];
                int liveCountNew = Mathf.Max(liveCount - 1, 0);
                ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
                customProperties["liveCount"] = liveCountNew;
                PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
                Debug.Log("[SetLiveCount] Cập nhật liveCount mới: " + liveCountNew);
            }
        }
    }    

    [PunRPC]
    public void SyncHealth(float currentHealth ) // Đồng bộ máu giữa các client
    {
        status.SetCurrentHealth(currentHealth);
        OnHealthChanged?.Invoke(status.currentHealth, status.GetMaxHealth());
    }
    
}
