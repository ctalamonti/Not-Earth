﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour

{
    public static SoundManager instance = null;
    [HideInInspector] public AudioSource audioSource = null;

    [Tooltip("Only put music clips in here!!!")]
    [SerializeField] public AudioClip[] musicClips = new AudioClip[0];
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public bool IsGameClipPSelected()
    {
        return audioSource.clip == musicClips[0];
    }

    public void SetClip(int audioSourceTrigger)
    {
        audioSource.clip = musicClips[audioSourceTrigger];
    }

    

}


