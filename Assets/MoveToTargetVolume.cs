using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTargetVolume : MonoBehaviour
{
    GameObject target;

    bool active = false;

    void Start()
    {
        target = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!active && collision.CompareTag("PlayerGroundCheck"))
        {
            active = true;
            Player p = collision.transform.parent.GetComponent<Player>();
            p.MoveTo(target.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.GetChild(0).position, 100f);
    }
}
