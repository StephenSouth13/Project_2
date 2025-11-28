using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class TypingWithEllipsisUI : MonoBehaviour
{
    public TextMeshProUGUI targetText;

    [Header ("Content")]
    public string fullContent;
    public float charDelay = 0.05f; // thời gian đánh từng chữ
    public bool useUnScaleTime = true;
    [Header ("dot")]
    public float ellipsisInterval = 0.5f;
    public int maxDots = 3;
    public bool showEllipsisAfterTyping = true;
    [Header ("CountDown")]
    public bool showCountDownAfterTyping = true;
    public int countDownSeconds = 3;
    public string countDownFormat = "<color=#FF0000>({0})</color>";
    public bool doneCountDown = false;

    [Header ("StopToRead")]
    public int timeToRead = 3;
    public bool isStopingToRead = false;


    Coroutine coroutine;

    void Start()
    {
        OnEnable();
    }
    public void ResetAll()
    {
        useUnScaleTime =true;
        showEllipsisAfterTyping = true;
        showCountDownAfterTyping = true;
        doneCountDown = false;
        isStopingToRead = false;
    }
    public void OnEnable()
    {
        if(targetText == null) targetText = GetComponent<TextMeshProUGUI>();
    }
    public void StartTyping(string fullText)
    {
        fullContent = fullText;
        StopTyping();
        coroutine = StartCoroutine(TypingFlow());
    }
    public void StopTyping()
    {
        if(coroutine != null) StopCoroutine(coroutine);
        coroutine = null;
    }
    IEnumerator TypingFlow()
    {
        targetText.text = "";
        if (string.IsNullOrEmpty(fullContent)) yield break;

        // gõ từng chữ
        for (int i = 0; i < fullContent.Length; i++)
        {
            targetText.text += fullContent[i];
            yield return Wait(charDelay);
        }
        
        if (!showEllipsisAfterTyping) yield break;

        if (!showCountDownAfterTyping)
        {
            int dot = 0;
            while (true)
            {
                dot = (dot % maxDots) + 1;
                targetText.text = fullContent + new string('.', dot);
                yield return Wait(ellipsisInterval);
            }
            
        }
        
        if(showCountDownAfterTyping)
        {
            // dùng float để đếm thời gian thực
            float remaining = (float)countDownSeconds;
            int dot = 0;
            float dotTimer = 0f;
            float dotInterval = Mathf.Max(0.01f, ellipsisInterval);

            // vòng chạy: cập nhật dot theo dotInterval, countdown theo delta time
            while (remaining > 0f)
            {
                // cập nhật thời gian từng frame
                float dt = useUnScaleTime ? Time.unscaledDeltaTime : Time.deltaTime;

                // cập nhật dot timer
                dotTimer += dt;
                if (dotTimer >= dotInterval)
                {
                    // tăng dot theo số lần vượt interval (trong trường hợp dt > dotInterval)
                    int steps = Mathf.FloorToInt(dotTimer / dotInterval);
                    dotTimer -= steps * dotInterval;
                    dot = ((dot + steps - 1) % maxDots) + 1;
                }

                // giảm countdown theo thời gian thực
                remaining -= dt;
                if (remaining < 0f)
                {
                    remaining = 0f;
                    doneCountDown = true;
                }

                // hiển thị: bạn có thể dùng Ceil để hiển thị 3,2,1 hoặc Floor tuỳ ý
                int displaySeconds = Mathf.CeilToInt(remaining); // hiển thị 3..2..1
                targetText.text = fullContent + string.Format(countDownFormat, displaySeconds) + new string('.', dot);

                yield return null; // chạy mỗi frame để mượt
            }

            // khi countdown về 0 -> dừng hẳn hoặc hiển thị final
            if (doneCountDown)
            {
                RoomState roomState = RematchManager.instance.GetRoomState();
                Debug.Log("[TypingWithEllipsisUI] roomState : " + roomState);
                
                
                if(roomState == RoomState.ReturningToLobby || roomState == RoomState.RematchDeclined)
                {
                    PhotonRoomManager.instance.leaveRoom();
                }
                if(roomState == RoomState.opponentLeftRoom && isStopingToRead)
                {
                    fullContent = "Prepare to leave the room in";
                    ResetAll();
                    SetTimeCountDown(5);
                    StartTyping(fullContent);
                }
                else if (roomState == RoomState.opponentLeftRoom && !isStopingToRead)
                {
                    PhotonRoomManager.instance.leaveRoom();
                    
                }
                if(roomState == RoomState.ReceiveMatch || roomState == RoomState.RematchPending )
                {
                    RematchManager.instance.SynceIntentAndShow(5);
                }
                else if(roomState == RoomState.RematchAccepted)
                {
                    Debug.Log("Start new game");        
                }
            }
            
        }
    }
    IEnumerator Wait(float seconds)
    {
        if (useUnScaleTime)
        {
            float t = 0f;
            while(t < seconds)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }

        }
        else
        {
            yield return new WaitForSeconds(seconds);
        }
    }
    public void FinishAndStopEllipsis()
    {
        StopTyping();
        targetText.text = fullContent;
    }
    public void SetTimeCountDown(int seconds) // gọi trước startTyping()
    {
        showCountDownAfterTyping = true;
        countDownSeconds = seconds;
    }
    public void SetTimeRead(int seconds)
    {
        isStopingToRead = true;
        timeToRead = seconds;
    }
}
