using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEffect : MonoBehaviour
{

    AudioSource audioSource;

    MeshRenderer mr;
    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = GameInfo.instance.effects;
        mr = GetComponent<MeshRenderer>();
        StartCoroutine(UpdateStaticSeed());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator UpdateStaticSeed()
    {
        while (true)
        {
            mr.material.SetFloat("_TimeD", Time.unscaledTime);
            yield return new WaitForSeconds(0.4f);
        }
    }
}
