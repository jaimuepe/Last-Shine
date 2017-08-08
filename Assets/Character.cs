using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public abstract class Character : MonoBehaviour
{
    public float timeEmptyCharge;
    public float maxCharge;

    public bool ignoreCollisions = false;
    public float staggerTime;

    public string portraitName;

    public Material chargedAttackMaterial;
    public Material blinkMaterial;
    public Material OnHitMaterial;

    public int maxHp;

    public float killTime = 3f;

    [SerializeField]
    bool canBeKnockedback;

    Material defaultMaterial;

    protected bool movementEnabled = true;
    public int currentHp;
    protected int damageKnockbackThreshold = 10000;
    int accumulatedDamageSinceLastKnockback = 0;

    Coroutine CoroutineResetKnockbackTimer;
    Coroutine emptyChargeCoroutine;
    Coroutine feelBadCoroutine;

    public bool charging = false;

    protected Vector3 animateToPoint;
    protected UIManager uiManager;
    protected SpriteRenderer sr;
    protected Animator animator;
    protected Shadow shadow;
    protected SpriteRenderer shadowSr;
    protected CharacterController2D characterController;
    protected BoxCollider2D boxCollider;

    public GameObject hit;
    public GameObject fistplosion;

    protected virtual void Start()
    {
        currentHp = maxHp;
        sr = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        defaultMaterial = sr.material;
        animator = GetComponent<Animator>();
        shadow = GetComponentInChildren<Shadow>();
        shadowSr = shadow.GetComponent<SpriteRenderer>();
        characterController = GetComponent<CharacterController2D>();
        uiManager = FindObjectOfType<UIManager>();
    }

    float chargeStateChangeTime = 0.2f;
    float chargeStateCount = 0f;

    bool chargeAttackState;

    protected virtual void Update()
    {
        if (charging && charge >= maxCharge)
        {
            chargeStateCount += Time.deltaTime;
            if (chargeStateCount > chargeStateChangeTime)
            {
                chargeStateCount = 0f;
                sr.material = chargedAttackMaterial;
                shadowSr.material = chargedAttackMaterial;
                chargedAttackMaterial.SetFloat("_alpha", chargeAttackState ? 1f : 0f);
                chargeAttackState = !chargeAttackState;
            }
        }

        float y = transform.position.y - boxCollider.size.y / 2 - characterController.GetExtraJumpHeight();

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            y / 100);


        shadow.SetExtraHeight(characterController.GetExtraJumpHeight());

        if (!feelingBad && forceCough)
        {
            forceCough = false;
            FeelBad();
        }
    }

    public bool IsCharging()
    {
        return charging;
    }

    public bool IsMovementEnabled()
    {
        return movementEnabled;
    }

    public bool IsFullHealth()
    {
        return maxHp == currentHp;
    }

    public void MoveHorizontally(bool moveRight)
    {
        characterController.MoveHorizontally(moveRight);
    }

    public void MoveVertically(bool moveUp)
    {
        characterController.MoveVertically(moveUp);
    }

    public void Attack()
    {
        characterController.Attack();
    }

    public void Jump()
    {
        characterController.Jump();
    }

    Coroutine randomCoughCoroutine;

    bool randomCough = true;

    public void DisableRandomCough()
    {
        randomCough = false;
        if (randomCoughCoroutine != null)
        {
            StopCoroutine(randomCoughCoroutine);
        }
    }
    public void EnableRandomCough()
    {
        if (randomCoughCoroutine != null)
        {
            StopCoroutine(randomCoughCoroutine);
        }
        randomCoughCoroutine = StartCoroutine(Cough());   
    }

    IEnumerator Cough()
    {
        while(randomCough)
        {
            yield return new WaitForSeconds(Random.Range(10f, 15f));
            forceCough = true;
        }
    }

    public void Block()
    {
        characterController.Block();
    }

    public void ExitBlock()
    {
        characterController.ExitBlock();
    }

    public bool IsDead()
    {
        return currentHp <= 0;
    }

    public bool IsBlocking()
    {
        return characterController.block;
    }

    public bool IsKnockedback()
    {
        return characterController.IsKnockedback();
    }

    public List<AudioClip> hitClips;

    public virtual bool Damage(int ammount, bool knockBack, bool comingFromRight, bool forceDramaticMode)
    {
        int hpAfterHit = currentHp - ammount;

        // If it's blocking and it's not a knockback it won't take any dmg
        if (IsBlocking() && !knockBack)
        {
            // He's looking left
            if (transform.localScale.x > 0 && !comingFromRight)
            {
                return false;
            }
            // He's looking right
            if (transform.localScale.x < 0 && comingFromRight)
            {
                return false;
            }
        }

        accumulatedDamageSinceLastKnockback += ammount;

        bool forceKnockback =
            accumulatedDamageSinceLastKnockback > damageKnockbackThreshold;

        bool successfulKnockback = false;

        bool isPlayer = GetType() == typeof(Player);
        bool isBoss = GetType() == typeof(Boss);

        // Some characters can't be knockedback
        if (canBeKnockedback &&
            (forceKnockback || knockBack || !characterController.IsGrounded()) || hpAfterHit <= 0)
        {
            // Apply knockback
            characterController.Knockback(comingFromRight, (isPlayer || isBoss || forceDramaticMode) && hpAfterHit <= 0);
            accumulatedDamageSinceLastKnockback = 0;
            successfulKnockback = true;

            if (isPlayer || isBoss)
            {
                StopFeelingBad();
            }
        }

        currentHp = Mathf.Clamp(hpAfterHit, 0, maxHp);

        StopCharging();

        if (ammount > 0)
        {
            Animator hitAnimator = hit.GetComponent<Animator>();

            if (!successfulKnockback)
            {
                animator.SetTrigger("hit");
            }
            hitAnimator.SetTrigger("hit");

            AudioSource.PlayClipAtPoint(
                hitClips[Random.Range(0, hitClips.Count)],
                Camera.main.transform.position,
                GameInfo.instance.effects);
        }

        if (IsDead())
        {
            StartCoroutine(KillCoroutine());
            return true;
        }
        else
        {

            // If they can't get knockedback they won't stagger
            if (canBeKnockedback)
            {
                ExitBlock();
                characterController.HandleActionsDelay(staggerTime);
            }
            if (CoroutineResetKnockbackTimer != null)
            {
                StopCoroutine(CoroutineResetKnockbackTimer);
            }

            CoroutineResetKnockbackTimer = StartCoroutine(ResetDamageKnockbackTimer());
        }

        return true;
    }

    float charge = 0f;

    public void IncreaseChargeAttack()
    {

        if (!characterController.IsGrounded())
        {
            StopCharging();
            return;
        }

        if (!charging)
        {
            if (!characterController.CanDoActions())
            {
                StopCharging();
                return;
            }
            animator.SetBool("walk", false);
            animator.SetBool("charging", true);
        }


        if (emptyChargeCoroutine != null)
        {
            StopCoroutine(emptyChargeCoroutine);
        }

        emptyChargeCoroutine = StartCoroutine(DrainCharge());

        charging = true;

        float previousCharge = charge;

        charge = Mathf.Min(maxCharge, charge + 0.02f);

        if (GetType() == typeof(Player))
        {
            uiManager.UpdateCharge(charge, previousCharge, maxCharge);
        }
    }


    public bool feelingBad = false;
    public bool forceCough;

    public bool IsFeelingBad()
    {
        return feelingBad;
    }

    public void StopFeelingBad()
    {
        if (feelBadCoroutine != null)
        {
            StopCoroutine(feelBadCoroutine);
        }
        animator.SetBool("feelingbad", false);
        feelingBad = false;
        characterController.actionsDisabled = false;
    }

    public AudioClip cough;

    public void FeelBad()
    {
        ExitBlock();
        StopCharging();
        animator.SetBool("walk", false);
        feelingBad = true;
        characterController.actionsDisabled = true;
        if (feelBadCoroutine != null)
        {
            StopCoroutine(feelBadCoroutine);
        }
        AudioSource.PlayClipAtPoint(cough, Camera.main.transform.position, GameInfo.instance.effects);
        feelBadCoroutine = StartCoroutine(PlayCoughAnimation());
    }

    IEnumerator PlayCoughAnimation()
    {
        while (!characterController.IsGrounded())
        {
            yield return null;
        }

        animator.SetBool("feelingbad", true);
        yield return new WaitForSeconds(1f);
        feelingBad = false;

        animator.SetBool("feelingbad", false);
        characterController.actionsDisabled = false;
    }

    public AudioClip fistplosionClip;

    public void AttackButtonReleased()
    {
        if (charging && charge >= maxCharge)
        {
            animator.SetTrigger("charged_attack");

            fistplosion.SetActive(true);
            StartCoroutine(DeactivateFistPlosionCollider());

            Animator animFist = fistplosion.GetComponent<Animator>();
            animFist.SetTrigger("fistplosion");
            AudioSource.PlayClipAtPoint(fistplosionClip, Camera.main.transform.position, GameInfo.instance.effects);
            StartCoroutine(StopChargeAfter());
        }
        else
        {
            StopCharging();
        }
    }

    IEnumerator DeactivateFistPlosionCollider()
    {
        yield return new WaitForSeconds(0.5f);
        fistplosion.SetActive(false);
    }

    IEnumerator StopChargeAfter()
    {
        sr.material = defaultMaterial;
        shadowSr.material = defaultMaterial;
        chargedAttackMaterial.SetFloat("_alpha", 0f);

        float previousCharge = charge;
        charge = 0f;
        uiManager.UpdateCharge(charge, previousCharge, maxCharge);

        if (emptyChargeCoroutine != null)
        {
            StopCoroutine(emptyChargeCoroutine);
        }

        yield return new WaitForSeconds(1f);

        charging = false;
        animator.SetBool("charging", false);
    }

    public void StopCharging()
    {
        if (GetType() != typeof(Player))
        {
            return;
        }

        chargedAttackMaterial.SetFloat("_alpha", 0f);
        sr.material = defaultMaterial;
        shadowSr.material = defaultMaterial;

        fistplosion.gameObject.SetActive(false);

        animator.SetBool("charging", false);
        charging = false;

        float previousCharge = charge;
        charge = 0f;

        uiManager.UpdateCharge(charge, previousCharge, maxCharge);

        if (emptyChargeCoroutine != null)
        {
            StopCoroutine(emptyChargeCoroutine);
        }
    }

    public bool unableToMove = false;

    protected virtual void Kill()
    {
        StartCoroutine(Blink());
    }

    public void MoveTo(Vector3 point)
    {
        animateToPoint = point;
        movementEnabled = false;
        ignoreCollisions = true;

        if (GetType() == typeof(Player))
        {
            uiManager.ShowWaitMessage();
        }
    }

    protected abstract void AfterDeathAnimation();

    IEnumerator DrainCharge()
    {
        yield return new WaitForSeconds(timeEmptyCharge);
    }

    protected IEnumerator ResetDamageKnockbackTimer()
    {
        yield return new WaitForSeconds(1f);
        accumulatedDamageSinceLastKnockback = 0;
    }

    public AudioClip deathSound;

    protected virtual void ActionsBeforeKill()
    {

    }

    protected IEnumerator KillCoroutine()
    {
        ActionsBeforeKill();
        animator.SetBool("dead", true);
        if (GetType() == typeof(Player) || GetType() == typeof(Player))
        {
            AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, GameInfo.instance.effects);
        }

        while (!characterController.IsGrounded())
        {
            yield return null;
        }
        Kill();
    }

    protected IEnumerator Blink()
    {
        float waitTime = 0.3f;
        bool visible = true;

        sr.material = blinkMaterial;
        shadowSr.material = blinkMaterial;

        float t = 0f;

        while (t < killTime)
        {
            sr.material.SetFloat("_alpha", visible ? 1f : 0f);
            shadowSr.material.SetFloat("_alpha", visible ? 1f : 0f);

            visible = !visible;

            yield return new WaitForSeconds(waitTime);
            waitTime -= waitTime * 0.08f;
            t += (Time.deltaTime + waitTime);
        }

        sr.material = defaultMaterial;
        shadowSr.material = defaultMaterial;

        AfterDeathAnimation();
    }
}
