using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public FadePanel fadePanel;
    public bool attenuateSounds = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerGroundCheck"))
        {
            StartCoroutine(NextScene());
        }
    }

    public void Change()
    {
        StartCoroutine(NextScene());
    }

    IEnumerator NextScene()
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            GameInfo.instance.score = FindObjectOfType<UIManager>().score;
        }
        fadePanel.Show(attenuateSounds);
        yield return new WaitForSeconds(fadePanel.fadeInTime);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
