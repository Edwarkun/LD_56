using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TurretAttack : MonoBehaviour
{

    //TARGETING
    public HashSet<EnemyController> targets = new HashSet<EnemyController>();
    [HideInInspector]
    public EnemyController currentTarget;

    [Header("Attacking")]
    public int attack = 1;
    public String attackSound;

    [HideInInspector]
    public float currentAttackTime = 0.0f;
    public float attackTime = 1.0f;

    [Header("Animator")]
    [SerializeField]
    protected Animator anim;
    [HideInInspector]
    public bool isAttacking = false;

    [Header("Projectile")]
    public Projectile projectile;
    public Transform projectileSpawnPoint;

    public void Start()
    {
        currentAttackTime = attackTime;
    }

    public void Attack()
    {
        currentAttackTime = 0.0f;
        isAttacking = true;

        anim.SetTrigger("Attack");
    }

    private void Update()
    {
        if (!GameController.Instance.RoundStarted)
            return;

        if (isAttacking)
        {
            currentAttackTime += Time.deltaTime;
            if (currentAttackTime >= attackTime)
            {
                isAttacking = false;
            }
        }
        else
        {
            if (currentTarget == null)
            {
                if (!GetClosestTarget())
                    return;
            }
            Attack();
        }
    }

    public bool GetClosestTarget()
    {
        if (targets.Count == 0)
            return false;

        var temp = targets.GetEnumerator();
        temp.MoveNext();
        currentTarget = temp.Current;
        float closestDistance = (currentTarget.transform.position - this.transform.position).sqrMagnitude;

        foreach (var target in targets)
        {
            float dist = (target.transform.position - transform.position).sqrMagnitude;
            if (dist < closestDistance)
            {
                currentTarget = target;
                closestDistance = dist;
            }
        }
        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController controller = collision.gameObject.GetComponent<EnemyController>();
        if (controller != null)
        {
            if (!targets.Contains(controller))
                targets.Add(controller);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        EnemyController controller = collision.gameObject.GetComponent<EnemyController>();
        if (controller != null)
        {
            if (targets.Contains(controller))
                targets.Remove(controller);
        }
    }
    public void SpawnProjectile()
    {
        if (currentTarget == null)
        {
            if(projectile.areaEffect != null)
                Instantiate(projectile.areaEffect, this.transform.position, Quaternion.identity, null);
            return;
        }

        Projectile p = Instantiate(projectile, null);

        p.transform.position = projectileSpawnPoint.position;
        p.SetTarget(currentTarget.gameObject, attack);

        if (attackSound != null)
        {
            AudioManager.Instance.PlaySounds(attackSound);
        }

    }

}
