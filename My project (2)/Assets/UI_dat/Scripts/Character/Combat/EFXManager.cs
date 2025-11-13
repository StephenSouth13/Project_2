using UnityEngine;
using Photon.Pun;
using System.Collections;

public class EFXManager : MonoBehaviourPun
{
    private Animator anim;
    PhotonPlayerMovement movement;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        movement = GetComponent<PhotonPlayerMovement>();
    }
    [PunRPC]
    public void playTrigerAnim(string animKey)
    {
        if (anim != null && !string.IsNullOrEmpty(animKey))
        {
            anim.SetTrigger(animKey);
        }
    }
    [PunRPC]
    public void SpawnEFX(string objName, string key, Vector3 pos)
    {
        movement.blockGetHorizontal = true;
        GameObject ObjSpawn = Instantiate(Resources.Load<GameObject>(objName), pos, Quaternion.identity);
        RectTransform parent = GetComponentInParent<RectTransform>();
        Debug.Log("x = " + parent.localScale.x);
        if (parent.localScale.x > 0)
        {
            ObjSpawn.transform.localScale = new Vector3(3f, 3f, 1f);
        }
        else
        {
            ObjSpawn.transform.localScale = new Vector3(-3f, 3f, 1f);
        }
        EFXManager eFXManager = ObjSpawn.GetComponent<EFXManager>();
        eFXManager.playTrigerAnim(key);
        photonView.RPC("playTrigerAnim", RpcTarget.Others, key);
        Destroy(ObjSpawn, 0.45f);
    }
    IEnumerator DestroyAfterDelay(GameObject obj, float delay) // có thể dùng nếu là efx gây dame
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(obj);
    }
    
}
