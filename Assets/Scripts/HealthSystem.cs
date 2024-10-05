using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int health;

    public int maxHealth;

    public Action onDie;

    public GameObject deathParticles;

    public String hurtSound;
    public String deadSound;

    public Animator animator;
    private void Start()
    {
        health = maxHealth;
    }
    public void TakeDamage(int amount)
    {
        if (hurtSound != null)
        {
            AudioManager.Instance.PlaySounds(hurtSound);
        }

        health = Mathf.Max(0, health - amount);
        if(health == 0)
        {
            Die();
        }
        else if(animator != null)
        {
            animator.SetTrigger("Damage");
        }
    }

    public void Heal(int amount) 
    {
        health = Mathf.Min(health + amount, maxHealth);
    }

    public void Die()
    {
        if (deadSound != null)
        {
            AudioManager.Instance.PlaySounds(deadSound);
        }

        if(onDie != null)
            onDie.Invoke();

        if (deathParticles != null)
        {
            deathParticles.SetActive(true);
            deathParticles.transform.parent = null;
        }
        Destroy(this.gameObject);
    }
}
