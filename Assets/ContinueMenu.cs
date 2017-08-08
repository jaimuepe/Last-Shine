using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContinueMenu : MonoBehaviour
{

    public float timeBeforeBackToMenu;

    public Text text;
    public FadePanel fadePanel;

    public Button yesBt;
    public Button nobt;

    public AudioClip countDown;

    Coroutine countdownCoroutine;

    IEnumerator Countdown()
    {
        int time = 10;
        while (time >= 0)
        {
            text.text = time.ToString();
            yield return new WaitForSeconds(1f);
            AudioSource.PlayClipAtPoint(countDown, Camera.main.transform.position, GameInfo.instance.effects);
            time -= 1;
        }

        Exit();
    }

    public void OnEnable()
    {
        Show();
    }

    public void Show()
    {
        MyButton bt = yesBt.GetComponent<MyButton>();
        bt.ignoreSelection = true;
        yesBt.Select();
        bt.ignoreSelection = false;

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
        countdownCoroutine = StartCoroutine(Countdown());
    }

    public void Continue()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
        Player player = FindObjectOfType<Player>();
        player.Respawn();
        EventSystem.current.SetSelectedGameObject(null);
        gameObject.SetActive(false);
    }

    public void Exit()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
        GameOver();
    }

    void GameOver()
    {
        yesBt.gameObject.SetActive(false);
        nobt.gameObject.SetActive(false);

        StartCoroutine(IGameOver());
    }

    IEnumerator IGameOver()
    {
        fadePanel.Show(true);
        yield return new WaitForSeconds(fadePanel.fadeInTime);
        yield return new WaitForSeconds(timeBeforeBackToMenu);
        SceneManager.LoadScene("menu");
    }
}
