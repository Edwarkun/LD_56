using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static bool RoundStarted = false;

    public Animator LeverAnimatior;
    public void StartRound()
    {
        RoundStarted = true;
        EnemyController[] enemyControllers = GameObject.FindObjectsOfType<EnemyController>();

        foreach (var enemyController in enemyControllers)
        {
            enemyController.BeginRound();
        }
    }

    public void PlayStartRoundAnimation()
    {
        AudioManager.Instance.PlaySounds("UI_StartRound");
        LeverAnimatior.SetBool("isGoingDown", true);


    }
}
