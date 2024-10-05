using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] music, sounds;

    [HideInInspector] public bool isPlayingMainMenuMusic;
    [HideInInspector] public bool isPlayingLevelsMusic;

    public AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;


            s.source.loop = s.loop;
        }

    }

    public void PlayMusic(string name)
    {
        Sound song = Array.Find(music, x => x.name == name);

        if (music == null)
        {
            Debug.Log("Song Not Found");
        }

        else
        {
            isPlayingMainMenuMusic = name == "MainMenu";
            musicSource.clip = song.clip;
            musicSource.Play();
        }
    }

    public void PlaySounds(string name)
    {
        Sound s = Array.Find(sounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }

        else
        {
            s.source.volume = (1 + s.volume) * UnityEngine.Random.Range(1.0f - s.modulationRange, 1.0f + s.modulationRange);
            s.source.pitch = (1 + s.pitch) * UnityEngine.Random.Range(1.0f - s.modulationRange, 1.0f + s.modulationRange);
            s.source.PlayOneShot(s.clip);
        }
    }
}
