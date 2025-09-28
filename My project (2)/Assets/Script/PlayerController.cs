using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Cài đặt Tốc độ và Lực Nhảy (Đã tối ưu cho cảm giác Battle Game)
    [Header("Movement Settings")]
    public float runSpeed = 7.5f;     // Tốc độ
    public float jumpForce = 7f;     // Lực nhảy
    public int maxJumps = 2;          // Số lần nhảy tối đa (Nhảy Đôi)

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
    private int jumpsRemaining; // Số lần nhảy còn lại

    // Khởi tạo
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found! Please attach it to the Player GameObject.");
        }
        jumpsRemaining = maxJumps; // Khởi tạo số lần nhảy
    }

    // Cập nhật Vật lý (FixedUpdate)
    void FixedUpdate()
    {
        // 1. KIỂM TRA CHẠM ĐẤT
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // Reset số lần nhảy khi chạm đất
        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
        }

        // 2. DI CHUYỂN NGANG
        MoveHorizontal();
    }

    // Cập nhật Input (Update)
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 3. NHẢY (Bây giờ đã hỗ trợ Nhảy Đôi)
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        // 4. XOAY CHIỀU SPRITE (Đã sửa lỗi)
        FlipSprite();
    }

    // =========================================================
    // HÀM XỬ LÝ CHUYỂN ĐỘNG
    // =========================================================

    private void MoveHorizontal()
    {
        // **Đã sửa lỗi:** Sử dụng rb.velocity thay vì rb.linearVelocity
        Vector2 movement = new Vector2(horizontalInput * runSpeed, rb.linearVelocity.y);
        rb.linearVelocity = movement;
    }

    private void Jump()
    {
        // Chỉ cho phép nhảy khi còn lượt nhảy
        if (jumpsRemaining > 0)
        {
            // Đặt vận tốc Y về 0 để lực nhảy nhất quán (tránh nhảy thêm lực từ lần rơi trước)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            jumpsRemaining--; // Giảm số lần nhảy
        }
    }

    private void FlipSprite()
    {
        // Đảm bảo có đối thủ để tránh lỗi
        if (opponentTransform == null)
        {
            // Giữ nguyên hướng mặt nếu không có đối thủ
            return;
        }

        float playerX = transform.position.x;
        float opponentX = opponentTransform.position.x;
        float currentAbsScaleX = Mathf.Abs(transform.localScale.x); // Lấy giá trị tuyệt đối của Scale X

        // =========================================================
        // A. Xử lý khi ĐANG CHẠM ĐẤT (Khóa hướng về phía đối thủ)
        // =========================================================
        if (isGrounded)
        {
            // Nếu đối thủ ở bên PHẢI (opponentX > playerX)
            if (opponentX > playerX)
            {
                // Player cần quay mặt sang PHẢI (Scale X phải là dương)
                // Lần này chúng ta kiểm tra, nếu vẫn sai thì đảo dấu ở đây:
                transform.localScale = new Vector3(
                    -currentAbsScaleX, // Dùng GIÁ TRỊ DƯƠNG (quay mặt phải)
                    transform.localScale.y,
                    transform.localScale.z
                );
            }
            // Nếu đối thủ ở bên TRÁI (opponentX < playerX)
            else if (opponentX < playerX)
            {
                // Player cần quay mặt sang TRÁI (Scale X phải là âm)
                // Nếu vẫn sai, đảo dấu ở đây:
                transform.localScale = new Vector3(
                    currentAbsScaleX, // Dùng GIÁ TRỊ ÂM (quay mặt trái)
                    transform.localScale.y,
                    transform.localScale.z
                );
            }
        }
        // =========================================================
        // B. Xử lý khi TRÊN KHÔNG (Giữ nguyên logic cũ)
        // =========================================================
        else if (!isGrounded && canFlipWhileAirborne)
        {
            // Logic này đang hoạt động đúng với input người chơi, giữ nguyên
            float targetDirection = horizontalInput;
            if (Mathf.Abs(targetDirection) > 0.01f)
            {
                transform.localScale = new Vector3(
                    targetDirection * currentAbsScaleX,
                    transform.localScale.y,
                    transform.localScale.z
                );
            }
        }
    }
}