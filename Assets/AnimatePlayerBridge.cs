using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatePlayerBridge : MonoBehaviour
{

    public GameObject g;
    Player player;

    public SceneChanger sceneChager;
    bool triggered = false;

    private void OnEnable()
    {
        player = FindObjectOfType<Player>();
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        yield return new WaitForSeconds(2f);
        player.MoveTo(g.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered && collision.CompareTag("Player"))
        {
            triggered = true;
            CharacterController2D cc = player.GetComponent<CharacterController2D>();
            cc.ValidateJump();
            cc.controlledFall = false;
            sceneChager.gameObject.SetActive(true);
            sceneChager.Change();
        }
    }
}
