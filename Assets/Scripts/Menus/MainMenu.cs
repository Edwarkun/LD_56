using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Slider volumeSlider;
    // Start is called before the first frame update
    private void Start()
    {
        volumeSlider.value = AudioManager.Instance.GetVolume();
        if (!AudioManager.Instance.isPlayingMainMenuMusic)
        {
            AudioManager.Instance.PlayMusic("MainMenu");
        }
    }

    // Update is called once per frame
    public void PlayGame()
    {
        AudioManager.Instance.PlaySounds("UI_PressButton");
        SceneManager.LoadScene("LevelSelector");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetVolume()
    {
        AudioManager.Instance.SetVolume(volumeSlider.value);
    }
}
