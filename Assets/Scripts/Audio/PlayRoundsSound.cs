using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRoundsSound : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayWinRounds()
    {
        AudioManager.Instance.PlaySounds("UI_WinRound");

    }

    public void PlayLoseRounds()
    {
        AudioManager.Instance.PlaySounds("UI_LoseRound");
        
    }
}
