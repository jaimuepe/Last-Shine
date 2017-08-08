using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    public float smoothTime = 3F;

    bool isLocked;

    Character target;

    public bool isCenteredOnTarget = false;
    Vector2 velocity = Vector2.zero;

    public BoxCollider2D boxColliderBack;

    void Start()
    {
        target = FindObjectOfType<Player>();
    }

    void LateUpdate()
    {
        if (isCenteredOnTarget)
        {
            SnapToTarget();
        }
    }

    void SnapToTarget()
    {
        Vector2 newPos = Vector2.SmoothDamp(transform.position,
            target.transform.position,
            ref velocity,
            smoothTime,
            Mathf.Infinity,
            Time.deltaTime
            );

        transform.position = new Vector3(
            Mathf.Max(0, newPos.x),
            transform.position.y,
            transform.position.z);
    }

    public void StopFollowingTarget()
    {
        isCenteredOnTarget = false;
    }

    public void Release()
    {
        isLocked = false;
    }

    public void Lock()
    {
        isLocked = true;
    }

    public void FollowTarget()
    {
        if (!isLocked)
        {
            isCenteredOnTarget = true;
        }
    }
}
