using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public float arrowDelay;

    public int score = 0;
    public Text scoreText;

    public RectTransform playerTotalHpBar;
    public RectTransform playerHpBar;

    public RectTransform currentChargeBar;
    public RectTransform totalChargeBar;

    public Text playerHpIndicator;

    [Header("Enemy info")]

    public RectTransform currentEnemyHpBar1;
    public RectTransform currentEnemtyTotalHpBar1;

    public RectTransform currentEnemyHpBar2;
    public RectTransform currentEnemtyTotalHpBar2;

    public RectTransform currentEnemyHpBar3;
    public RectTransform currentEnemtyTotalHpBar3;

    public Text enemyHpIndicator;


    [Header("Portraits")]
    public Material bwMaterialEnemyPortrait;

    public Image cross;
    public Image enemyPortrait;
    public GameObject enemyPortraitBg;

    public Image heroPortrait;
    public Sprite defaultHeroSprite;
    public Sprite damagedHeroSprite;
    public Sprite badlyWoundedHeroSprite;

    [Header("Others")]
    public GameObject continuePanel;

    public Text waitMessage;

    public GameObject arrow;

    Material enemyPortraitDefaultMaterial;

    Enemy currentlyFocusedEnemy;

    Coroutine showArrowCoroutine;
    Coroutine updateEnemyBar1Coroutine;
    Coroutine updateEnemyBar2Coroutine;
    Coroutine updateEnemyBar3Coroutine;
    Coroutine restoreHeroPortraitCoroutine;
    Coroutine restoreEnemyPortraitCoroutine;

    void Start()
    {
        enemyPortraitDefaultMaterial = enemyPortrait.material;

        currentEnemtyTotalHpBar1.gameObject.SetActive(false);
        currentEnemtyTotalHpBar2.gameObject.SetActive(false);
        currentEnemtyTotalHpBar3.gameObject.SetActive(false);

        enemyPortraitBg.gameObject.SetActive(false);
        score = GameInfo.instance.score;
        UpdateScore(score);
    }

    public void UpdatePlayerHp(float currentHp, float previousHp, float totalHp)
    {
        playerHpBar.gameObject.SetActive(true);
        StartCoroutine(AnimateBarChange(playerHpBar, playerTotalHpBar, currentHp, previousHp, totalHp, true, true, false));
        playerHpIndicator.text = (int)currentHp + "/" + (int)totalHp;
    }

    public void UpdateCharge(float currentCharge, float previousCharge, float maxCharge)
    {
        StartCoroutine(AnimateBarChange(currentChargeBar, totalChargeBar, currentCharge, previousCharge, maxCharge, false, false, false));
    }

    public void EnemyKilled(Enemy enemy)
    {
        if (currentlyFocusedEnemy != null && currentlyFocusedEnemy == enemy)
        {
            currentEnemtyTotalHpBar1.gameObject.SetActive(false);
            currentEnemtyTotalHpBar2.gameObject.SetActive(false);
            enemyPortraitBg.gameObject.SetActive(false);
        }

        UpdateScore(enemy.score);
    }

    public void UpdateScore(int ammount)
    {
        score += ammount;
        scoreText.text = score.ToString();
    }

    public void UpdateEnemyHp(Enemy currentlyFocusedEnemy, float currentHp, float previousHp, float totalHp)
    {

        if (restoreEnemyPortraitCoroutine != null)
        {
            StopCoroutine(restoreEnemyPortraitCoroutine);
        }

        bool isBoss = currentlyFocusedEnemy.GetType() == typeof(Boss);

        enemyPortrait.sprite = currentlyFocusedEnemy.defaultSprite;

        bwMaterialEnemyPortrait.SetFloat("_alpha", 1f);
        enemyPortrait.material = enemyPortraitDefaultMaterial;
        cross.gameObject.SetActive(false);

        enemyPortraitBg.gameObject.SetActive(true);
        this.currentlyFocusedEnemy = currentlyFocusedEnemy;

        int newBarIdx = (int)Mathf.Ceil(currentHp / 100);
        int prevBarIdx = (int)Mathf.Ceil(previousHp / 100);

        currentEnemtyTotalHpBar1.gameObject.SetActive(false);
        currentEnemtyTotalHpBar2.gameObject.SetActive(false);
        currentEnemtyTotalHpBar3.gameObject.SetActive(false);

        if (updateEnemyBar1Coroutine != null)
        {
            StopCoroutine(updateEnemyBar1Coroutine);
        }

        if (updateEnemyBar2Coroutine != null)
        {
            StopCoroutine(updateEnemyBar2Coroutine);
        }

        if (updateEnemyBar3Coroutine != null)
        {
            StopCoroutine(updateEnemyBar3Coroutine);
        }

        if (newBarIdx != prevBarIdx && newBarIdx != 0)
        {
            if (newBarIdx == 1)
            {
                currentEnemtyTotalHpBar1.gameObject.SetActive(true);
                currentEnemtyTotalHpBar2.gameObject.SetActive(true);

                // Both
                StartCoroutine(AnimateBothBars(
                    currentEnemyHpBar1,
                    currentEnemtyTotalHpBar1,
                    currentEnemyHpBar2,
                    currentEnemtyTotalHpBar2,
                    currentHp,
                    previousHp,
                    false,
                    isBoss));
            }
            else if (newBarIdx == 2)
            {
                currentEnemtyTotalHpBar2.gameObject.SetActive(true);
                currentEnemtyTotalHpBar3.gameObject.SetActive(true);

                StartCoroutine(AnimateBothBars(
                    currentEnemyHpBar2,
                    currentEnemtyTotalHpBar2,
                    currentEnemyHpBar3,
                    currentEnemtyTotalHpBar3,
                    currentHp,
                    previousHp,
                    false,
                    isBoss));
            }
        }
        else if (newBarIdx == 1 || newBarIdx == 0)
        {

            // lower bar
            currentEnemtyTotalHpBar1.gameObject.SetActive(true);

            // currentEnemyHpBar2.sizeDelta = new Vector2(
            //    0f,
            //    currentEnemyHpBar2.sizeDelta.y);

            updateEnemyBar1Coroutine = StartCoroutine(AnimateBarChange(
                currentEnemyHpBar1,
                currentEnemtyTotalHpBar1,
                currentHp,
                previousHp,
                100f,
                true,
                false,
                isBoss));
        }
        else if (newBarIdx == 2)
        {
            // upper bar

            currentEnemtyTotalHpBar2.gameObject.SetActive(true);

            // currentEnemyHpBar2.sizeDelta = new Vector2(
            // 100f,
            // currentEnemyHpBar2.sizeDelta.y);

            updateEnemyBar2Coroutine = StartCoroutine(AnimateBarChange(
                currentEnemyHpBar2,
                currentEnemtyTotalHpBar2,
                currentHp % 100,
                previousHp - 100,
                100f,
                true,
                false,
                isBoss));
        }
        else
        {
            currentEnemtyTotalHpBar3.gameObject.SetActive(true);

            // currentEnemyHpBar3.sizeDelta = new Vector2(
            // 100f,
            // currentEnemyHpBar3.sizeDelta.y);

            updateEnemyBar3Coroutine = StartCoroutine(AnimateBarChange(
                currentEnemyHpBar3,
                currentEnemtyTotalHpBar3,
                currentHp % 100,
                previousHp - 200,
                100f,
                true,
                false,
                isBoss));
        }

        enemyHpIndicator.text = (int)currentHp + "/" + (int)totalHp;

        if (currentHp == 0)
        {
            enemyPortrait.material = bwMaterialEnemyPortrait;
            bwMaterialEnemyPortrait.SetFloat("_alpha", 0f);
            cross.gameObject.SetActive(true);
        }
    }

    public void ShowWaitMessage()
    {
        // waitMessage.gameObject.SetActive(true);
    }

    public void HideWaitMessage()
    {
        waitMessage.gameObject.SetActive(false);
    }

    public void ShowArrow()
    {
        if (showArrowCoroutine != null)
        {
            StopCoroutine(showArrowCoroutine);
        }
        showArrowCoroutine = StartCoroutine(ShowArrowAfterDelay());
    }

    public void HideArrow()
    {
        if (showArrowCoroutine != null)
        {
            StopCoroutine(showArrowCoroutine);
        }
        arrow.SetActive(false);
    }

    IEnumerator AnimateBothBars(
        RectTransform bar1,
        RectTransform backgroundBar1,
        RectTransform bar2,
        RectTransform backgroundBar2,
        float current1,
        float previousValue,
        bool player,
        bool isBoss)
    {

        enemyPortraitBg.gameObject.SetActive(true);

        float previous2 = previousValue - 100;

        if (previousValue >= 200)
        {
            current1 = current1 - 100;
            previous2 = previousValue - 200;
        }
        else
        {
            previous2 = previousValue - 100;
        }

        StartCoroutine(AnimateBarChange(bar2, backgroundBar2, 0f, previous2, 100, true, player, isBoss));
        yield return new WaitForSeconds(0.1f);
        backgroundBar2.gameObject.SetActive(false);
        StartCoroutine(AnimateBarChange(bar1, backgroundBar1, current1, 100f, 100, true, player, isBoss));
    }

    IEnumerator AnimateBarChange(
        RectTransform bar,
        RectTransform backgroundBar,
        float current,
        float previous,
        float maxValue,
        bool isHpBar,
        bool player,
        bool boss)
    {

        float aTime = 0.1f;
        float oldValue = backgroundBar.sizeDelta.x * previous / maxValue;

        float targetValue = backgroundBar.sizeDelta.x * current / maxValue;

        if (isHpBar && targetValue < oldValue)
        {
            // TODO shake anim.

            if (player)
            {
                if (restoreHeroPortraitCoroutine != null)
                {
                    StopCoroutine(restoreHeroPortraitCoroutine);
                }
                if (current <= 0f)
                {
                    heroPortrait.sprite = badlyWoundedHeroSprite;
                }
                else
                {
                    heroPortrait.sprite = damagedHeroSprite;
                    restoreHeroPortraitCoroutine = StartCoroutine(RestoreHeroPortrait());
                }
            }
            else
            {
                if (restoreEnemyPortraitCoroutine != null)
                {
                    StopCoroutine(restoreEnemyPortraitCoroutine);
                }

                if (currentlyFocusedEnemy.IsDead())
                {
                    enemyPortrait.sprite = currentlyFocusedEnemy.badlyWoundedSprite;
                }
                else
                {
                    enemyPortrait.sprite = currentlyFocusedEnemy.damagedSprite;
                    restoreEnemyPortraitCoroutine = StartCoroutine(RestoreEnemyPortrait());
                }
            }
        }

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            float newValue = Mathf.Lerp(oldValue, targetValue, t);
            bar.sizeDelta = new Vector2(
                newValue,
                bar.sizeDelta.y);

            yield return null;
        }

        bar.sizeDelta = new Vector2(
                targetValue,
                bar.sizeDelta.y);
    }

    public void RestorePlayerPortraitInmediately()
    {
        heroPortrait.sprite = defaultHeroSprite;
    }

    IEnumerator RestoreEnemyPortrait()
    {
        yield return new WaitForSeconds(0.5f);
        if (currentlyFocusedEnemy != null)
        {
            enemyPortrait.sprite = currentlyFocusedEnemy.defaultSprite;
        }
    }

    IEnumerator RestoreHeroPortrait()
    {
        yield return new WaitForSeconds(0.5f);
        RestorePlayerPortraitInmediately();
    }

    IEnumerator ShowArrowAfterDelay()
    {
        yield return new WaitForSeconds(arrowDelay);
        arrow.SetActive(true);
    }

    IEnumerator DisableEnemyHPBar()
    {
        yield return new WaitForSeconds(2f);
        currentEnemtyTotalHpBar1.gameObject.SetActive(false);
        currentEnemtyTotalHpBar2.gameObject.SetActive(false);
        currentEnemtyTotalHpBar3.gameObject.SetActive(false);
        enemyPortraitBg.gameObject.SetActive(false);
    }

    public void ShowContinueScreen()
    {
        continuePanel.SetActive(true);
    }
}
