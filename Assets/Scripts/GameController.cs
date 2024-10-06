using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int maxLosses = 2;
    private bool[] victories;
    [SerializeField]
    private LevelRounds[] levelRounds;

    public Transform roundIndicator;
    public GameObject roundResultPrefab;

    public int currentRound = 0;

    public static bool RoundStarted = false;

    public static GameController Instance = null;

    public Animator LeverAnimatior;

    public HashSet<EnemyController> enemies = new HashSet<EnemyController>();

    public Animator roundAnim;

    public GameObject dialog;
    public TextMeshProUGUI dialogText;

    public bool canCombine = true;
    public GameObject[] towerButtons;

    public void Start()
    {
        Instance = this;
        victories = new bool[levelRounds[LevelSelector.selectedLevel].rounds.Length];
        for(int i = 0; i < levelRounds[LevelSelector.selectedLevel].rounds.Length; i++)
        {
            Instantiate(roundResultPrefab, roundIndicator);
        }
        SetupRound();
    }

    private void OnDestroy()
    {
        if(Instance == this)
            Instance = null;
    }

    public void Update()
    {
        if (RoundStarted)
        {
            if(enemies.Count == 0 || GridController.Instance.placeables.Count == 0)
            {
                RoundStarted = false;
                FinishRound();
            }
        }
    }

    public void SetupRound()
    {
        EnemyController[] remaninigEnemies = enemies.ToArray();
        foreach (var remainingEnemy in remaninigEnemies)
        {
            Destroy(remainingEnemy.gameObject);
        }
        enemies.Clear();

        levelRounds[LevelSelector.selectedLevel].rounds[currentRound].SpawnEnemies();
        GridController.Instance.SetupRound(levelRounds[LevelSelector.selectedLevel].rounds[currentRound].resources);

        dialog.SetActive(levelRounds[LevelSelector.selectedLevel].rounds[currentRound].dialog.Length > 0);
        dialogText.text = levelRounds[LevelSelector.selectedLevel].rounds[currentRound].dialog;

        for(int i = 0; i < towerButtons.Length; i++)
        {
            towerButtons[i].SetActive(levelRounds[LevelSelector.selectedLevel].rounds[currentRound].canPlaceTower[i]);
        }
        canCombine = levelRounds[LevelSelector.selectedLevel].rounds[currentRound].canCombine;
    }
    public void StartRound()
    {
        RoundStarted = true;
        foreach (var enemyController in enemies)
        {
            enemyController.BeginRound();
        }
    }

    public void FinishRound()
    {
        bool won = enemies.Count == 0;
        if (won)
        {
            victories[currentRound] = true;
        }
        roundIndicator.GetChild(currentRound).GetChild(won ? 0 : 1).gameObject.SetActive(true);
        
        currentRound++;
        roundAnim.SetTrigger(won ? "RoundWon" : "RoundLost");
        StartCoroutine(WaitAndReset());
    }

    public void PlayStartRoundAnimation()
    {
        AudioManager.Instance.PlaySounds("UI_StartRound");
        LeverAnimatior.SetTrigger("Activate");
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        GridController.Instance.StartRound();
        yield return new WaitForSeconds(1.5f);
        StartRound();
    }

    public IEnumerator WaitAndReset()
    {
        yield return new WaitForSeconds(4.0f);
        int losses = 0;
        for (int i = 0; i < currentRound; i++)
            losses += victories[i] ? 0 : 1;

        if (losses > maxLosses)
        {
            //Trigger loss animation
            SceneManager.LoadScene(1);
        }
        else if (currentRound == levelRounds[LevelSelector.selectedLevel].rounds.Length)
        {
            //Trigger finish animation
            LevelSelector.CompleteLevel(0, losses == 0);
            SceneManager.LoadScene(1);
        }
        else
        {
            LeverAnimatior.SetTrigger("Reset");
            SetupRound();
        }
    }
}
