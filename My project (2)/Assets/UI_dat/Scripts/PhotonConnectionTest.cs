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
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError("❌ Mất kết nối: " + cause);
    }
}
