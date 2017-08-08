using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PleaseWaitAnimation : MonoBehaviour
{
    public float delay;
    Text text;

    int i = 0;
    Coroutine waitCoroutine;

    private void Start()
    {
        text = GetComponent<Text>();
        waitCoroutine = StartCoroutine(Animate());
    }

    void OnEnable()
    {
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
        }
        waitCoroutine = StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        while (true)
        {

            yield return new WaitForSeconds(delay);
            i = (i + 1) % 3;
            if (i == 0)
            {
                text.text = "PLEASE WAIT...";
            }
            else if (i == 1)
            {
                text.text = "PLEASE WAIT..";
            }
            else
            {
                text.text = "PLEASE WAIT.";
            }
        }
    }
}
