using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyButton : MonoBehaviour, ISelectHandler
{

    Button b;
    public AudioClip audioClip;
    public bool ignoreSelection;

    private void Start()
    {
        b = GetComponent<Button>();
        b.onClick.AddListener(PlaySound);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!ignoreSelection)
        {
            PlaySound();
        }
    }

    void PlaySound()
    {
        AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position, GameInfo.instance.effects);
    }
}
