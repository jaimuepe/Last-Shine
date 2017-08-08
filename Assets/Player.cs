using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class Player : Character
{

    public bool startInTheSkies = true;

    public LayerMask enemyMask;
    public AudioClip fallFromSkyClip;

    public int respawnAreaDamage;
    CameraBehaviour cameraBehaviour;

    bool firstSpawn = true;

    public GameObject moveToOnSpawn;

    Coroutine fallFromTheSkyCoroutine;

    public bool deferInitialization = false;

    protected override void Start()
    {
        base.Start();
        EnableRandomCough();

        currentHp = 0;
        sr.enabled = false;
        cameraBehaviour = FindObjectOfType<CameraBehaviour>();
        damageKnockbackThreshold = maxHp / 3;
        uiManager.UpdatePlayerHp(currentHp, currentHp, maxHp);

        if (startInTheSkies)
        {
            Respawn();
        }
        else
        {
            firstSpawn = false;
            sr.enabled = true;
            if (deferInitialization && firstSpawn)
            {
                return;
            }

            Initialize();
            MoveTo(moveToOnSpawn.transform.position);
        }
        deferInitialization = false;
    }

    public void Initialize()
    {
        currentHp = maxHp;
        uiManager.UpdatePlayerHp(currentHp, currentHp, maxHp);
        movementEnabled = true;
        characterController.actionsDisabled = false;
    }

    protected override void Update()
    {
        base.Update();

        if (!movementEnabled)
        {
            bool reached = characterController.MoveToPoint(animateToPoint);
            if (reached)
            {
                movementEnabled = true;
                ignoreCollisions = false;
                uiManager.HideWaitMessage();
            }
        }
    }

    public override bool Damage(int ammount, bool knockBack, bool comingFromRight, bool forceDramaticMode)
    {
        bool damaged = base.Damage(ammount, knockBack, comingFromRight, forceDramaticMode);
        if (damaged)
        {
            // Stupid chickens
            float previousValue;
            if (ammount < 0)
            {
                previousValue = Mathf.Min(maxHp, currentHp + ammount);
            }
            else
            {
                previousValue = Mathf.Max(0f, currentHp + ammount);
            }
            uiManager.UpdatePlayerHp(currentHp, previousValue, maxHp);
        }
        return damaged;
    }

    protected override void AfterDeathAnimation()
    {
        sr.enabled = false;
        shadow.GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(ShowContinueMenu());
    }

    public void Respawn()
    {
        characterController.actionsDisabled = true;
        characterController.groundCheck.enabled = false;
        cameraBehaviour.StopFollowingTarget();
        shadow.GetComponent<SpriteRenderer>().enabled = true;
        uiManager.RestorePlayerPortraitInmediately();

        if (fallFromTheSkyCoroutine != null)
        {
            StopCoroutine(fallFromTheSkyCoroutine);
        }

        fallFromTheSkyCoroutine = StartCoroutine(FallFromTheSky());
    }

    void Ready()
    {
        AudioSource.PlayClipAtPoint(fallFromSkyClip, Camera.main.transform.position, GameInfo.instance.effects);
        if (firstSpawn)
        {
            firstSpawn = false;
        }
        else
        {
            cameraBehaviour.FollowTarget();
        }

        if (!deferInitialization)
        {
            currentHp = maxHp;
            uiManager.UpdatePlayerHp(currentHp, 0, maxHp);
            characterController.actionsDisabled = false;
            characterController.groundCheck.enabled = true;
        }

        Camera camera = Camera.main;
        float aspectRatio = camera.aspect;
        float height = 1.5f * camera.orthographicSize;
        float width = height * aspectRatio;

        Collider2D[] enemiesInRange = Physics2D.OverlapBoxAll(transform.position,
            new Vector2(width, height), 0f, enemyMask);

        foreach (Collider2D enemy in enemiesInRange)
        {
            Enemy e = enemy.GetComponent<Enemy>();
            if (e.ignoreCollisions)
            {
                continue;
            }
            bool comingFromRight = enemy.transform.position.x < transform.position.x;
            e.Damage(respawnAreaDamage, true, comingFromRight, false);
        }
    }

    IEnumerator FallFromTheSky()
    {
        float targetPosY = transform.position.y;

        float outOfScreenY = transform.position.y + 1200;
        float posY = outOfScreenY;

        yield return new WaitForSeconds(1.5f);
        animator.SetBool("dead", false);
        animator.SetBool("knockedback", false);

        sr.enabled = true;

        while (posY > targetPosY)
        {
            posY -= 30f;
            if (posY < targetPosY)
            {
                break;
            }
            transform.position = new Vector3(transform.position.x, posY,
                transform.position.z);

            shadow.SetExtraHeight(posY - targetPosY);
            characterController.SetExtraJumpHeight(posY - targetPosY);
            yield return null;
        }

        transform.position = new Vector3(
            transform.position.x,
            targetPosY,
            transform.position.z);

        shadow.SetExtraHeight(0f);
        characterController.SetExtraJumpHeight(0f);
        Ready();
    }

    IEnumerator ShowContinueMenu()
    {
        yield return new WaitForSeconds(1f);
        uiManager.ShowContinueScreen();
    }
}
