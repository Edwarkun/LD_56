using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Button[] buttons;
    public GameObject levelButtons;

    public static int selectedLevel = 0;

    private void Awake()
    {
        ButtonsToArray();
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 0);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = i <= unlockedLevel;

            int completed = PlayerPrefs.GetInt("CompletedLevels", 0);
            buttons[i].transform.GetChild(0).gameObject.SetActive(i > unlockedLevel);
            buttons[i].transform.GetChild(2).gameObject.SetActive((completed >> i) % 2 > 0);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (!AudioManager.Instance.isPlayingMainMenuMusic)
        {
            AudioManager.Instance.PlayMusic("MainMenu");
        }
    }

    public void OpenLevel(int levelId)
    {
        selectedLevel = 0;
        SceneManager.LoadScene(2);
        AudioManager.Instance.PlaySounds("PressButton");

        if (!AudioManager.Instance.isPlayingLevelsMusic)
        {
            AudioManager.Instance.PlayMusic("LevelsMusic");
        }
    }

    public void ReturnMainMenu()
    {
        AudioManager.Instance.PlaySounds("PressButton");
        SceneManager.LoadScene("MainMenu");
    }

    void ButtonsToArray()
    {
        int childCount = levelButtons.transform.childCount;
        buttons = new Button[childCount];
        for (int i = 0; i < childCount; i++)
        {
            buttons[i] = levelButtons.transform.GetChild(i).gameObject.GetComponent<Button>();
        }
    }
    //This function will be in the winning condition when finishing one level
    public static void CompleteLevel(int index, bool completed)
    {
        int maxUnlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        int currentCompletedLevels = PlayerPrefs.GetInt("CompletedLevels", 1);
        PlayerPrefs.SetInt("UnlockedLevel", Mathf.Max(maxUnlockedLevel, index));
        PlayerPrefs.SetInt("CompletedLevels", currentCompletedLevels & (1 << index));
        PlayerPrefs.Save();
    }

    public void UnlockAllLevels()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
            buttons[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        PlayerPrefs.SetInt("UnlockedLevel", 10);
    }
}
