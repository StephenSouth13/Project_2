using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using WebSocketSharp;

public class PlayerSpawner : MonoBehaviourPun
{
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    string prefabName;
    void Start()
    {
        Invoke("SpawnPLayer",1f);
    }
    public void SpawnPLayer()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsConnectedAndReady && playerPrefab != null && spawnPoints.Length > 0)
            {
                int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber % spawnPoints.Length; // Chọn điểm spawn dựa trên ActorNumber
                Debug.Log("[SpawnPLayer] Spawning player at index: " + spawnIndex);
                string characterName = PlayerPrefs.GetString("Character");
                if (!string.IsNullOrEmpty(characterName)) // Khi đã lưu lựa chọn character
                {
                    string prefabName = "Character/" + characterName;
                     Vector2 spawnPosition = spawnPoints[spawnIndex].position; // Lấy vị trí spawn từ mảng spawnPoints
                    GameObject player = PhotonNetwork.Instantiate(prefabName, spawnPosition, Quaternion.identity);
                    // spawn player tại vị trí đã chọn

                    player.name = "Player_" + PhotonNetwork.LocalPlayer.ActorNumber; // Đặt tên cho player dựa trên ActorNumber
                    Debug.Log("[SpawnPLayer] Player spawned with name: " + player.name);

                    EventSystem.current.SetSelectedGameObject(null); // Bỏ chọn button sau khi nhấn
                
                }
                else // lấy default character
                {
                    string prefabName = "Character/" + playerPrefab.name;
                     Vector2 spawnPosition = spawnPoints[spawnIndex].position; // Lấy vị trí spawn từ mảng spawnPoints
                    GameObject player = PhotonNetwork.Instantiate(prefabName, spawnPosition, Quaternion.identity);
                    // spawn player tại vị trí đã chọn

                    player.name = "Player_" + PhotonNetwork.LocalPlayer.ActorNumber; // Đặt tên cho player dựa trên ActorNumber
                    Debug.Log("[SpawnPLayer] Player spawned with name: " + player.name);

                    EventSystem.current.SetSelectedGameObject(null); // Bỏ chọn button sau khi nhấn
                
                }
                   

            }
        }
        else
        {
            Debug.LogWarning("⚠️ [SpawnPLayer] Cannot spawn player because not in a room.");
        }
    }
}
