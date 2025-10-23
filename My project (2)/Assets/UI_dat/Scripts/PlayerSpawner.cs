using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSpawner : MonoBehaviourPun
{
    public GameObject playerPrefab;
    public Transform[] spawnPoints;

    public void SpawnPLayer()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsConnectedAndReady && playerPrefab != null && spawnPoints.Length > 0)
            {
                int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber % spawnPoints.Length; // Chọn điểm spawn dựa trên ActorNumber
                Debug.Log("[SpawnPLayer] Spawning player at index: " + spawnIndex);

                string prefabName = "Character/" + playerPrefab.name;
                Vector2 spawnPosition = spawnPoints[spawnIndex].position; // Lấy vị trí spawn từ mảng spawnPoints
                GameObject player = PhotonNetwork.Instantiate(prefabName, spawnPosition, Quaternion.identity);
                // spawn player tại vị trí đã chọn

                player.name = "Player_" + PhotonNetwork.LocalPlayer.ActorNumber; // Đặt tên cho player dựa trên ActorNumber
                Debug.Log("[SpawnPLayer] Player spawned with name: " + player.name);

                EventSystem.current.SetSelectedGameObject(null); // Bỏ chọn button sau khi nhấn

            }
        }
        else
        {
            Debug.LogWarning("⚠️ [SpawnPLayer] Cannot spawn player because not in a room.");
        }
    }
}
