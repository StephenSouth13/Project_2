using UnityEngine;
using Photon.Pun;
public class PlayerVFXController : MonoBehaviourPun
{
    public GameObject prefabEFX;
    public Transform dashPos;
    public PhotonView pv;
    PhotonPlayerMovement movement;
    EFXManager eFXManager;
    Rigidbody2D rb;
    void Awake()
    {
        pv = GetComponentInParent<PhotonView>();
        movement = GetComponentInParent<PhotonPlayerMovement>();
        eFXManager = GetComponentInParent<EFXManager>();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    public void PlayDash() 
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                string prefabName = "EFX/" + prefabEFX.name;
                eFXManager.SpawnEFX(prefabName, "Dash", dashPos.position);
                Debug.Log("Spawn OK");
                
            }
        }
    }
    public void setBlockMovement() // khóa di chuyển // thêm event ở đầu anim
    {
        SetFreeze(true, true, true);
    }
    public void setBack() // trả lại trạng thái ban đầu // thêm event ở cuối anim 
    {

        SetFreeze(false, true, true);
        SetFreeze(false, false, true);
    }
    public void SetFreeze(bool freezeX, bool freezeY, bool freezeRotation)
    {
        RigidbodyConstraints2D constraints = RigidbodyConstraints2D.None;

        if (freezeX) constraints |= RigidbodyConstraints2D.FreezePositionX;
        if (freezeY) constraints |= RigidbodyConstraints2D.FreezePositionY;
        if (freezeRotation) constraints |= RigidbodyConstraints2D.FreezeRotation;

        rb.constraints = constraints;
        rb.gravityScale = freezeY ? 0f : 4f;
        
    }

}
