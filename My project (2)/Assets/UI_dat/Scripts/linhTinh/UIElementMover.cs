using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIElementMover  :  MonoBehaviour, ISelectHandler, IDeselectHandler , IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler

{
    // Công dụng : Di chuyển text khi nhấn nút button theo targetPos đã đặt sẵn
    public Button button;
    public TextMeshProUGUI text;
    public Image image;
    public Vector2 targetPos;
    public bool isPressed = false;
    public Outline outline;
    [SerializeField] bool isOver = false; // Vẫn chưa dùng đến logic này nhưng tạm thời giữ lại để sau này có thể phát triển thêm
    Vector2 startPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void Init()
    {
        if (text != null)
        {
            startPos = text.rectTransform.anchoredPosition;
        }
        if (image != null)
        {
            startPos = image.rectTransform.anchoredPosition;
        }
    }
    // void OnButtonClick()
    // {
       
    // }
    void moveText(bool IsPressed)
    {
        Vector2 destPos = (IsPressed) ? targetPos : startPos;
        if (text != null)
        {
            text.rectTransform.anchoredPosition = destPos;
        }
        if (image != null)
        {
            image.rectTransform.anchoredPosition = destPos;
        }
    }
    public void OnPointerEnter(PointerEventData eventData) // khi lia chuột vào khu vực button sẽ chạy
    {
        isOver = true;
        if(outline != null)
        {
            outline.enabled = true;
        }
    }
    public void OnPointerExit(PointerEventData eventData) // khi rời chuột khỏi khu vực button sẽ chạy
    {
        isOver = false;
        if(outline != null)
        {
            outline.enabled = false;
        }
    }
    public void OnPointerDown(PointerEventData evenData) // khi nhấn chuột vào khu vực button sẽ chạy
    {
        isPressed = true;
        moveText(isPressed);
        // Debug.Log("down");
        
    }
    public void OnPointerUp(PointerEventData eventData) // khi thả chuột ra khỏi khu vực button sẽ chạy
    {
        isPressed = false;
        moveText(isPressed);
        // Debug.Log("up");
        // dừng focus khi thả chuột ra ngoài button
        if (!isOver)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    public void OnSelect(BaseEventData eventData) // Gọi 1 lần duy nhất khi được chọn (focus)
    {
        // isOver = true;

        // Debug.Log("select");
    }
    public void OnDeselect(BaseEventData eventData) // Click ra ngoài hoặc click 1 UI khác để hủy chọn
    {
        // isOver = false;
        // Debug.Log("deselect");
    }
}
