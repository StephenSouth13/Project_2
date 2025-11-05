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
        }
        joinRoom_Btn.onClick.RemoveAllListeners();
        joinRoom_Btn.onClick.AddListener(JoinRoom);
    }
    public void JoinRoom()
    {
        string selectedRoomName = this.roomName;
        if (selectedRoomName != null)
        {
            PhotonRoomManager.instance.joinSpecificRoom(selectedRoomName);
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
