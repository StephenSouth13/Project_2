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
                SetFreeze(false, true, true); // lock y và x 
                string prefabName = "EFX/" + prefabEFX.name;
                eFXManager.SpawnEFX(prefabName, "Dash", dashPos.position);
                pv.RPC("SpawnEFX", RpcTarget.Others, prefabName, "Dash", dashPos.position);
                Debug.Log("Spawn OK");
                
            }
        }
    }



    public void setPosAfterDash()
    {
        RectTransform parent = GetComponentInParent<RectTransform>();
        parent.position = transform.position;
        SetFreeze(false, false, true);
        movement.blockGetHorizontal = false;
    }
    public void SetFreeze(bool freezeX, bool freezeY, bool freezeRotation)
    {
        RigidbodyConstraints2D constraints = RigidbodyConstraints2D.None;

        if (freezeX) constraints |= RigidbodyConstraints2D.FreezePositionX;
        if (freezeY) constraints |= RigidbodyConstraints2D.FreezePositionY;
        if (freezeRotation) constraints |= RigidbodyConstraints2D.FreezeRotation;

        rb.constraints = constraints;
    }

}
