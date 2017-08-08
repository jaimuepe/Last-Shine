using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceCough : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Player player = collider.GetComponent<Player>();
            player.forceCough = true;
            gameObject.SetActive(false);
        }
    }
}
