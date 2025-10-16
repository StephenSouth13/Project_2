using Photon.Pun;
using UnityEngine;

public class PhotonPlayerMovement : MonoBehaviourPun
{
    // Cài đặt Tốc độ và Lực Nhảy (Đã tối ưu cho cảm giác Battle Game)
    [Header("Movement Settings")]
    public float runSpeed = 7.5f;     // Tốc độ
    public float jumpForce = 7f;     // Lực nhảy
    public int maxJumps = 2;          // Số lần nhảy tối đa (Nhảy Đôi)
    public bool isJump;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    // Thành phần và Trạng thái
    [Header("Components")]
    public Rigidbody2D rb;
    public AnimCharacter animCharacter;
    private bool isGrounded;
    private float horizontalInput;
    public int jumpsRemaining; // Số lần nhảy còn lại

    void Awake()
    {
        // Lấy thành phần Rigidbody2D và Animator
        rb = GetComponent<Rigidbody2D>();
        animCharacter = GetComponentInChildren<AnimCharacter>();
    }
    // Khởi tạo
    void Start()
    {
        jumpsRemaining = maxJumps;
    }

    // Cập nhật Vật lý (FixedUpdate)
    void Update()
    {


        if (PhotonNetwork.IsConnected && photonView.IsMine)
        {
            // 1. KIỂM TRA CHẠM ĐẤT
            CheckGrounded();

            // 2. DI CHUYỂN NGANG
            MoveHorizontal();

            // 3. NHẢY
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
        }
    }


    // =========================================================
    // HÀM XỬ LÝ CHUYỂN ĐỘNG
    // =========================================================
    void CheckGrounded()
    {
        // 1. KIỂM TRA CHẠM ĐẤT
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // Reset số lần nhảy khi chạm đất
        if (isGrounded && !isJump)
        {
            jumpsRemaining = maxJumps; isJump = true;
            // Debug.Log("Reset jumps to: " + jumpsRemaining);
        }
    }

    private void MoveHorizontal()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        Vector2 movement = new Vector2(horizontalInput * runSpeed, rb.linearVelocity.y);
        rb.linearVelocity = movement;
        bool isMoving = horizontalInput > 0.05f || horizontalInput < -0.05f;
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("PlayMove", RpcTarget.Others, isMoving); // Gọi RPC để đồng bộ animation cho các client khác
        }
        animCharacter.PlayMove(isMoving); // local animation
        FlipSprite();
    }

    private void Jump()
    {
        // Chỉ cho phép nhảy khi còn lượt nhảy
        if (jumpsRemaining > 0 && isJump)
        {
            // Đặt vận tốc Y về 0 để lực nhảy nhất quán (tránh nhảy thêm lực từ lần rơi trước)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            jumpsRemaining--; // Giảm số lần nhảy

        }
        else
        {
            isJump = false;
        }
    }


    private void FlipSprite()
    {
        float currentAbsScaleX = Mathf.Abs(rb.transform.localScale.x);
        float direction = horizontalInput;
        bool defaultFace = false; // Mặc định hướng mặt về bên trái (Scale X Dương)
        if (Mathf.Abs(direction) > 0.01f)
        {
            float targetScaleX = rb.transform.localScale.x;
            if (direction > 0) // Di chuyển sang PHẢI
            {
                defaultFace = true;
                // Quay Phải (Scale Âm, vì bạn đã định nghĩa 'Quay Phải' là -currentAbsScaleX)
                targetScaleX = -currentAbsScaleX;
                
                photonView.RPC("Flip", RpcTarget.Others, defaultFace); // Gọi RPC để lật sprite cho các client khác
            }
            else if (direction < 0) // Di chuyển sang TRÁI
            {
                defaultFace = false;
                // Quay Trái (Scale Dương)
                targetScaleX = currentAbsScaleX;
                photonView.RPC("Flip", RpcTarget.Others, defaultFace); // Gọi RPC để lật sprite cho các client khác
            }
            transform.localScale = new Vector3(
                targetScaleX,
                transform.localScale.y,
                transform.localScale.z
            ); // Áp dụng Scale X mới locally
        }

    }
    [PunRPC]
    void Flip(bool faceRight) // Hàm này sẽ được gọi trên các client khác
    {
        Vector3 scale = transform.localScale;
        scale.x = faceRight ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}
