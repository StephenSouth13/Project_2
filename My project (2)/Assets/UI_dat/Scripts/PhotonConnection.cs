using Photon.Pun;
using UnityEngine;

public class PhotonConnection : MonoBehaviourPunCallbacks
{
    public static PhotonConnection instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            CheckInternetAndConnect();

        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
    }
    void CheckInternetAndConnect()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogWarning("❌ [CheckInternetAndConnect] Không có kết nối Internet. Chuyển sang chế độ Offline.");
            StartOfflineMode();
        }
        else
        {
            Debug.Log("🌐 [CheckInternetAndConnect] Kết nối Internet khả dụng. Bắt đầu chế độ Online.");
            StartOnlineMode();
        }
    }
    void StartOnlineMode()
    {
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.ConnectUsingSettings();
    }
    void StartOfflineMode()
    {
        PhotonNetwork.OfflineMode = true;
        Debug.Log("✅ [StartOfflineMode] Chế độ Offline: " + PhotonNetwork.OfflineMode);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("✅ [OnConnectedToMaster] Đã kết nối tới Photon Server!");
        Debug.Log("[OnConnectedToMaster] ID Người chơi: " + PhotonNetwork.LocalPlayer.UserId);
        if (!PhotonNetwork.OfflineMode && !PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError("❌ [OnDisconnected] Mất kết nối: " + cause);
        Debug.Log("[OnDisconnected] Đang thử kết nối lại...");
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("✅ [OnJoinedLobby] Đã vào Lobby!");
    }

}
