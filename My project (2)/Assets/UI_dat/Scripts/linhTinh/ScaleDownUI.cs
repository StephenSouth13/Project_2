using UnityEngine;
using System.Collections;
public class ScaleDownUI : MonoBehaviour
{
    RectTransform targetUI;
    public Vector2 startSize = new Vector2(1920f, 1080f);
    public Vector2 endSize = new Vector2(600f, 400f);
    public float scaleDuration = 0.5f; // thời gian thu nhỏ

    void Start()
    {
        OnEnable();
    }
    void OnEnable()
    {
        targetUI = GetComponent<RectTransform>();
        if(targetUI == null)
        {
            Debug.LogError("ScaleDownUI: Không tìm thấy RectTransform trên đối tượng này.");
            return;
        }
        targetUI.sizeDelta = startSize;
        StartCoroutine(ResizeUI());
    }
    IEnumerator ResizeUI()
    {
        float elapsedTime = 0f;
        Vector2 initialSize = targetUI.sizeDelta;
        while(elapsedTime < scaleDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // sử dụng unscaledDeltaTime để bỏ qua Time.timeScale
            float t = elapsedTime / scaleDuration;
            targetUI.sizeDelta = Vector2.Lerp(initialSize, endSize, t);
            yield return null;

        }
        targetUI.sizeDelta = endSize; // đảm bảo kích thước cuối cùng chính xác
    }
}
