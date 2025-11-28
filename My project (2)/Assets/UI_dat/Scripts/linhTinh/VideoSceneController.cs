using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VideoSceneController : MonoBehaviour
{
    public bool isJoinRoom = false;
    public bool isCreateRoom = false;
    public string roomName; // khi nhấn các nút create/join room sẽ gán tên phòng vào biến này
    public VideoPlayer videoPlayer;
    public string nextSceneName = "MainScene"; // tên scene muốn chuyển
    public static VideoSceneController instance;
    void Awake() => instance = this;
    void Start()
    {
        // Đăng ký sự kiện khi video kết thúc
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // Chuyển scene khi video kết thúc
        SceneManager.LoadScene(nextSceneName);
        if(isJoinRoom)
        {
            PhotonRoomManager.instance.joinSpecificRoom(roomName); // Tham gia phòng sau khi chuyển scene
        }
        if(isCreateRoom)
        {
            PhotonRoomManager.instance.createRoom(); // Tạo phòng sau khi chuyển scene
        }

    }
    public void SetEnabledRawImage(bool isActive)
    {
        RawImage videoImage = videoPlayer.GetComponent<RawImage>();
        if (videoImage != null)
        {
            videoImage.enabled = isActive;
        }
        VideoPlayer vp = videoPlayer.GetComponent<VideoPlayer>();
        if (vp != null)
        {
            vp.Play();
        }
    }
    public void SendRoomName(string roomName)
    {
        this.roomName = roomName;
    }
    public void SendBoolCreateJoin(bool isCreateRoom, bool isJoinRoom)
    {
        this.isCreateRoom = isCreateRoom;
        this.isJoinRoom = isJoinRoom;
    }
}
