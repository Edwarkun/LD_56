using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject menu;
    public Button continueButton;
    public Slider volumeSlider;
    public Button backToMenuButton;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
            AudioManager.Instance.PlaySounds("UI_PressButton");
        }
    }

    public void TogglePauseMenu()
    {
        if (menu.activeInHierarchy)
        {
            Time.timeScale = 1.0f;
            menu.SetActive(false);
        }
        else
        {
            menu.SetActive(true);
            volumeSlider.value = AudioManager.Instance.GetVolume();
            Time.timeScale = 0.0f;
        }
    }

    public void SetVolume()
    {
        AudioManager.Instance.SetVolume(volumeSlider.value);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
        AudioManager.Instance.PlaySounds("UI_PressButton");
    }
}
