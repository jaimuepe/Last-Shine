using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterController2D : MonoBehaviour
{

    Animator animator;
    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        if (h != 0.0f)
        {
            if (h > 0.0f)
            {
                transform.position += new Vector3(2.5f, 0f, 0f);
                transform.localScale = new Vector3(
                    -Mathf.Abs(transform.localScale.x),
                    transform.localScale.y,
                    transform.localScale.z);
            }
            else
            {
                transform.position += new Vector3(-2.5f, 0f, 0f);
                transform.localScale = new Vector3(
                    Mathf.Abs(transform.localScale.x),
                    transform.localScale.y,
                    transform.localScale.z);
            }
            animator.SetBool("walk", true);
        }
        else
        {
            animator.SetBool("walk", false);
        }
    }
}
