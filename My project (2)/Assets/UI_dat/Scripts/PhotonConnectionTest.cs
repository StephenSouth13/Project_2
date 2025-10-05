using Photon.Pun;
using UnityEngine;

public class PhotonConnectionTest : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("✅ Đã kết nối tới Photon Server!");
        Debug.Log("ID Người chơi: " + PhotonNetwork.LocalPlayer.UserId);
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError("❌ Mất kết nối: " + cause);
        Debug.Log("Đang thử kết nối lại...");
    }
}
