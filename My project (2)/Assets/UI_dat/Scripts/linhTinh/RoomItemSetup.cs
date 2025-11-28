using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemSetup : MonoBehaviourPunCallbacks
{
    public string roomName;
    public TextMeshProUGUI roomName_Txt;
    public TextMeshProUGUI playerCount_Txt;
    public Image IconLock_Img;
    public Image IconUnlock_Img;

    public Button joinRoom_Btn;

    RoomInfo roomInfo;
    bool isJoining = false;
    public void SetupRoomItem(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;
        roomName = roomInfo.Name;
        roomName_Txt.text = roomInfo.Name;
        playerCount_Txt.text = roomInfo.PlayerCount + " / " + roomInfo.MaxPlayers;
        if (roomInfo.IsOpen)
        {
            IconLock_Img.gameObject.SetActive(false);
            IconUnlock_Img.gameObject.SetActive(true);
        }
        else
        {
            IconLock_Img.gameObject.SetActive(true);
            IconUnlock_Img.gameObject.SetActive(false);
            playerCount_Txt.text = "Full";
            this.playerCount_Txt.color = Color.red;

        }
        joinRoom_Btn.onClick.RemoveAllListeners();
        joinRoom_Btn.onClick.AddListener(JoinRoom);
    }
    public void JoinRoom()
    {
        if(isJoining) return; // tránh double click
        if(this.playerCount_Txt.text == "Full" || this.playerCount_Txt.text =="0/0")
        {
            return;
        }

        isJoining = true; 
        string selectedRoomName = this.roomName;

        if (selectedRoomName != null)
        {
            VideoSceneController.instance.SetEnabledRawImage(true);
            VideoSceneController.instance.SendRoomName(selectedRoomName);
            VideoSceneController.instance.SendBoolCreateJoin(false, true);
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(roomInfo == null) return;
        foreach (RoomInfo room in roomList)
        {
            if (room.Name == roomName)
            {
                SetupRoomItem(room);
                break;
            }
        }
    }
}
