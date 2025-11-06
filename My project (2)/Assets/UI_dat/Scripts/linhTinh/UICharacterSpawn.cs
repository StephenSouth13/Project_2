using UnityEngine;
using UnityEngine.UI;
public class UICharacterSpawn : MonoBehaviour
{
    public Button button;
    public GameObject prefabCharracter;
    public RectTransform spawnPos;
    public GameObject Prefab_Container_status;
    public RectTransform spawnPosStatus;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnCharacter(prefabCharracter, spawnPos, new Vector3(140f,140f, 1f));
        button.onClick.AddListener(spawnPrefabStatus);
    }
    void SpawnCharacter(GameObject Prefab, RectTransform rect, Vector3 scale)
    {
        GameObject characterInstan = Instantiate(Prefab, rect);
        RectTransform charRec = characterInstan.GetComponent<RectTransform>();

        charRec.anchoredPosition = Vector2.zero;
        charRec.localScale = scale;
        Rigidbody2D charRB = characterInstan.GetComponent<Rigidbody2D>();
        charRB.bodyType = RigidbodyType2D.Static;
    }
    public void spawnPrefabStatus()
    {
        ClearAllChill(spawnPosStatus.gameObject);
        GameObject statusInstan = Instantiate(Prefab_Container_status, spawnPosStatus);
        RectTransform statusRec = statusInstan.GetComponent<RectTransform>();
        statusRec.anchoredPosition = Vector2.zero;
        Transform spawnPos2 = statusInstan.transform.Find("SpawnPoint_objChar");
        if (spawnPos2 != null)
        {
            RectTransform pos = spawnPos2.gameObject.GetComponent<RectTransform>();
            SpawnCharacter(prefabCharracter, pos, new Vector3(400f, 400f, 1f));
        }
        var allUISpawner = FindObjectsByType<StatusUiSpawner>(FindObjectsSortMode.None);
        foreach (var s in allUISpawner)
        {
            s.characterPrefab = prefabCharracter;
            s.Init();
        }
    }
    public void ClearAllChill(GameObject container)
    {
        foreach(Transform chill in container.transform)
        {
            Destroy(chill.gameObject);
        }
    }
}
