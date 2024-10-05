using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        if (!AudioManager.Instance.isPlayingMainMenuMusic)
        {
            AudioManager.Instance.PlayMusic("MainMenu");
        }
    }

    // Update is called once per frame
    public void PlayGame()
    {
        AudioManager.Instance.PlaySounds("PressButton");
        SceneManager.LoadScene("LevelSelector");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MuteGame()
    {

    }
}
