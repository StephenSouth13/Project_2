using UnityEngine;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;
using UnityEngine.UI;
public class GameEndManager : MonoBehaviourPunCallbacks
{
    public GameObject panelEnd;
    public Button rematchBtn;
    public Button homeBtn;

    public bool IsKoTime = false;
    public static GameEndManager instance;

    void Awake() => instance = this;
    void Start()
    {
        rematchBtn.onClick.AddListener(() =>
        {
            RequestRematch();
        });
        homeBtn.onClick.AddListener(() =>
        {
            BackToHome();
        });
    }
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
        IsKoTime = false;
        Time.timeScale = 1f; // Đặt lại timeScale về 1 phòng khi rematch
        Time.fixedDeltaTime = 0.02f; // Đặt lại fixedDeltaTime về mặc định phòng khi rematch
        CameraZoom.instance.AssignPlayers(); // Gán lại người chơi cho CameraZoom
        CameraZoom.instance.ShowKOPanel(false); // Ẩn bảng KO
    }
    public void GetCustomerProperties() // chỉ để debug lấy thông tin lưu trữ trong custom properties
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
    IEnumerator DelaySpawn1Player(float delay) // chỉ player đã chết mới chạy hàm này
    {
        yield return new WaitForSecondsRealtime(delay);
        int liveCount = GetLiveCount();
        if(liveCount > 0)
        {
            Spawn1Player();
        }
        else
        {
            Debug.Log("[DieSequence] Không còn mạng , xử lý logic end Game.");
            photonView.RPC("SysncDieSystem", RpcTarget.All);
            

        }

    }
    [PunRPC]
    public void SetBoolKOTime(bool t)
    {
        IsKoTime = t; 
        Debug.Log("[SetBoolKOTime] : IsKOTime : " + IsKoTime);
    }
    [PunRPC]
    public void SysncDieSystem()
    {
        PlayerSpawner.instance.DestroyAllAvt();
        panelEnd.SetActive(true);
        RematchManager.instance.SetRoomState(RoomState.PostMatch);
    }
    public int GetLiveCount()
    {
        if (PhotonNetwork.InRoom)
        {
            int liveCount = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("liveCount") ?
                             (int)PhotonNetwork.LocalPlayer.CustomProperties["liveCount"] : 0;
            Debug.Log("[GetLiveCount] Current liveCount: " + liveCount);
            return liveCount;
        }
        return 0;
    }
    public void BackToHome() // Gọi khi nhấn nút về menu chính
    {
        Time.timeScale = 1f; // Đặt lại timeScale về 1 khi về menu chính
        Time.fixedDeltaTime = 0.02f; // Đặt lại fixedDeltaTime về mặc định
        PhotonRoomManager.instance.leaveRoom();
    }
    public void RequestRematch() // Gọi khi nhấn nút rematch
    {
        photonView.RPC("SetGlobalTimeScale", RpcTarget.All, 1f);
        RematchManager.instance.RequestRematch();
    }
}

