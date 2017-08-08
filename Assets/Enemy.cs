using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyTypes
{
    BASIC_MELEE, BASIC_RANGED
}

public class Enemy : Character
{
    public int score = 100;
    public int type;
    public float swapStateTime;

    public Vector2 attackDistance;
    public Vector2 minDistanceToPlayer;

    public bool active = true;
    public float maxDistanceToPlayer;
    private float maxDistanceToPlayerLower;
    private float maxDistanceToPlayerUpper;

    [Header("GUI info")]
    public Sprite defaultSprite;
    public Sprite damagedSprite;
    public Sprite badlyWoundedSprite;

    protected Player player;

    private State state;

    private enum State
    {
        IDLE, ATTACKING, DEFENDING
    };

    Coroutine changeStateCoroutine;

    protected override void Start()
    {
        base.Start();
        state = Random.Range(0, 2) == 0 ? State.ATTACKING : State.DEFENDING;
        player = FindObjectOfType<Player>();

        changeStateCoroutine = StartCoroutine(ChangeState());

        maxDistanceToPlayerLower = maxDistanceToPlayer - maxDistanceToPlayer * 0.1f;
        maxDistanceToPlayerUpper = maxDistanceToPlayer + maxDistanceToPlayer + 0.1f;
    }

    private void OnEnable()
    {
        if (changeStateCoroutine != null)
        {
            StopCoroutine(changeStateCoroutine);
        }
        changeStateCoroutine = StartCoroutine(ChangeState());
    }

    protected virtual void Chase()
    {

        if (!movementEnabled)
        {
            bool reached = characterController.MoveToPoint(animateToPoint);
            if (reached)
            {
                movementEnabled = true;
                StartCoroutine(ChaseCoroutine());
            }
            return;
        }
    }

    protected override void Update()
    {
        base.Update();

        Chase();

        if (!active || IsDead() || IsFeelingBad())
        {
            return;
        }

        Vector3 playerPos = player.transform.position;
        Vector3 pos = transform.position;

        float distanceX = Mathf.Abs(playerPos.x - pos.x);
        float distanceY = Mathf.Abs(playerPos.y - pos.y);

        if (state == State.ATTACKING)
        {
            if (distanceX < attackDistance.x && distanceY < attackDistance.y)
            {
                StartCoroutine(AttackCoroutine());
            }
            else
            {
                if (distanceX > minDistanceToPlayer.x)
                {
                    if (playerPos.x > pos.x)
                    {
                        MoveHorizontally(true);
                    }
                    else
                    {
                        MoveHorizontally(false);
                    }
                }
                if (distanceY > minDistanceToPlayer.y)
                {
                    if (playerPos.y > pos.y)
                    {
                        MoveVertically(true);
                    }
                    else
                    {
                        MoveVertically(false);
                    }
                }
            }
        }
        else if (state == State.DEFENDING)
        {
            // If the player is too close the enemy will try to defend himself
            if (distanceX < attackDistance.x && distanceY < attackDistance.y)
            {
                characterController.Block();
            }
            else
            {
                characterController.ExitBlock();

                if (distanceX > maxDistanceToPlayerLower && distanceX < maxDistanceToPlayerUpper)
                {
                    // OK - in range
                }
                else
                {
                    if (playerPos.x > pos.x)
                    {
                        MoveHorizontally(false);
                    }
                    else
                    {
                        MoveHorizontally(true);
                    }
                }

                if (distanceY > maxDistanceToPlayerLower && distanceY < maxDistanceToPlayerUpper)
                {
                    // OK - in range
                }
                else
                {
                    if (playerPos.y > pos.y)
                    {
                        MoveVertically(false);
                    }
                    else
                    {
                        MoveVertically(true);
                    }
                }
            }
        }
    }

    public void IgnorePlayer()
    {
        active = false;
    }

    public void ChasePlayer()
    {
        active = true;
    }

    public override bool Damage(int ammount, bool knockBack, bool comingFromRight, bool forceDramaticMode)
    {
        bool damaged = base.Damage(ammount, knockBack, comingFromRight, forceDramaticMode);
        if (damaged)
        {
            uiManager.UpdateEnemyHp(this, currentHp, currentHp + ammount, maxHp);
        }

        return damaged;
    }

    protected override void AfterDeathAnimation()
    {
        uiManager.EnemyKilled(this);
        SendMessageUpwards("GoonDied", null, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }

    protected virtual void ActionsBeforeChase()
    {

    }
    IEnumerator ChaseCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(0.8f, 1.2f));
        ActionsBeforeChase();
        ignoreCollisions = false;
        ChasePlayer();
    }

    IEnumerator AttackCoroutine()
    {
        if (characterController.CanDoActions())
        {
            yield return new WaitForSeconds(0.3f);
            characterController.Attack();
        }
    }

    IEnumerator ChangeState()
    {
        while (true)
        {

            float factor;
            if (state == State.ATTACKING)
            {
                factor = 1.5f;
            }
            else
            {
                factor = 0.2f;
            }

            yield return new WaitForSeconds(Random.Range(
                swapStateTime * factor * 0.8f,
                swapStateTime * factor * 1.2f));

            State oldState = state;
            state = State.IDLE;

            yield return new WaitForSeconds(1f);

            if (oldState == State.ATTACKING)
            {
                characterController.speed = characterController.defaultSpeed / 4f;
                characterController.moonWalking = true;
                state = State.DEFENDING;
            }
            else
            {
                characterController.speed = characterController.defaultSpeed;
                characterController.moonWalking = false;
                characterController.ExitBlock();
                state = State.ATTACKING;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, attackDistance);
        Gizmos.DrawWireCube(transform.position, minDistanceToPlayer);
    }
}
