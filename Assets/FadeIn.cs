using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{

    SpriteRenderer sr;

    private void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        float aTime = 0.3f;
        float oldValue = 0f;
        float targetValue = 1f;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            float newValue = Mathf.Lerp(oldValue, targetValue, t);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, newValue);
            yield return null;
        }
    }
}
