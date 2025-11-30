using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using WebSocketSharp;
using UnityEngine.UI;
using Photon.Realtime;
public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public Slider[] healbar_slider;
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    string prefabName;
    public static PlayerSpawner instance;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
    }

    public void SpawnPLayer(int spawnIndex) // gọi từ OnJoinedRoom và GameEndManager
    {
        if (PhotonNetwork.InRoom)
        {
            
            if (PhotonNetwork.IsConnectedAndReady && playerPrefab != null && spawnPoints.Length > 0)
            {
        

                Debug.Log("[SpawnPLayer] Spawning player at index: " + spawnIndex);
                string characterName = PlayerPrefs.GetString("Character");
                if (!string.IsNullOrEmpty(characterName)) // Khi đã lưu lựa chọn character
                {
                    string prefabName = "Character/" + characterName;
                    Vector2 spawnPosition = spawnPoints[spawnIndex].position; // Lấy vị trí spawn từ mảng spawnPoints
                    GameObject player = PhotonNetwork.Instantiate(prefabName, spawnPosition, Quaternion.identity);
                    // spawn player tại vị trí đã chọn
                    Slider slider = healbar_slider[spawnIndex];
                    GetAVTCharacter getAVT = slider.GetComponent<GetAVTCharacter>();
                    if(getAVT != null)
                    {
                        getAVT.Spawn(player);
                    }
                    FindAnyObjectByType<UICharacter>().SetSingleCharacter(player.GetComponent<CombatCharacter>(), spawnIndex);
                    player.name = "Player_" + PhotonNetwork.LocalPlayer.ActorNumber; // Đặt tên cho player dựa trên ActorNumber
                    Debug.Log("[SpawnPLayer] Player spawned with name: " + player.name);

                }
                else // lấy default character
                {
                    string prefabName = "Character/" + playerPrefab.name;
                    Vector2 spawnPosition = spawnPoints[spawnIndex].position; // Lấy vị trí spawn từ mảng spawnPoints
                    GameObject player = PhotonNetwork.Instantiate(prefabName, spawnPosition, Quaternion.identity);
                    // spawn player tại vị trí đã chọn

                    player.name = "Player_" + PhotonNetwork.LocalPlayer.ActorNumber; // Đặt tên cho player dựa trên ActorNumber
                    Debug.Log("[SpawnPLayer] Player spawned with name: " + player.name);

                }
            }
        }
    }
    public void DestroyAllAvt()
    {
        foreach(Slider slider in healbar_slider)
        {
            GetAVTCharacter getAVT = slider.GetComponent<GetAVTCharacter>();
            if(getAVT == null ) continue;
            getAVT.DestroyAvt();
        }
    }
}
