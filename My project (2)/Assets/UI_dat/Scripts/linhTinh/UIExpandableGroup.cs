using System.Collections.Generic;
using Cainos.LucidEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIExpandableGroup : MonoBehaviour
{
    // Công dụng : Sổ ra các nút chức năng khi nhấn nút
    public Button toggleButton;
    public List<Button> menuFuntions_Btns = new List<Button>(); // danh sách các nút chức năng bên trong menu
    public float moveSpeed; // tốc độ di chuyển các nút chức năng trong menu lưu ý: >500f để di chuyển nhanh hơn
    public float spaceBetweenItems; // khoảng cách giữa các nút chức năng trong menu
    public bool upStyle = true; // kiểu mở menu theo hướng lên/xuống
    public bool sideRightStyle = false; // kiểu mở menu theo hướng sang phải/trái
    [SerializeField] bool isSameSide = false;
    [ShowIf("isSameSide")]
    [SerializeField] float firstItemOffset; // khoảng cách của nút chức năng đầu tiên so với nút toggle
    bool isExpanded = true; // trạng thái mở/đóng menu
    Vector2 startPos;
    void Awake()
    {
        Init();
    }
    void Start()
    {
        toggleButton.onClick.AddListener(ToggleOnClick);
    }
    void Update()
    {
        if (upStyle)
            Open_Up();
        if (sideRightStyle)
            Open_SideRight();
    }
    void Open_Up() // di chuyển theo hướng lên/xuống các nút chức năng trong menu
    {
        if (isExpanded)
        {
            for (int i = 0; i < menuFuntions_Btns.Count; i++)
            {
                RectTransform btnRect = menuFuntions_Btns[i].GetComponent<RectTransform>();
                MoveItemToPos(startPos, btnRect);
            }
        }
        else
        {
            for (int i = 0; i < menuFuntions_Btns.Count; i++)
            {
                float offsetY = (i + 1) * spaceBetweenItems; // khoảng cách giữa các nút chức năng trong menu
                if (!isSameSide)
                {
                    offsetY += firstItemOffset;
                }
                Vector2 targerPos = new Vector2(startPos.x, startPos.y + offsetY);
                RectTransform btnRect = menuFuntions_Btns[i].GetComponent<RectTransform>();
                MoveItemToPos(targerPos, btnRect);
            }
        }
    }
    void Open_SideRight()
    {
        if (isExpanded)
        {
            for (int i = 0; i < menuFuntions_Btns.Count; i++)
            {
                RectTransform btnRect = menuFuntions_Btns[i].GetComponent<RectTransform>();
                MoveItemToPos(startPos, btnRect);
            }
        }
        else
        {
            for (int i = 0; i < menuFuntions_Btns.Count; i++)
            {
                float offsetX = (i + 1) * spaceBetweenItems; // khoảng cách giữa các nút chức năng trong menu
                if (!isSameSide) 
                {
                    offsetX += firstItemOffset;
                }
                Vector2 targerPos = new Vector2(startPos.x + offsetX, startPos.y);
                RectTransform btnRect = menuFuntions_Btns[i].GetComponent<RectTransform>();
                MoveItemToPos(targerPos, btnRect);
            }
        }
    }

    void Init() // khởi tạo vị trí ban đầu của các nút chức năng trong menu
    {
        startPos = toggleButton.GetComponent<RectTransform>().anchoredPosition;
    }
    void MoveItemToPos(Vector2 targetPos, RectTransform btn) // di chuyển nút về vị trí targetPos
    {
        if (!isExpanded)
        {
            btn.gameObject.SetActive(true);
        }
        if (Vector2.Distance(btn.anchoredPosition, targetPos) > 0.05f)
        {
            btn.anchoredPosition = Vector2.MoveTowards(btn.anchoredPosition, targetPos, Time.deltaTime * moveSpeed);
        }
        else
        {
            btn.anchoredPosition = targetPos;
            btn.gameObject.SetActive(!isExpanded);
        }
    }
    public void ToggleOnClick() // khi nhấn nút menu sẽ chạy
    {
        isExpanded = !isExpanded;
    }
}
