using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Photon.Pun;
public class GetAVTCharacter : MonoBehaviourPun
{
    public Transform SpawnPos;
    public Vector2 scaleAVT;
    void Start()
    {
    }
    public void Spawn(GameObject clone)
    {
        DestroyChildren(SpawnPos.gameObject);
        Transform child = clone.transform.Find("UnitRoot/Root/BodySet/P_Body/HeadSet");
        if(child != null)
        {
            GameObject head = Instantiate(child.gameObject,SpawnPos.position, Quaternion.identity,SpawnPos.transform);
            head.transform.localScale = scaleAVT;
            SortingGroup sortingGB = head.AddComponent<SortingGroup>();
            sortingGB.sortingLayerName = "Game";
            sortingGB.sortingOrder = 1;
            Transform pHead = head.transform.Find("P_Head");
            if(pHead != null)
            {
                pHead.transform.localRotation = Quaternion.identity;
            }
        }
    }
    void DestroyChildren(GameObject parent)
    {
        Transform t = parent.transform;
        if (t.childCount > 0)
        {
            for (int i = t.childCount - 1; i >= 0; i--) // duyệt ngược để tránh lỗi
            {
                GameObject child = t.GetChild(i).gameObject;
                Destroy(child);
            }
        }
    }

}
