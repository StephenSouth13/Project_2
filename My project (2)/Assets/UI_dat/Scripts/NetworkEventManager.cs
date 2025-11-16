using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkEventManager : MonoBehaviourPunCallbacks
{
    public static NetworkEventManager instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    private void OnEvent(EventData photonEvent)
    {
        switch ((InitNetworkInGame)photonEvent.Code)
        {
            case InitNetworkInGame.Spawn:
                
                break;
        }
    }
}

public enum InitNetworkInGame : byte
{
    Spawn = 1
}