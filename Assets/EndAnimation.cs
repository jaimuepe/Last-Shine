using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAnimation : MonoBehaviour
{

    public AudioClip ac1;
    public AudioClip ac2;

    public AudioClip bgMusic;

    public SceneChanger sceneChanger;

    void Start()
    {
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        AudioSource audioSource = FindObjectOfType<AudioSource>();

        if (audioSource.clip != bgMusic)
        {
            audioSource.clip = bgMusic;
            audioSource.Play();
        }

        yield return new WaitForSeconds(1f);
        if (ac2 != null)
        {
            AudioSource.PlayClipAtPoint(ac2, Camera.main.transform.position);
        }

        yield return new WaitForSeconds(0.5f);
        if (ac1 != null)
        {
            AudioSource.PlayClipAtPoint(ac1, Camera.main.transform.position);
        }

        yield return new WaitForSeconds(1f);

        if (sceneChanger != null)
        {
            sceneChanger.gameObject.SetActive(true);
            sceneChanger.Change();
        }
    }
}
