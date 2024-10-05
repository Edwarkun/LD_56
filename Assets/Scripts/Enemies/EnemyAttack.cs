using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int attack = 1;
    public float range = 0.5f;
    [HideInInspector]
    public float squaredRange = 1f;
    [HideInInspector]
    public float currentAttackTime = 0.0f;
    public float attackTime = 1.0f;

    [SerializeField]
    protected Animator anim;
    [HideInInspector]
    public bool isAttacking = false;
    public String meleAttackSound;
    public String rangeAttackSound;

    [HideInInspector]
    public GameObject target;

    [Header("Ranged Only")]
    public Projectile projectile;
    public Transform projectileSpawnPoint;

    protected void Start()
    {
        squaredRange = range * range;
    }

    public void Attack(GameObject target)
    {
        currentAttackTime = 0.0f;
        isAttacking = true;

        this.target = target;

        anim.SetTrigger("Attack");

        if (meleAttackSound != null)
        {
            AudioManager.Instance.PlaySounds(meleAttackSound);
        }

    }

    private void Update()
    {
        if (isAttacking)
        {
            currentAttackTime += Time.deltaTime;
            if(currentAttackTime > attackTime)
            {
                isAttacking = false;
            }
        }
    }

    public void DealDamage()
    {
        if (target == null)
            return;

        HealthSystem healthSystem = target.GetComponent<HealthSystem>();

        if (healthSystem == null)
            return;

        healthSystem.TakeDamage(attack);
    }

    public void SpawnProjectile()
    {
        if (target == null)
            return;

        Projectile p = Instantiate(projectile, null);

        p.transform.position = projectileSpawnPoint.position;
        p.SetTarget(target, attack);

        if (rangeAttackSound != null)
        {
            AudioManager.Instance.PlaySounds(rangeAttackSound);
        }

    }
}
