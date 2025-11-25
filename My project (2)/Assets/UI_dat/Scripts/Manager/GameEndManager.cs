using UnityEngine;
using Photon.Pun;
public class GameEndManager : MonoBehaviourPunCallbacks
{
    public static GameEndManager instance;

    void Awake() => instance = this;

    [PunRPC]
    public void SetGlobalTimeScale(float scale)
    {
        foreach(var p in CameraZoom.instance.player)
        {
            if (p != null)
            {
                var movement = p.GetComponent<PhotonPlayerMovement>();
                if (movement != null)
                {
                    movement.enabled = scale > 0f; // Chỉ cho phép di chuyển khi timeScale > 0
                }
                var combat = p.GetComponent<CombatCharacter>();
                if (combat != null)
                {
                    combat.enabled = scale > 0f; // Chỉ cho phép tấn công khi timeScale > 0
                }
            }
        }
        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
    public void Spawn1Player()
    {
        if (PhotonNetwork.InRoom)
        {
            int spawnIndex = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("spawnIndex") ?
                             (int)PhotonNetwork.LocalPlayer.CustomProperties["spawnIndex"] : 0;
            Debug.Log("[Spawn1Player] Respawning player at index: " + spawnIndex);
            PlayerSpawner.instance.SpawnPLayer(spawnIndex);
            photonView.RPC("ResetInit", RpcTarget.All, spawnIndex);
        }
    }
    [PunRPC]
    public void ResetInit(int spawnIndex)
    {
        CameraZoom.instance.AssignPlayers(); // Gán lại người chơi cho CameraZoom
        CameraZoom.instance.ShowKOPanel(false); // Ẩn bảng KO
    }
    public void GetCustomerProperties()
    {
        if (PhotonNetwork.InRoom)
        {
            int spawnIndex = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("spawnIndex") ?
                             (int)PhotonNetwork.LocalPlayer.CustomProperties["spawnIndex"] : 0;
            int liveCount = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("liveCount") ?
                             (int)PhotonNetwork.LocalPlayer.CustomProperties["liveCount"] : 0;
            Debug.Log("[GetCustomerProperties] Current spawnIndex: " + spawnIndex);
            Debug.Log("[GetCustomerProperties] Current liveCount: " + liveCount);
        }
    }
}

