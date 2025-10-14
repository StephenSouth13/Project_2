using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    // Cài đặt Tốc độ và Lực Nhảy (Đã tối ưu cho cảm giác Battle Game)
    [Header("Movement Settings")]
    public float runSpeed = 7.5f;     // Tốc độ
    public float jumpForce = 7f;     // Lực nhảy
    public int maxJumps = 2;          // Số lần nhảy tối đa (Nhảy Đôi)
    public bool isJump;

    [Header("Combat Interaction")]
    public Transform opponentTransform; // Tham chiếu đến đối tượng Đối thủ (Enemy)
    public bool canFlipWhileAirborne = true; // Biến bật/tắt logic xoay khi nhảy (Mặc định là BẬT)
    // Lưu ý: Biến này cần được Dev 2 cung cấp thông tin (tham chiếu đến kẻ địch Dummy/Thật)

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    // Thành phần và Trạng thái
    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;
    public int jumpsRemaining; // Số lần nhảy còn lại
    private PhotonView photonView;
    private Animator anim;
    // Khởi tạo
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator not found! Check if it's on UniRoot."); 
        }
        jumpsRemaining = maxJumps; // Khởi tạo số lần nhảy
    }

    // Cập nhật Vật lý (FixedUpdate)
    void FixedUpdate()
    {
        // 1. KIỂM TRA CHẠM ĐẤT
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // Reset số lần nhảy khi chạm đất
        if (isGrounded && !isJump)
        {
            jumpsRemaining = maxJumps; isJump = true;
        }


        // 2. DI CHUYỂN NGANG
        MoveHorizontal();
        
    }

    // Cập nhật Input (Update)
    void Update()
    {
   
        if (photonView.IsMine)
        {
            HandleInput();
            UpdateAnimations();
        }
        else
        {
            
            // Hiện tại không làm gì, chỉ cho phép PhotonView Component tự đồng bộ.
        }
    }
    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        //Debug.Log("Horizontal Input: " + horizontalInput); 
        // 3. NHẢY (Bây giờ đã hỗ trợ Nhảy Đôi)
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        // 4. XOAY CHIỀU SPRITE 
        FlipSprite();
    }


    private void MoveHorizontal()
    {
        rb.linearVelocity = new Vector2(horizontalInput * runSpeed, rb.linearVelocity.y);
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
        
        float currentAbsScaleX = Mathf.Abs(transform.localScale.x);
        float direction = horizontalInput;

      
        if (Mathf.Abs(direction) > 0.01f)
        {
            float targetScaleX = transform.localScale.x;

            if (direction > 0) // Di chuyển sang PHẢI
            {
                targetScaleX = -currentAbsScaleX;
            }
            else if (direction < 0) // Di chuyển sang TRÁI
            {
                targetScaleX = currentAbsScaleX;
            }

            // Áp dụng Scale X mới
            transform.localScale = new Vector3(
                targetScaleX,
                transform.localScale.y,
                transform.localScale.z
            );
        }
       
        else if (opponentTransform != null) // Nếu không có input di chuyển, quay mặt về phía đối thủ
        {
            float playerX = transform.position.x;
            float opponentX = opponentTransform.position.x;
            float targetScaleX = (opponentX > playerX) ? -currentAbsScaleX : currentAbsScaleX;

            transform.localScale = new Vector3(
                targetScaleX,
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    private void UpdateAnimations()
    {
        if (anim == null) return;

        // 1. Run/Idle
        bool IsMoving = Mathf.Abs(horizontalInput) > 0.01f; 
        anim.SetBool("1_Move", IsMoving); 

        
    }
}
