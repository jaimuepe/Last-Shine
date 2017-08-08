using System.Collections;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    public BoxCollider2D punch1;
    public BoxCollider2D punch2;
    public BoxCollider2D punch3;
    public BoxCollider2D airKick;
    public BoxCollider2D groundCheck;

    public AudioClip hitGroundClip1;
    public AudioClip hitGroundClip2;
    public AudioClip hitGroundClip3;

    public float speed;
    public LayerMask foodMask;
    public LayerMask collisionMask;

    public float delayAfterAttack = 0.3f;
    public float delayAfterLastComboAttack = 0.5f;

    public int maxComboHits = 3;
    public float timeBetweenComboHits = 0.5f;

    public float jumpSpeed = 0.05f;
    public float jumpDistance = 1f;

    public bool moonWalking = false;

    public float recoveryAfterKnockback;

    public float defaultSpeed;
    float extraJumpHeight = 0f;

    public bool actionsDisabled = false;

    bool moveRight;
    bool moveLeft;
    bool moveUp;
    bool moveDown;
    bool punch;
    bool jump;
    public bool block = false;

    bool knockedback = false;

    bool airPunched = false;
    bool isGrounded = true;

    int comboCount = 0;

    float boxWidth;
    float boxHeight;

    Animator animator;
    Character character;

    Coroutine comboCoroutine;
    Coroutine jumpCoroutine;

    UIManager uiManager;

    void Start()
    {
        defaultSpeed = speed;

        uiManager = FindObjectOfType<UIManager>();

        character = GetComponent<Character>();
        animator = GetComponent<Animator>();
        boxWidth = groundCheck.bounds.size.x;
        boxHeight = groundCheck.bounds.size.y;

        airKick.gameObject.SetActive(false);
        punch1.gameObject.SetActive(false);
        punch2.gameObject.SetActive(false);
        punch3.gameObject.SetActive(false);
    }

    public void Attack()
    {
        punch = true;
    }

    public void Jump()
    {
        jump = true;
    }

    public void Block()
    {
        if (IsGrounded() && CanDoActions())
        {
            block = true;
            animator.SetBool("block", true);
        }
    }

    public void ExitBlock()
    {
        block = false;
        animator.SetBool("block", false);
    }

    public void MoveHorizontally(bool moveRight)
    {
        this.moveRight = moveRight;
        moveLeft = !moveRight;
    }

    public void MoveVertically(bool moveUp)
    {
        this.moveUp = moveUp;
        moveDown = !moveUp;
    }

    public void Knockback(bool comingFromRight, bool slowmoEffect)
    {
        block = false;
        knockedback = true;
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
        }
        animator.SetBool("knockedback", true);

        StartCoroutine(KnockbackCoroutine(comingFromRight, slowmoEffect));
    }

    public bool IsKnockedback()
    {
        return knockedback;
    }

    public void SetExtraJumpHeight(float extraJumpHeight)
    {
        this.extraJumpHeight = extraJumpHeight;
    }

    public float GetExtraJumpHeight()
    {
        return extraJumpHeight;
    }

    public bool CanDoActions()
    {
        return !block
            && !character.unableToMove 
            && character.IsMovementEnabled()
            && !character.IsCharging()
            && !actionsDisabled
            && !knockedback;
    }

    void Update()
    {
        if (CanDoActions())
        {
            // Punches have priority except when mid-air
            if (isGrounded && punch)
            {
                Food food = SomethingToCollect();
                if (food)
                {
                    Collect(food);
                }
                else
                {
                    Punch();
                }
            }
            else
            {
                if (!airPunched && punch)
                {
                    airPunched = true;
                    airKick.gameObject.SetActive(true);
                    animator.SetBool("space_attack", true);
                    StartCoroutine(DeactivatePunchCollider(airKick, 0.2f));
                }

                if (moveUp || moveDown)
                {
                    ValidateVerticalMovement(moveUp);
                }
                if (moveLeft || moveRight)
                {
                    ValidateHorizontalMovement(moveRight);
                }
                if (moveLeft || moveRight || moveUp || moveDown)
                {
                    animator.SetBool("walk", true);
                }
                else
                {
                    animator.SetBool("walk", false);
                }
            }

            if (jump)
            {
                ValidateJump();
            }
        }

        punch = jump = moveUp = moveDown = moveLeft = moveRight = false;
    }

    void ValidateVerticalMovement(bool moveUp)
    {
        Vector2 origin = groundCheck.transform.position
            + new Vector3(groundCheck.offset.x, groundCheck.offset.y, 0f);

        origin.x -= boxWidth / 2;
        origin.y -= extraJumpHeight;

        Vector2 direction;

        float dy = (moveUp ? 1 : -1) * speed * Time.deltaTime;

        if (moveUp)
        {
            origin.y += boxHeight / 2;
            direction = Vector2.up;
        }
        else
        {
            origin.y -= boxHeight / 2;
            direction = Vector2.down;
        }

        bool collisionFound = false;

        if (!character.ignoreCollisions)
        {
            for (int i = 0; i < 3; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(origin, direction, Mathf.Abs(dy), collisionMask);
                if (hit)
                {
                    collisionFound = true;
                    break;
                }
                origin.x += boxWidth / 2;
            }
        }

        if (collisionFound)
        {

        }
        else
        {
            Vector3 position = transform.position;
            position.y += dy;
            transform.position = position;
        }
    }

    float epsilon = 50f;

    public bool MoveToPoint(Vector3 point)
    {
        Vector3 pos = transform.position + new Vector3(
            groundCheck.offset.x,
            groundCheck.offset.y,
            0f);

        float distanceX = Mathf.Abs(pos.x - point.x);
        float distanceY = Mathf.Abs(pos.y - point.y);

        if (distanceX > epsilon)
        {
            if (pos.x > point.x)
            {
                ValidateHorizontalMovement(false);
            }
            else
            {
                ValidateHorizontalMovement(true);
            }
        }

        if (distanceY > epsilon)
        {
            if (pos.y > point.y)
            {
                ValidateVerticalMovement(false);
            }
            else
            {
                ValidateVerticalMovement(true);
            }
        }

        animator.SetBool("walk", true);

        if (distanceX <= epsilon && distanceY <= epsilon)
        {
            return true;
        }
        return false;
    }

    void ValidateHorizontalMovement(bool moveRight)
    {
        Vector2 origin = groundCheck.transform.position
            + new Vector3(groundCheck.offset.x, groundCheck.offset.y, 0f);

        origin.y = origin.y - boxHeight / 2 - extraJumpHeight;

        Vector2 direction;

        float dx = (moveRight ? 1 : -1) * speed * Time.deltaTime;

        if (moveRight)
        {
            origin.x += boxWidth / 2;
            direction = Vector2.right;
        }
        else
        {
            origin.x -= boxWidth / 2;
            direction = Vector2.left;
        }

        bool collisionFound = false;

        if (!character.ignoreCollisions)
        {
            for (int i = 0; i < 3; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(origin, direction, Mathf.Abs(dx), collisionMask);
                if (hit)
                {
                    collisionFound = true;
                    break;
                }
                origin.y += boxHeight / 2;
            }
        }

        if (collisionFound)
        {
        }
        else
        {
            Vector3 position = transform.position;
            position.x += dx;
            transform.position = position;
        }

        int xScale;
        if (moveRight)
        {
            xScale = -1;
        }
        else
        {
            xScale = 1;
        }

        if (moonWalking || IsKnockedback())
        {
            xScale *= -1;
        }

        Vector3 scale = transform.localScale;
        scale.x = xScale * Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    public bool controlledFall = true;

    public void ValidateJump()
    {
        if (isGrounded)
        {
            isGrounded = false;
            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
            }
            animator.SetBool("jump", true);
            animator.SetBool("grounded", false);
            jumpCoroutine = StartCoroutine(JumpCounter(false));
        }
    }

    Food SomethingToCollect()
    {
        // Enemies can't pickup food (for now...)
        if (character.GetType() != typeof(Player))
        {
            return null;
        }

        Collider2D[] colliders = Physics2D.OverlapBoxAll(
           groundCheck.bounds.center,
           groundCheck.bounds.size,
           0f,
           foodMask);

        if (colliders.Length > 0)
        {
            Food f = colliders[0].GetComponent<Food>();
            if (f.readyToPickUp)
            {
                return f;
            }
        }
        return null;
    }

    void Collect(Food food)
    {
        int hp = food.hpAmmount;

        AudioSource.PlayClipAtPoint(food.biteClip, Camera.main.transform.position, GameInfo.instance.effects);
        Destroy(food.gameObject);

        if (character.IsFullHealth())
        {
            uiManager.UpdateScore(10 * hp);
            return;
        }

        character.Damage(-hp, false, false, false);
        StartCoroutine(ResetAttackDelay(delayAfterAttack));
    }

    void Punch()
    {
        comboCount = (comboCount + 1) % (maxComboHits + 1);

        punch1.gameObject.SetActive(false);
        punch2.gameObject.SetActive(false);
        punch3.gameObject.SetActive(false);

        if (comboCount == 1)
        {
            animator.SetTrigger("punch1");
            punch1.gameObject.SetActive(true);
            StartCoroutine(DeactivatePunchCollider(punch1, 0.2f));
        }
        else if (comboCount == 2)
        {
            animator.SetTrigger("punch2");
            punch2.gameObject.SetActive(true);
            StartCoroutine(DeactivatePunchCollider(punch2, 0.2f));
        }
        else if (comboCount == 3)
        {
            animator.SetTrigger("punch3");
            punch3.gameObject.SetActive(true);
            StartCoroutine(DeactivatePunchCollider(punch3, 0.2f));
        }

        if (comboCoroutine != null)
        {
            StopCoroutine(comboCoroutine);
        }

        if (comboCount == maxComboHits)
        {
            HandleActionsDelay(delayAfterLastComboAttack);
        }
        else
        {
            HandleActionsDelay(delayAfterAttack);
        }

        comboCoroutine = StartCoroutine(ComboCountCounter());
    }

    public void HandleActionsDelay(float delay)
    {
        actionsDisabled = true;
        StartCoroutine(ResetAttackDelay(delay));
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    IEnumerator DeactivatePunchCollider(BoxCollider2D punchCollider, float delay)
    {
        yield return new WaitForSeconds(delay);
        punchCollider.gameObject.SetActive(false);
    }

    IEnumerator ComboCountCounter()
    {
        yield return new WaitForSeconds(timeBetweenComboHits);
        comboCount = 0;
        animator.ResetTrigger("punch1");
        animator.ResetTrigger("punch2");
        animator.ResetTrigger("punch3");
    }

    IEnumerator ResetAttackDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        actionsDisabled = false;
    }

    float slowmoDelay = 0.04f;

    IEnumerator KnockbackCoroutine(bool comingFromRight, bool slowmoEffect)
    {
        isGrounded = false;
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
        }

        animator.ResetTrigger("hit");
        animator.SetBool("jump", false);
        animator.SetBool("grounded", false);

        jumpCoroutine = StartCoroutine(JumpCounter(slowmoEffect));

        speed = 2 * defaultSpeed;

        while (!isGrounded)
        {
            ValidateHorizontalMovement(!comingFromRight);
            if (slowmoEffect)
            {
                yield return new WaitForSeconds(slowmoDelay);
            }
            else
            {
                yield return null;
            }
        }

        AudioClip clip;
        int rnd = Random.Range(0, 2);
        if (rnd == 0)
        {
            clip = hitGroundClip1;
        }
        else
        {
            clip = hitGroundClip3;
        }

        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, GameInfo.instance.effects);

        speed = defaultSpeed;
        yield return new WaitForSeconds(recoveryAfterKnockback);

        if (!character.IsDead())
        {
            animator.SetBool("knockedback", false);
            animator.SetTrigger("standup");
        }
        knockedback = false;
    }

    IEnumerator JumpCounter(bool dramatic)
    {
        while (extraJumpHeight < jumpDistance)
        {
            extraJumpHeight += jumpSpeed;
            Vector3 position = transform.position;
            position.y += jumpSpeed;
            transform.position = position;
            if (dramatic)
            {
                yield return new WaitForSeconds(slowmoDelay);
            }
            else
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.1f);

        while (extraJumpHeight > 0 || !controlledFall)
        {
            if (controlledFall)
            {
                extraJumpHeight -= jumpSpeed;
            }
            Vector3 position = transform.position;
            position.y -= jumpSpeed;
            transform.position = position;
            if (dramatic)
            {
                yield return new WaitForSeconds(slowmoDelay);
            }
            else
            {
                yield return null;
            }
        }

        extraJumpHeight = 0f;

        isGrounded = true;
        airPunched = false;
        airKick.gameObject.SetActive(false);
        animator.SetBool("grounded", true);
        animator.SetBool("jump", false);
        animator.SetBool("space_attack", false);
    }
}
