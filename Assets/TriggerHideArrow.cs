using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHideArrow : MonoBehaviour
{

    public LayerMask playerLayer;

    UIManager uiManager;
    BoxCollider2D boxCollider;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        Collider2D player = Physics2D.OverlapBox(
           boxCollider.bounds.center,
           boxCollider.bounds.size,
           0f,
           playerLayer);

        if (player)
        {
            uiManager.HideArrow();
            transform.parent.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        uiManager = FindObjectOfType<UIManager>();
    }
}
