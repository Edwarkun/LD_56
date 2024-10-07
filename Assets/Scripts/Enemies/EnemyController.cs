using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyMovement movement;
    public EnemyAttack attack;

    public GameObject target;

    public void Start()
    {
        GameController.Instance.enemies.Add(this);
    }
    public void OnDestroy()
    {
        if(GameController.Instance != null)
            GameController.Instance.enemies.Remove(this);
    }

    public void BeginRound() 
    {
        movement.enabled = true;
        attack.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.Instance.RoundStarted)
            return;

        //Check for tower
        if(target == null)
        {
            if (GridController.Instance.placeables.Count == 0)
                return;

            GameObject closestTower = GridController.Instance.placeables.Values.First().gameObject;
            float minSqrDist = (closestTower.transform.position - this.transform.position).sqrMagnitude;
            //Get tower
            foreach (var item in GridController.Instance.placeables)
            {
                float newSqrDist = (item.Value.gameObject.transform.position - this.transform.position).sqrMagnitude;
                if(newSqrDist < minSqrDist)
                {
                    closestTower = item.Value.gameObject;
                    minSqrDist = newSqrDist;
                }
            }
            target = closestTower;
            movement.SetTarget(target);
        }

        //Get into range
        if ((target.transform.position - this.transform.position).sqrMagnitude > attack.squaredRange)
        {
            movement.canMove = true;
            return;
        }

        //Attack Tower
        movement.canMove = false;
        if (!attack.isAttacking)
        {
            attack.Attack(target);
        }
    }
}
