using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateZDepth : MonoBehaviour
{

    BoxCollider2D boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (boxCollider)
        {
            float y = transform.position.y - boxCollider.size.y / 2;
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                y / 100);
        }
    }
}
