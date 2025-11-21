using UnityEngine;
using Photon.Pun;
public class GameEndManager : MonoBehaviourPun
{
    public static GameEndManager instance;

    void Awake() => instance = this;

    [PunRPC]
    public void SetGlobalTimeScale(float scale)
    {
        foreach(var p in CameraZoom.instance.player)
        {
            if (p != null)
            {
                var movement = p.GetComponent<PhotonPlayerMovement>();
                if (movement != null)
                {
                    movement.enabled = scale > 0f; // Chỉ cho phép di chuyển khi timeScale > 0
                }
                var combat = p.GetComponent<CombatCharacter>();
                if (combat != null)
                {
                    combat.enabled = scale > 0f; // Chỉ cho phép tấn công khi timeScale > 0
                }
            }
        }
        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}

