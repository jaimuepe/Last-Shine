using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour
{

    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;

    Image image;

    void Start()
    {
        image = GetComponent<Image>();
        Color c = image.color;
        c.a = 1f;
        image.color = c;
        Hide();
    }

    public void Show(bool attenuateSounds)
    {
        StartCoroutine(FadeTo(1f, fadeInTime, attenuateSounds));
    }

    public void Hide()
    {
        StartCoroutine(FadeTo(0f, fadeOutTime, false));
    }

    IEnumerator FadeTo(float aValue, float aTime, bool attenuateSounds)
    {

        AudioSource[] sources = new AudioSource[0];
        float[] defaultVolumes = new float[0];

        if (attenuateSounds)
        {
            sources = FindObjectsOfType<AudioSource>();
            defaultVolumes = new float[sources.Length];

            for (int i = 0; i < sources.Length; i++)
            {
                defaultVolumes[i] = sources[i].volume;
            }
        }

        float alpha = image.color.a;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            if (attenuateSounds)
            {
                for (int i = 0; i < sources.Length; i++)
                {
                    if (sources[i])
                    {
                        sources[i].volume = Mathf.Lerp(defaultVolumes[i], 0f, t);
                    }
                }
            }

            Color newColor = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(alpha, aValue, t));
            image.color = newColor;
            yield return null;
        }
    }
}
