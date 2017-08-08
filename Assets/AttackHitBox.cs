using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitBox : MonoBehaviour
{
    public bool causesKnockBack;
    public int damage;

    public LayerMask targetLayerMask;

    public AudioClip hitClip;
    public AudioClip missClip;
    public AudioClip blockClip;

    BoxCollider2D boxCollider;

    bool playedClip = false;
    List<GameObject> damagedCharacters = new List<GameObject>();

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            targetLayerMask);

        bool playClipLocal = false;

        if (colliders.Length != 0)
        {
            bool someoneGotHurt = false;

            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Player") || collider.CompareTag("Enemy"))
                {
                    Character c = collider.GetComponent<Character>();

                    if (!c.ignoreCollisions && !c.IsDead() && !c.IsKnockedback() &&
                        !damagedCharacters.Contains(collider.gameObject))
                    {
                        playClipLocal = true;
                        damagedCharacters.Add(collider.gameObject);
                        bool comingFromRight = transform.parent.localScale.x > 0;

                        someoneGotHurt = someoneGotHurt || c.Damage(damage, causesKnockBack, comingFromRight, false);
                    }
                }
            }

            if (playClipLocal && !playedClip)
            {
                playedClip = true;
                if (someoneGotHurt)
                {
                    AudioSource.PlayClipAtPoint(hitClip, Camera.main.transform.position, GameInfo.instance.effects);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(blockClip, Camera.main.transform.position, GameInfo.instance.effects);
                }
            }
        }
        if (!playClipLocal && !playedClip && missClip)
        {
            playedClip = true;
            AudioSource.PlayClipAtPoint(missClip, Camera.main.transform.position, GameInfo.instance.effects);
        }
    }

    private void OnEnable()
    {
        damagedCharacters.Clear();
    }

    private void OnDisable()
    {
        playedClip = false;
        damagedCharacters.Clear();
    }

    private void OnDrawGizmos()
    {
        if (boxCollider)
        {
            Gizmos.DrawCube(boxCollider.bounds.center,
                boxCollider.bounds.size);
        }
    }
}
