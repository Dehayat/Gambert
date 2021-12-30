using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour
{
    public static BlackScreen instance;

    public System.Action done;

    public float fadeDuration = 1f;
    public Image image;

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private bool isFading = false;
    public void FadeToBlack()
    {
        if (isFading)
        {
            StopAllCoroutines();
        }
        StartCoroutine(FadeTo(Color.black));
    }
    public void FadeFromBlack()
    {
        if (isFading)
        {
            StopAllCoroutines();
        }
        StartCoroutine(FadeTo(new Color(0f, 0f, 0f, 0f)));
    }
    IEnumerator FadeTo(Color end)
    {
        isFading = true;
        float timer = 0f;
        Color start = image.color;
        while (timer < fadeDuration)
        {
            image.color = Color.Lerp(start, end, timer / fadeDuration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        image.color = end;
        isFading = false;
        done?.Invoke();
    }

}
