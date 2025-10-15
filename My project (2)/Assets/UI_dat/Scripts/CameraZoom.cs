using Photon.Pun;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Transform player1; // Tham chiếu đến Transform của người chơi
    public Transform player2; // Tham chiếu đến Transform của người chơi thứ hai

    public float zoomOutMin = 5f; // Khoảng cách tối thiểu để bắt đầu zoom out
    public float zoomOutMax = 15f; // Khoảng cách tối đa để zoom out
    public float zoomSpeed = 2f; // Tốc độ zoom
    public PolygonCollider2D CameraBounds; // Collider để giới hạn camera
    private Camera cam;
    public static CameraZoom instance;  
    void Awake()
    {
        
        instance = this;
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
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
        if (player1 != null && player2 != null)
        {
            // Tính khoảng cách giữa hai người chơi
            float distance = Vector3.Distance(player1.position, player2.position);

            // Tính toán kích thước camera dựa trên khoảng cách
            float targetZoom = Mathf.Clamp(distance, zoomOutMin, zoomOutMax);

            // Mượt mà thay đổi kích thước camera
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);

            Vector3 midPoint = (player1.position + player2.position) / 2f;
            cam.transform.position = new Vector3(midPoint.x, midPoint.y, cam.transform.position.z);
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
        foreach (GameObject player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null)
            {
                if (pv.Owner.ActorNumber == 1)
                {
                    player1 = player.transform;
                }
                else if (pv.Owner.ActorNumber == 2)
                {
                    player2 = player.transform;
                }
            }
        }
        if(player1 != null && player2 != null)
        {
            CancelInvoke("AssignPlayers"); // Hủy việc gọi lại nếu đã gán được cả hai người chơi
        }
    }
    void TryAssignPlayers()
    {
        if (player1 == null || player2 == null)
        {
            InvokeRepeating("AssignPlayers", 0f, 1f); // Thử gán lại mỗi giây
        }
    }
}
