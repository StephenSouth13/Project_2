using TMPro;
using UnityEngine;

public class PhotonStatus : MonoBehaviour
{
    private string photonStatus;
    public TextMeshProUGUI status_Txt;

    void Update()
    {
        photonStatus = Photon.Pun.PhotonNetwork.NetworkClientState.ToString();
        status_Txt.text = "Photon Status: " + photonStatus;
    }
}
