using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class CameraZoom : MonoBehaviour
{
    public Slider[] healthBar;
    public List<Transform> player = new List<Transform>
    {
        null,
        null
    }; // Tham chiếu đến Transform của các người chơi

    public float zoomOutMin = 5f; // Khoảng cách tối thiểu để bắt đầu zoom out
    public float zoomOutMax = 15f; // Khoảng cách tối đa để zoom out
    public float zoomSpeed = 2f; // Tốc độ zoom
    public PolygonCollider2D CameraBounds; // Collider để giới hạn camera
    private Camera cam;
    public static CameraZoom instance;  
    public float zoomValueY;
    void Awake()
    {
        
        instance = this;
        cam = Camera.main;

        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TryAssignPlayers();

    }

    // Update is called once per frame
    void LateUpdate()
    {
        zoom();
        bounds();
    }
    void zoom()
    {
        if (player[0] != null && player[1] != null)
        {
            // Tính khoảng cách giữa hai người chơi
            float distance = Vector3.Distance(player[0].position, player[1].position);
            if (Mathf.Abs(distance - cam.orthographicSize) > 1f) // chỉ zoom khi chênh lệch > 1
            {
                // Tính toán kích thước camera dựa trên khoảng cách
                float targetZoom = Mathf.Clamp(distance, zoomOutMin, zoomOutMax);

                // Mượt mà thay đổi kích thước camera
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
            }
            Vector3 midPoint = (player[0].position + player[1].position) / 2f;
            cam.transform.position = new Vector3(midPoint.x, midPoint.y + zoomValueY, cam.transform.position.z);
            
        }
        else
        {
            float targetZoom = zoomOutMin;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
        }
        if(player[0] == null && player[1] != null)
        {
            cam.transform.position = new Vector3(player[1].position.x, player[1].position.y + zoomValueY, cam.transform.position.z);
        }
        else if(player[0] != null && player[1] == null)
        {
            cam.transform.position = new Vector3(player[0].position.x, player[0].position.y + zoomValueY, cam.transform.position.z);
        }
    }
    void bounds()
    {
        if (CameraBounds != null)
        {
            Vector3 camPos = cam.transform.position;
            float camHeight = cam.orthographicSize;
            float camWidth = cam.aspect * camHeight;

            // Lấy các điểm biên của collider
            Vector2 minBounds = CameraBounds.bounds.min;
            Vector2 maxBounds = CameraBounds.bounds.max;

            // Giới hạn vị trí camera
            float clampedX = Mathf.Clamp(camPos.x, minBounds.x + camWidth, maxBounds.x - camWidth);
            float clampedY = Mathf.Clamp(camPos.y, minBounds.y + camHeight, maxBounds.y - camHeight);

            cam.transform.position = new Vector3(clampedX, clampedY, camPos.z);
        }
    }
    public void AssignPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            PhotonView pv = p.GetComponent<PhotonView>();
            if (pv != null)
            {
                if (pv.Owner.ActorNumber == 1)
                {
                    player[0] = p.transform;                    
                }
                else if (pv.Owner.ActorNumber == 2)
                {
                    player[1] = p.transform;                    
                }
            }
        }
        if(player[0] != null && player[1] != null)
        {
            CancelInvoke("AssignPlayers"); // Hủy việc gọi lại nếu đã gán được cả hai người chơi
            Debug.Log("Players assigned to CameraZoom.");
            FindAnyObjectByType<UICharacter>().Setcharacter(player[0].GetComponent<CombatCharacter>(), player[1].GetComponent<CombatCharacter>());
            for(int i = 0 ; i < healthBar.Length; i++) // spawn avt ở thanh máu
            {
                GetAVTCharacter getAVT = healthBar[i].GetComponent<GetAVTCharacter>();
                if(getAVT != null)
                {
                    GameObject gameObject = player[i].gameObject;
                    getAVT.Spawn(gameObject);
                }
            }
        }
    }
    void TryAssignPlayers()
    {
        if (player[0] == null || player[1] == null)
        {
            InvokeRepeating("AssignPlayers", 0f, 1f); // Thử gán lại mỗi giây
        }
    }
}
