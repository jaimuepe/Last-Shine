using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{

    public SceneChanger sceneChanger;

    public AudioClip evilLaugh;
    public AudioSource bgMusic;
    public AudioClip song;

    public InfiniteSpawner spawner;

    protected override void Start()
    {
        base.Start();
        bgMusic = FindObjectOfType<AudioSource>();
        if (bgMusic != null)
        {
            bgMusic.clip = song;
            bgMusic.volume = GameInfo.instance.music;
            bgMusic.Play();
        }
        active = false;
        ignoreCollisions = true;
        IgnorePlayer();
        StartCoroutine(LaughCoroutine());
    }

    protected override void Chase()
    {
    }

    IEnumerator LaughCoroutine()
    {
        yield return new WaitForSeconds(4f);
        animator.SetBool("evil_laugh", true);
        AudioSource.PlayClipAtPoint(evilLaugh, Camera.main.transform.position, GameInfo.instance.effects);
        yield return new WaitForSeconds(2.5f);

        yield return new WaitForSeconds(1.0f);
        animator.SetBool("evil_laugh", false);

        player.Initialize();
        active = true;
        ignoreCollisions = false;
        movementEnabled = true;
        characterController.actionsDisabled = false;
        EnableRandomCough();
        player.EnableRandomCough();
        spawner.gameObject.SetActive(true);
    }


    protected override void ActionsBeforeKill()
    {
        DisableRandomCough();
        player.DisableRandomCough();
        spawner.gameObject.SetActive(false);

        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            if (enemy != this)
            {
                bool comingFromRight = Camera.main.WorldToViewportPoint(enemy.transform.position).x < 0.5;
                enemy.Damage(100000, true, comingFromRight, true);
            }
        }
    }

    protected override void Kill()
    {
        if (bgMusic != null)
        {
            bgMusic.Stop();
        }
        player.unableToMove = true;
        StartCoroutine(EndGame());
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2f);
        animator.SetBool("knockedback", false);
        animator.SetBool("dead", false);

        yield return new WaitForSeconds(1f);

        forceCough = true;
        player.forceCough = true;

        yield return new WaitForSeconds(2f);

        forceCough = true;
        player.forceCough = true;

        yield return new WaitForSeconds(1f);

        sceneChanger.gameObject.SetActive(true);
        sceneChanger.Change();
    }
}
