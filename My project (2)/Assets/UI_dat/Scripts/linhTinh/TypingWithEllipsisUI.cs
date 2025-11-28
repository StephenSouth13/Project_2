using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
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

    Coroutine coroutine;

    void Awake()
    {
        OnEnable();
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
        if (!string.IsNullOrEmpty(fullContent))
        {
            for(int i = 0; i < fullContent.Length; i++)
            {
                targetText.text += fullContent[i];
                yield return Wait(charDelay);
            }
            if (showEllipsisAfterTyping)
            {
                int dot = 0;
                while (true)
                {
                    dot = (dot % maxDots) + 1;
                    targetText.text = fullContent + new string('.',dot);
                    yield return Wait(ellipsisInterval);
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
}
