using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventSystemMenu : MonoBehaviour
{

    public float timeBeforeShowingText;
    public float timeBeforeLoadingNextScene;

    public FadePanel fadePanel;
    public Text nextScreenText;

    public Button easyMode;
    public Button hardMode;

    public RectTransform buttonsPanel;

    [Header("Main menu")]
    public Image bg;
    public Button startButton;
    public Button optionsButton;
    public Button howToPlayButton;

    public Button exitButton;

    [Header("How to play")]
    public Text descHowToPlayPage;
    public Text pageNumbrHowToPlay;
    public Button nextHowToPlayButton;
    public Button prevHowToPlayButton;
    public Button backHowToPlayButton;
    public GameObject howToPlayPanel;

    public Material howToPlayAnimMaterial;
    public Animator howToPlayAnimator;

    public List<string> howToDescList;
    public List<string> howToBoolProperties;

    [Header("Options")]
    public Button musicButton;
    public Button soundButton;

    public Slider musicSlider;
    public Slider soundSlider;

    public Button backButton;

    public GameObject musicContainer;
    public GameObject soundContainer;

    public Text musicPercText;
    public Text effectsPecText;

    public RectTransform fillMusicSlider;
    public RectTransform fillEffectsSlider;

    AudioSource audioSource;

    GameObject selectedObj;

    int indexHowToPlayPage = 0;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        selectedObj = EventSystem.current.currentSelectedGameObject;
        StartCoroutine(StartAnimations());
        MyButton bt = startButton.GetComponent<MyButton>();
        bt.ignoreSelection = true;
        startButton.Select();
        bt.ignoreSelection = false;
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = GameInfo.instance.music;
        musicSlider.value = 100f * GameInfo.instance.music;

        UpdateSliderValue(0f, musicSlider, fillMusicSlider, musicPercText);
        UpdateSliderValue(0f, soundSlider, fillEffectsSlider, effectsPecText);

        StartCoroutine(Blink());
    }

    void Update()
    {

        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(selectedObj);

        selectedObj = EventSystem.current.currentSelectedGameObject;

        float h = Input.GetAxis("Horizontal");

        if (h != 0f)
        {
            GameObject go = EventSystem.current.currentSelectedGameObject;
            if (go &&
                (go.GetInstanceID() == musicButton.gameObject.GetInstanceID() ||
                go.GetInstanceID() == soundButton.gameObject.GetInstanceID()))
            {
                if (go.GetInstanceID() == musicButton.gameObject.GetInstanceID())
                {
                    UpdateSliderValue(
                        Mathf.Sign(h) * 0.5f,
                        musicSlider,
                        fillMusicSlider,
                        musicPercText);

                    audioSource.volume = musicSlider.value / 100f;
                }
                else
                {
                    UpdateSliderValue(
                        Mathf.Sign(h) * 0.5f,
                        soundSlider,
                        fillEffectsSlider,
                        effectsPecText);
                }
            }
        }
    }

    void UpdateSliderValue(float value, Slider slider, RectTransform fill, Text percText)
    {
        slider.value += value;
        if (slider == musicSlider)
        {
            GameInfo.instance.music = slider.value / 100f;
        }
        else
        {
            GameInfo.instance.effects = slider.value / 100f;
        }
        percText.rectTransform.anchoredPosition = new Vector3(1400 - 420f * (1f - slider.value / slider.maxValue),
                        percText.rectTransform.anchoredPosition.y);

        percText.text = ((int)slider.value).ToString() + "%";
    }

    public void ShowOptions()
    {
        bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0.3f);

        musicContainer.gameObject.SetActive(true);
        soundContainer.gameObject.SetActive(true);

        backButton.gameObject.SetActive(true);

        startButton.gameObject.SetActive(false);
        optionsButton.gameObject.SetActive(false);
        howToPlayButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);

        MyButton bt = backButton.GetComponent<MyButton>();
        bt.ignoreSelection = true;
        backButton.Select();
        bt.ignoreSelection = false;
    }

    public void LoadHowToPlay()
    {
        bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0.3f);
        HowToPlay(0);
        MyButton bt = nextHowToPlayButton.GetComponent<MyButton>();
        bt.ignoreSelection = true;
        nextHowToPlayButton.Select();
        bt.ignoreSelection = false;
    }

    public void HowToPlay(int index)
    {
        pageNumbrHowToPlay.text = (index + 1).ToString() + "/" + howToDescList.Count.ToString();

        indexHowToPlayPage = index;
        howToPlayPanel.gameObject.SetActive(true);

        startButton.gameObject.SetActive(false);
        optionsButton.gameObject.SetActive(false);
        howToPlayButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);

        descHowToPlayPage.text = howToDescList[index];

        for (int i = 0; i < howToBoolProperties.Count; i++)
        {
            howToPlayAnimator.SetBool(howToBoolProperties[i], i == index);
        }
    }

    bool blink;
    IEnumerator Blink()
    {
        while (true)
        {
            blink = !blink;
            howToPlayAnimMaterial.SetFloat("_alpha", blink ? 1f : 0f);
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void NextHowToPage()
    {
        HowToPlay((indexHowToPlayPage + 1) % howToDescList.Count);
        MyButton bt = nextHowToPlayButton.GetComponent<MyButton>();
        bt.ignoreSelection = true;
        nextHowToPlayButton.Select();
        bt.ignoreSelection = false;
    }

    public void PrevHowToPage()
    {
        if (indexHowToPlayPage == 0)
        {
            HowToPlay(howToDescList.Count - 1);
        }
        else
        {
            HowToPlay(indexHowToPlayPage - 1);
        }

        MyButton bt = prevHowToPlayButton.GetComponent<MyButton>();
        bt.ignoreSelection = true;
        prevHowToPlayButton.Select();
        bt.ignoreSelection = false;
    }
    void ResetHowToPlayIndex()
    {
        indexHowToPlayPage = 0;
    }

    public void BackToMainMenu()
    {
        bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 1f);

        for (int i = 0; i < howToBoolProperties.Count; i++)
        {
            howToPlayAnimator.SetBool(howToBoolProperties[i], false);
        }

        howToPlayPanel.gameObject.SetActive(false);
        ResetHowToPlayIndex();

        musicContainer.gameObject.SetActive(false);
        soundContainer.gameObject.SetActive(false);

        backButton.gameObject.SetActive(false);

        startButton.gameObject.SetActive(true);
        optionsButton.gameObject.SetActive(true);
        howToPlayButton.gameObject.SetActive(true);

        MyButton bt = startButton.GetComponent<MyButton>();
        bt.ignoreSelection = true;
        startButton.Select();
        bt.ignoreSelection = false;
    }

    public void StartGame(bool heroMode)
    {

        GameInfo.instance.heroMode = heroMode;

        easyMode.gameObject.SetActive(false);
        hardMode.gameObject.SetActive(false);

        StartCoroutine(LoadFirstScene());
    }

    public AudioClip sample1;
    public AudioClip sample2;
    public AudioClip sample3;
    public AudioClip sample4;
    public AudioClip sample5;

    public void TestEffectsLevel()
    {
        AudioClip sample;

        switch (Random.Range(0, 5))
        {
            case 0:
                sample = sample1;
                break;
            case 1:
                sample = sample2;
                break;
            case 2:
                sample = sample3;
                break;
            case 3:
                sample = sample4;
                break;
            default:
                sample = sample5;
                break;
        }

        AudioSource.PlayClipAtPoint(sample, Camera.main.transform.position, GameInfo.instance.effects);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ShowDifficultyButtons()
    {
        startButton.gameObject.SetActive(false);
        optionsButton.gameObject.SetActive(false);
        howToPlayButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        StartGame(true);

        return;
    }

    IEnumerator StartAnimations()
    {
        yield return new WaitForSeconds(fadePanel.fadeInTime);
        yield return new WaitForSeconds(timeBeforeShowingText);

        nextScreenText.gameObject.SetActive(true);
        // StartCoroutine(FadeIn(nextScreenText, 1f, 2f));
    }

    IEnumerator LoadFirstScene()
    {
        fadePanel.Show(true);
        yield return new WaitForSeconds(fadePanel.fadeOutTime);
        yield return new WaitForSeconds(timeBeforeLoadingNextScene);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator FadeIn(Text text, float aValue, float aTime)
    {
        float alpha = text.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(alpha, aValue, t));
            text.color = newColor;
            yield return null;
        }
    }
}
