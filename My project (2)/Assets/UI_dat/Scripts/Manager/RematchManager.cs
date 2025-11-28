using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum RematchEventCode : byte {leftRoom = 0, RequestRematch = 1, ReceiveRequest = 2 , AcceptRematch = 3, DecLineRematch = 4 , BackToLobby = 5}
public enum RoomState { InGame, PostMatch, opponentLeftRoom , ReceiveMatch, RematchPending, RematchAccepted, RematchDeclined, ReturningToLobby}
public class RematchManager : MonoBehaviourPun
{ 
    public GameObject statusText; 
    public GameObject rematchPanel;
    public Button accept_btn;
    public Button decline_btn;
    public static RematchManager instance;

    string fullText;
    void Awake() => instance = this;
    void Start()
    {
        accept_btn.onClick.AddListener(() =>
        {
            GetPlayerIntent();
            GetRoomState();
            Debug.Log("đã gọi");
            SynceIntentAndShow(3);
            photonView.RPC("SynceIntentAndShow", RpcTarget.Others, 3);

        });
        decline_btn.onClick.AddListener(() =>
        {
            SynceIntentAndShow(5);
            photonView.RPC("SynceIntentAndShow",RpcTarget.Others,4);

        });

    }
    public void RequestRematch() // nếu tự mình yêu cầu rematch thì sẽ tự set RematchIntent = 1 và vào trạng thái chờ đợi
    {
        Debug.Log("[RematchManager] Yêu cầu rematch được gửi.");
        // Thêm logic rematch ở đây
        SetPlayerIntent(1); // 1 = muốn 
        
        photonView.RPC("SynceIntentAndShow", RpcTarget.Others, 2);

    }
    [PunRPC]
    public void SynceIntentAndShow(int b) 
    {
        byte intent = (byte)b;
        SetPlayerIntent(intent);
    }
    
    
    public void SetRoomState(RoomState newState)
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["RoomState"] = newState;
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        Debug.Log("[SetRoomState] RoomState = " + newState);
        OnRoomStateChanged(newState);
        
        
    }
    public RoomState GetRoomState() // tạm thời chưa dùng do không get về đúng
    {
        RoomState state = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RoomState") ?
            (RoomState)PhotonNetwork.CurrentRoom.CustomProperties["RoomState"] : RoomState.InGame;
        Debug.Log("[GetRoomState] RoomState = " + state);
        
        return state;
    }
    public void SetPlayerIntent(byte intent) // tự set local
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["RematchIntent"] = intent;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        Debug.Log("[SetPlayerIntent] RematchIntent = " + intent);
        int b = (int)intent;
        ShowRematchPanel(true, b);
    }
    public byte GetPlayerIntent() // tạm thời chưa dùng do không get về đúng
    {
        byte intent = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("RematchIntent") ? 
            (byte)PhotonNetwork.LocalPlayer.CustomProperties["RematchIntent"] : (byte)0;
        Debug.Log("[GetPlayerIntent] RematchIntent = " + intent);
        
        return intent;
    }
    [PunRPC]
    public void ShowRematchPanel(bool show, int b) // đông bộ trạng thái panel rematch dựa trên ý định của người chơi
    {
        rematchPanel.SetActive(show); // lấy về trạng thái của mình 
        
        switch (b)
        {
            case 0:
                Debug.Log("[RematchManager] Hiển thị panel rematch thông báo cho người chơi là đối thủ đã rời đi .");
                ShowObject(false);
                SetRoomState(RoomState.opponentLeftRoom);

                break;
            case 1:
                Debug.Log("[RematchManager] Hiển thị panel rematch cho chính người chơi gửi yêu cầu - chờ đợi .");
                SetRoomState(RoomState.RematchPending);
                ShowObject(false);
                // chời nhận lại trạng thái từ người chơi
                // sẽ tự set type 3 hoặc 4 tương tự khi oponent gửi SynceIntentAndShow qua để tự set local 3 - 4
                break;
            case 2:
                Debug.Log("[RematchManager] Hiển thị panel rematch cho người nhận để xem sét");
                SetRoomState(RoomState.ReceiveMatch);
                ShowObject(true);
                // sẽ có giới hạn thời gian ở đây tương tự người gửi đang chờ 

                // khi hết sẽ vào thẳng trạng thái DecLineRematch = 4; - rời phòng
                break;
            case 3:
                Debug.Log("[RematchManager] người chơi đã đồng ý rematch.");
                SetRoomState(RoomState.RematchAccepted);
                ShowObject(false);

                // Chờ 1 logic để bên local và bên khác cùng nhận 1 chỉ thị rematch game
                // khi người chơi nhấn accept thì sẽ tự set local byte = 3 và SynceIntentAndShow cho oponent để họ vào byte = 3
                // logic để reMatch ván game
                break;
            case 4:
                Debug.Log("[RematchManager] người chơi đã từ chối rematch.");
                SetRoomState(RoomState.RematchDeclined);
                ShowObject(false);
                // gửi text thông báo bên còn lại

                // khi này người chơi sẽ nhận thông báo từ chối và chuẩn bị RoomState.ReturningToLobby

                break;
            case 5:
                SetRoomState(RoomState.ReturningToLobby);
                ShowObject(false);

                break;
            default:
                Debug.LogWarning("[RematchManager] Trạng thái ý định rematch không xác định.");
                break; 
        }
    }
    public void ShowObject(bool show)
    {
        accept_btn.gameObject.SetActive(show);
        decline_btn.gameObject.SetActive(show);
    }
    public void OnRoomStateChanged(RoomState newState) // tự thay đổi và set bên khác RoomState
    {
        
        TypingWithEllipsisUI type = statusText.GetComponent<TypingWithEllipsisUI>();
        if(type == null) 
        {
            Debug.Log("[OnRoomStateChanged] type == null"); 
            return;
        }
        type.ResetAll();
        //RoomState chỉ cần bắt sự kiện và viết text hiển thị coroutine;
        //Sau khi hết coroutine thì sẽ có hàm tiếp ứng 
        switch (newState)
        {
            case RoomState.opponentLeftRoom:

                
                fullText = "Opponent left the room";
                type.SetTimeRead(5);
                type.SetTimeCountDown(5);
                type.StartTyping(fullText);

            
                
            
            
                break;
            case RoomState.RematchPending:
                fullText = "Waiting for opponent is rematch";
                type.SetTimeCountDown(15);
                type.StartTyping(fullText); // bắt đầu countDown 10s
                // khi hết thời gian
                // tự set vào RoomState.ReturningToLobby

                break;
            case RoomState.ReceiveMatch:
                fullText = "Opponent invites a rematch. Accept?";
                type.SetTimeCountDown(15);
                type.StartTyping(fullText); // bắt đầu countDown 10s

                // khi hết thời gian
                // tự set vào RoomState.ReturningToLobby
                break;
            case RoomState.RematchAccepted:
                fullText = "Rematch confirmed! Starting in";
                type.SetTimeCountDown(10);
                type.StartTyping(fullText);
                
                break;
            case RoomState.RematchDeclined:
                fullText = "Opponent declined. Returning to lobby.";
                type.SetTimeCountDown(5);

                type.StartTyping(fullText);// bắt đầu countDown 3s
                
                // ở đây người chơi nhận Synce từ người từ chối và tiến vào đây 

                // thông báo text

                // khi hết thời gian
                // tự set vào RoomState.ReturningToLobby
                break;
            
            case RoomState.ReturningToLobby:
                fullText = "No response. Returning to lobby";
                type.SetTimeCountDown(7);
                type.StartTyping(fullText);

                // khi hết thời gian
                // LoadScene và leaveRoom - JoinLobby
                break;
            default:
                fullText = "BUG";
                //No response. Returning to lobby.
                //Left: “Opponent left the room
                break;
        }
    }
}
