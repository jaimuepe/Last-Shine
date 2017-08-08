using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{

    public float delay;

    Image image;

    Coroutine blinkCoroutine;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // This is not a music sound, it's an effect
        audioSource.volume = GameInfo.instance.effects;
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = StartCoroutine(Blink());
    }

    private void OnDisable()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
    }

    IEnumerator Blink()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            Color c = image.color;
            if (c.a > 0)
            {
                c.a = 0f;
            }
            else
            {
                c.a = 1f;
            }
            image.color = c;
        }
    }
}
