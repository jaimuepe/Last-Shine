using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventManagerHouse : MonoBehaviour
{

    public float timeBeforeEvent;
    public float timeBeforeSignalOff;
    public float timeBeforeSceneChange;

    public AudioClip explosionClip;
    public AudioClip urgentNewsClip;

    public CameraShake cameraShake;

    public FadePanel fadePanel;

    public GameObject tv;
    public GameObject staticNoise;

    AudioSource audioSource;
    AudioSource audioSourceTv;

    void Start()
    {
        audioSourceTv = tv.GetComponent<AudioSource>();
        audioSourceTv.volume = GameInfo.instance.effects;

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = GameInfo.instance.music;

        StartCoroutine(FireEvent());
    }

    IEnumerator FireEvent()
    {
        yield return new WaitForSeconds(timeBeforeEvent);

        cameraShake.shakeDuration = 1f;
        cameraShake.shakeAmount = 2f;

        audioSource.Stop();
        AudioSource.PlayClipAtPoint(explosionClip, Camera.main.transform.position, GameInfo.instance.effects);

        yield return new WaitForSeconds(4f);

        tv.SetActive(true);
        AudioSource.PlayClipAtPoint(urgentNewsClip, Camera.main.transform.position, GameInfo.instance.effects);
        yield return new WaitForSeconds(timeBeforeSignalOff);

        cameraShake.shakeDuration = 1f;
        cameraShake.shakeAmount = 3f;

        AudioSource.PlayClipAtPoint(explosionClip, Camera.main.transform.position, GameInfo.instance.effects);

        yield return new WaitForSeconds(1f);
        tv.SetActive(false);
        staticNoise.SetActive(true);
        yield return new WaitForSeconds(timeBeforeSceneChange);
        fadePanel.Show(true);
        yield return new WaitForSeconds(1f);
        ChangeScene();
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
