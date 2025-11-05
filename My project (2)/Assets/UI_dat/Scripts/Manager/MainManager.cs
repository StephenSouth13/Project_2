using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class MainManager : MonoBehaviourPunCallbacks
{
    public GameObject RoomPrefab; // Prefab cho phòng
    public Transform ContentRoom; // Nơi chứa các phòng trong giao diện người dùng

    public Button RefreshRoom_Btn;
    public Button Search_Btn;
    public TMP_InputField inputField;
    private void Start()
    {
        RefreshRoom_Btn.onClick.AddListener(RefreshRoomList);
        Search_Btn.onClick.AddListener(SearchRoom);
    }
    public void SpawnRoom(RoomInfo roomInfo)
    {
        GameObject Room = Instantiate(RoomPrefab, ContentRoom);
        RoomItemSetup roomItemSetup = Room.GetComponent<RoomItemSetup>();
        if (roomItemSetup != null)
        {
            roomItemSetup.SetupRoomItem(roomInfo);
        }

    }
    public void CreateRoom()
    {

        PhotonRoomManager.instance.createRoom();
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in ContentRoom)
        {
            Destroy(child.gameObject);
        }
        foreach (RoomInfo room in roomList)
        {
            SpawnRoom(room);
        }
    }
    public void RefreshRoomList()
    {
        Debug.Log("[RefreshRoomList] Đang làm mới danh sách phòng...");
        foreach (Transform child in ContentRoom)
        {
            Destroy(child.gameObject);
        }
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinLobby();
    }
    public void SearchRoom()
    {
        string text = inputField.text;
        Debug.Log(text);
    }
}
