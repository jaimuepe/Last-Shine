using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManagerMain : MonoBehaviour
{

    AudioSource audioSource;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = GameInfo.instance.music;
    }
}
