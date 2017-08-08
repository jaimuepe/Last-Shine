using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    float timeHoldingAttackButton;

    void Update()
    {

        if (player.IsDead())
        {
            return;
        }

        float h = Input.GetAxis("Horizontal");
        if (h != 0.0f)
        {
            if (h > 0.0f)
            {
                player.MoveHorizontally(true);
            }
            else
            {
                player.MoveHorizontally(false);
            }
        }

        float v = Input.GetAxis("Vertical");
        if (v != 0.0f)
        {
            if (v > 0.0f)
            {
                player.MoveVertically(true);
            }
            else
            {
                player.MoveVertically(false);
            }
        }

        if (Input.GetButtonDown("Attack"))
        {
            player.Attack();
        }

        if (Input.GetButton("Attack"))
        {
            timeHoldingAttackButton += Time.deltaTime;
            if (timeHoldingAttackButton > 0.5f)
            {
                player.IncreaseChargeAttack();
            }
        }

        if (Input.GetButtonUp("Attack"))
        {
            timeHoldingAttackButton = 0f;
            player.AttackButtonReleased();
        }

        if (Input.GetButtonDown("Jump"))
        {
            player.Jump();
        }

        if (Input.GetButtonDown("Block"))
        {
            player.Block();
        }

        if (Input.GetButtonUp("Block"))
        {
            player.ExitBlock();
        }
    }
}
