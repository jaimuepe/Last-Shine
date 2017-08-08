using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateText : MonoBehaviour
{

    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Select()
    {
        animator.SetBool("animate", true);
    }

    public void Animate(bool animate)
    {
        animator.SetBool("animate", animate);
    }
}
