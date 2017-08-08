using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{

    public Vector3 shadowOffset;

    float extraHeight = 0f;

    public void SetExtraHeight(float extraHeight)
    {
        this.extraHeight = extraHeight;
    }

    private void LateUpdate()
    {
        float extraJumpHeight = extraHeight;
        Vector3 pos = transform.parent.position;
        pos.y -= Mathf.Abs(transform.parent.localScale.x) * extraJumpHeight;
        pos += Mathf.Abs(transform.parent.localScale.x) * shadowOffset;

        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
}
