using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{


    public Material blinkMaterial;
    public AudioClip biteClip;
    public int hpAmmount;

    public float timeBeforeBeginBlinking;
    public float totalTimeBeforeDissapearing;

    Material defaultMaterial;

    Shadow shadow;
    SpriteRenderer sr;
    SpriteRenderer shadowSr;
    float targetY;

    public bool readyToPickUp = true;

    private void Start()
    {
        targetY = transform.position.y;
        float newY = targetY + 3000 + Random.Range(-200f, 200f);

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z);

        shadow = GetComponentInChildren<Shadow>();
        sr = GetComponent<SpriteRenderer>();
        shadowSr = shadow.GetComponent<SpriteRenderer>();

        defaultMaterial = sr.material;

        StartCoroutine(FallAnimation());
    }

    IEnumerator Blink()
    {
        yield return new WaitForSeconds(timeBeforeBeginBlinking);

        float waitTime = 0.3f;
        bool visible = true;

        sr.material = blinkMaterial;
        shadowSr.material = blinkMaterial;

        float t = 0f;

        float timeBlinking = totalTimeBeforeDissapearing - timeBeforeBeginBlinking;

        while (t < timeBlinking)
        {
            sr.material.SetFloat("_alpha", visible ? 1f : 0f);
            shadowSr.material.SetFloat("_alpha", visible ? 1f : 0f);

            visible = !visible;

            yield return new WaitForSeconds(waitTime);
            t += Time.deltaTime + waitTime;
        }

        sr.material = defaultMaterial;
        shadowSr.material = defaultMaterial;

        Destroy(gameObject);
    }

    IEnumerator FallAnimation()
    {
        float posY = transform.position.y;

        while (posY > targetY)
        {
            posY -= 20f;
            if (posY < targetY)
            {
                break;
            }
            transform.position = new Vector3(transform.position.x, posY,
                transform.position.z);

            shadow.SetExtraHeight(posY - targetY);
            yield return null;
        }

        transform.position = new Vector3(
            transform.position.x,
            targetY,
            transform.position.z);

        shadow.SetExtraHeight(0);
        readyToPickUp = true;

        StartCoroutine(Blink());
    }
}
