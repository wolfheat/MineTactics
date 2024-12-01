using System.Collections;
using TMPro;
using UnityEngine;

public class FadableInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI info_text;
    [SerializeField] Color startColor;
    [SerializeField] Color endColor;
    [SerializeField] float fadeTime = 2f;
    [SerializeField] float startFadeAt = 1f;


    public static FadableInfo Instance { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void ShowInfo(string info)
    {
        StartCoroutine(TimedFade());
        info_text.text = info;
    }

    private IEnumerator TimedFade()
    {
        float timer = 0;
        while(timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer < startFadeAt ? 0 : (timer-startFadeAt / fadeTime);
            info_text.color = Color.Lerp(startColor,endColor,t);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
