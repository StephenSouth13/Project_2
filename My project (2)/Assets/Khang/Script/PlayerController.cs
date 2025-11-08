using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    // Cài đặt Tốc độ và Lực Nhảy (Đã tối ưu cho cảm giác Battle Game)
    [Header("Movement Settings")]
    public float runSpeed = 7.5f;     // Tốc độ
    public float jumpForce = 7f;      // Lực nhảy
    public int maxJumps = 2;          // Số lần nhảy tối đa (Nhảy Đôi)
    public bool isJump;

    [Header("Dash Settings")]
    public float dashForce = 15f; // Lực đẩy Dash (Tùy chọn)
    public float dashDuration = 0.2f; // Thời gian Dash
    public float dashCooldown = 1f; // Cooldown Dash
    private float nextDashTime = 0f;
    private bool isDashing = false; // Trạng thái đang Dash

    // ĐỊNH NGHĨA KEY SFX (Phải khớp với Key trong AudioManager Inspector)
    private const string JUMP_SFX_KEY = "Jump";

    [Header("Combat Interaction")]
    public Transform opponentTransform;
    public bool canFlipWhileAirborne = true;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Jump VFX")]
    public GameObject jumpVFXPrefab;
    public Transform vfxSpawnPoint;

    // Thành phần và Trạng thái
    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;
    public int jumpsRemaining;
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

        // Đã xóa logic tìm kiếm AudioSource cục bộ
    }

    // Cập nhật Vật lý (FixedUpdate)
    void FixedUpdate()
    {
        // 1. KIỂM TRA CHẠM ĐẤT
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // Reset số lần nhảy khi chạm đất
        if (isGrounded && !isJump)
        {
            jumpsRemaining = maxJumps;
            isJump = true;
        }
      
        // Ngăn chặn di chuyển khi đang Dash
        if (!isDashing)
        {
            // 2. DI CHUYỂN NGANG
            MoveHorizontal();
        }
    }

    // Cập nhật Input (Update)
    void Update()
    {
        // QUAN TRỌNG: Chỉ client sở hữu mới xử lý Input
        if (photonView.IsMine)
        {
            //Debug.Log("Player đang xử lý Input (IsMine = TRUE)");
            //HandleInput();
            //UpdateAnimations();
            // Chặn Input khi đang Dash (không cho nhảy, di chuyển)
            if (isDashing) return;

            Debug.Log("Player đang xử lý Input (IsMine = TRUE)");
            HandleInput();
            UpdateAnimations();
        }
        // Logic đồng bộ vị trí/Animation cho client không sở hữu (được xử lý bởi PhotonView/Photon Transform View)
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 3. NHẢY (Hỗ trợ Nhảy Đôi)
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Input Nhảy được nhận!");
            Jump();
        }

        // INPUT TỐC BIẾN (DASH) - NÚT L
        // =========================================================
        bool canDash = Time.time >= nextDashTime;
        if (Input.GetKeyDown(KeyCode.L) && canDash)
        {
            PerformDash();
        }

        // 4. XOAY CHIỀU SPRITE 
        FlipSprite();
    }


    private void MoveHorizontal()
    {
        // Sử dụng velocity để di chuyển
        rb.linearVelocity = new Vector2(horizontalInput * runSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        // Chỉ cho phép nhảy khi còn lượt nhảy
        if (jumpsRemaining > 0 && isJump)
        {
            // Đặt vận tốc Y về 0
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // GỌI HÀM TẠO VFX NGAY SAU KHI NHẢY (Chỉ nhảy lần đầu)
            if (jumpsRemaining == maxJumps) // Chỉ khi nhảy từ mặt đất
            {
                SpawnJumpVFX();
            }

            jumpsRemaining--; // Giảm số lần nhảy

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.Play2D(JUMP_SFX_KEY);
            }
        }
        else
        {
        
            isJump = false;
        }
    }

    // THÊM HÀM RPC ĐỂ ĐỒNG BỘ ÂM THANH
    [PunRPC]
    void Rpc_PlaySFX(string sfxKey)
    {
        // VỊ TRÍ 2: Xác nhận NHẬN lệnh RPC và cố gắng phát
        Debug.Log($"2. NHẬN RPC, cố gắng phát SFX: {sfxKey}");
        // TẤT CẢ client sẽ chạy lệnh này, phát SFX qua AudioManager
        if (AudioManager.Instance != null)
        {
            // Âm thanh 2D (Play2D) dùng cho SFX của Player/UI
            AudioManager.Instance.Play2D(sfxKey);
        }
        else
        {
            Debug.LogError("AudioManager.Instance là NULL! Không thể phát SFX.");
        }
    }

    private void FlipSprite()
    {
        // ... (Giữ nguyên logic xoay Sprite)
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
        anim.SetBool("isMoving", IsMoving);
    }

    void SpawnJumpVFX()
    {
        if (jumpVFXPrefab == null || vfxSpawnPoint == null)
        {
            Debug.LogWarning("Jump VFX Prefab or Spawn Point missing in PlayerController!");
            return;
        }
        GameObject vfx = Instantiate(jumpVFXPrefab, vfxSpawnPoint.position, Quaternion.identity);
        vfx.transform.localScale = transform.localScale; // Đảm bảo VFX cùng hướng với Player


    }

    private void PerformDash()
    {
        // Đặt Cooldown
        nextDashTime = Time.time + dashCooldown;

        // Kích hoạt Animation Dash (Cần Trigger "Dash" trong Animator)
        if (anim != null)
        {
            anim.SetTrigger("Dash");
        }

        // Bắt đầu logic Dash (Dùng Coroutine để xử lý thời gian)
        StartCoroutine(DashCoroutine());
    }

    private System.Collections.IEnumerator DashCoroutine()
    {
        isDashing = true; // Bật trạng thái Dash

        // Lưu lại gravity scale cũ
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f; // Tắt trọng lực
        rb.linearVelocity = Vector2.zero;

        // Tính toán hướng Dash
        float direction = Mathf.Sign(transform.localScale.x); // Lấy hướng hiện tại của nhân vật
        Vector2 dashDirection = new Vector2(-direction * dashForce, 0f); // Nhân với -direction vì scale X đang là âm khi quay phải

        rb.linearVelocity = dashDirection;
        Debug.Log($"Dash launched! Velocity: {rb.linearVelocity}");

        // Chờ đợi thời gian Dash
        yield return new WaitForSeconds(dashDuration);

        // Kết thúc Dash
        rb.gravityScale = originalGravity; // Khôi phục trọng lực
        isDashing = false; // Tắt trạng thái Dash
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Đặt vận tốc Y về 0 để tránh va chạm ngay sau Dash
    }



}