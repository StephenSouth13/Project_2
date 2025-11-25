using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;  
public class PhotonRoomManager : MonoBehaviourPunCallbacks
{
    public static PhotonRoomManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [SerializeField] public List<RoomInfo> availableRooms = new List<RoomInfo>(); // Danh sách phòng hiện có
    public void createRoom() // Tạo phòng mới với tên duy nhất
    {
        int indexRoom = 1; // Bắt đầu từ Room_1
        string roomName = "Room_" + indexRoom; // Tên phòng mặc định
        while (availableRooms.Exists(r => r.Name == roomName))
        {
            indexRoom++;
            roomName = "Room_" + indexRoom;
        }
        RoomOptions roomOptions = new RoomOptions // Tùy chọn phòng
        {
            MaxPlayers = 2,
            IsOpen = true
        };
        PhotonNetwork.CreateRoom(roomName, roomOptions); // Tạo phòng với tên và tùy chọn đã định nghĩa
        SceneManager.LoadSceneAsync("Battle_Fight");
        
    }
    public void joinRoom() // Hàm sẽ được sử dụng cho button "Join Room" // Tham gia phòng ngẫu nhiên
    {
        if (availableRooms.Count == 0)
        {
            Debug.LogWarning("⚠️ [joinRoom] Không có phòng nào để tham gia. Tạo phòng mới.");
            createRoom();
            return;
        }

        List<RoomInfo> shuffledRooms = new List<RoomInfo>(availableRooms);
        ShuffleRoomList(shuffledRooms); // Xáo trộn danh sách phòng

        foreach (RoomInfo room in shuffledRooms)
        {
            if (room.PlayerCount < room.MaxPlayers)
            {
                SceneManager.LoadSceneAsync("Battle_Fight"); // Load scene Battle_Fight khi tham gia phòng
                PhotonNetwork.JoinRoom(room.Name);

                return;
            }
        }
        Debug.LogWarning("⚠️ [joinRoom] Không có phòng trống. Tạo phòng mới.");
        createRoom();
        EventSystem.current.SetSelectedGameObject(null); // Bỏ chọn button sau khi nhấn

    }
    public void joinSpecificRoom(string roomName) // Tham gia phòng cụ thể theo tên
    {
        RoomInfo targetRoom = availableRooms.Find(r => r.Name == roomName);
        if (targetRoom != null && targetRoom.PlayerCount < targetRoom.MaxPlayers)
        {
            
            SceneManager.LoadSceneAsync("Battle_Fight"); // Load scene Battle_Fight khi tham gia phòng
            PhotonNetwork.JoinRoom(roomName);
            Debug.Log("✅ [joinSpecificRoom] Tham gia phòng: " + roomName);
        }
        else
        {
            Debug.LogWarning("⚠️ [joinSpecificRoom] Phòng không tồn tại hoặc đã đầy: " + roomName);
        }
        EventSystem.current.SetSelectedGameObject(null); // Bỏ chọn button sau khi nhấn
    }
    public void leaveRoom() // Hàm sẽ được sử dụng cho button "Leave Room" // Rời phòng hiện tại
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            Debug.Log("✅ [leaveRoom] Đã rời phòng.");
            SceneManager.LoadSceneAsync("Main_game"); // Quay lại scene Lobby khi rời phòng

        }
        else
        {
            Debug.LogWarning("⚠️ [leaveRoom] Không thể rời phòng vì không ở trong phòng nào.");
        }
        EventSystem.current.SetSelectedGameObject(null); // Bỏ chọn button sau khi nhấn
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        availableRooms.Clear();
        foreach (RoomInfo room in roomList)
        {
            availableRooms.Add(room);
            Debug.Log("[OnRoomlistUpdate] Room Name: " + room.Name + ", Players: " + room.PlayerCount + "/" + room.MaxPlayers);
        }
    }
    public override void OnLeftRoom()
    {
        Debug.Log("✅ [OnLeftRoom] Đã rời phòng thành công.");
        if (PhotonNetwork.InLobby)
        {
            Debug.Log("✅ [OnLeftRoom] Vẫn ở trong Lobby.");
        }
        else
        {
            Debug.LogWarning("⚠️ [OnLeftRoom] Không còn ở trong Lobby, đang tham gia lại...");
            PhotonNetwork.JoinLobby(); // Tham gia lại Lobby nếu cần
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer) // ai xử lý ? là master client khi có người rời phòng
    {
        Debug.Log("⚠️ [OnPlayerLeftRoom] Người chơi rời phòng: " + otherPlayer.NickName);
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("✅ [OnPlayerLeftRoom] Bạn là Master Client. Phòng vẫn mở cho người chơi khác.");
            if (PhotonNetwork.CurrentRoom.PlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                PhotonNetwork.CurrentRoom.IsOpen = true; // Mở lại phòng nếu có người rời
            }
            // Thêm logic xử lý Quay lại scene chờ. thoát khởi scene game.

            
        }
        else
        {
            Debug.Log("⏳ [OnPlayerLeftRoom] Chờ Master Client xử lý...");
        }
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("✅ [OnJoinedRoom] Đã vào phòng: " + PhotonNetwork.CurrentRoom.Name);

        int index = GetFreeSpawnIndex();
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["spawnIndex"] = index;
        customProperties["liveCount"] = 2;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

        if(PlayerSpawner.instance != null)
        {
            Debug.Log("✅ [OnJoinedRoom] Gọi SpawnPLayer từ PlayerSpawner.");
            PlayerSpawner.instance.SpawnPLayer(index);
        }
        else
        {
            Debug.LogError("❌ [OnJoinedRoom] PlayerSpawner.instance là null. Không thể gọi SpawnPLayer.");
        }

        Debug.Log("✅ [OnJoinedRoom] Gán spawnIndex = " + index + " cho LocalPlayer");
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
           
            Debug.Log("✅ [OnJoinedRoom] Phòng đầy. Bắt đầu trò chơi!");
            // logic Bắt đầu trò chơi
            PhotonNetwork.CurrentRoom.IsOpen = false; // Đóng phòng để không ai khác có thể tham gia
        }
        else
        {
            Debug.Log("⏳ [OnJoinedRoom] Đang chờ người chơi khác...");
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("❌ [OnJoinRoomFailed] Không thể vào phòng: " + message);
        joinRoom(); // Thử tham gia phòng khác
    }
    public override void OnCreatedRoom()
    {
       

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("❌ [OnCreateRoomFailed] Không thể tạo phòng: " + message);
    }
    void ShuffleRoomList(List<RoomInfo> list) // Hàm xáo trộn danh sách phòng
    {
        for (int i = 0; i < list.Count; i++)
        {
            RoomInfo temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    int GetFreeSpawnIndex() // Hàm tìm chỉ số spawn trống cho người chơi mới
    {
        Debug.Log("[GetFreeSpawnIndex] Tìm chỉ số spawn trống...");
        HashSet<int> usedIndices = new HashSet<int>(); // Tập hợp chỉ số đã được sử dụng
        foreach (Player player in PhotonNetwork.PlayerList) // Duyệt qua tất cả người chơi trong phòng và thu thập chỉ số đã sử dụng
        {
            if (player.CustomProperties.ContainsKey("spawnIndex"))
            {
                Debug.Log("[GetFreeSpawnIndex] Đã sử dụng spawnIndex: " + (int)player.CustomProperties["spawnIndex"]);
                usedIndices.Add((int)player.CustomProperties["spawnIndex"]);
            }
        }
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++) // Tìm chỉ số trống đầu tiên
        {
            if (!usedIndices.Contains(i))
            {
                Debug.Log("[GetFreeSpawnIndex] Chỉ số spawn trống tìm thấy: " + i);
                return i;
            }
        }
        Debug.LogWarning("⚠️ [GetFreeSpawnIndex] Không tìm thấy chỉ số spawn trống.");
        return -1; // Nếu không còn chỉ số trống
    }
}
