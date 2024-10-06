using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DamageArea : MonoBehaviour
{
    public HashSet<HealthSystem> targets = new HashSet<HealthSystem>();

    public int damage;

    IEnumerator Start()
    {
        yield return new WaitForFixedUpdate();
        Queue<HealthSystem> hitTargets = new Queue<HealthSystem>();
        foreach (var target in targets)
        {
            hitTargets.Enqueue(target);
        }
        while (hitTargets.Count > 0)
        {
            hitTargets.Dequeue().TakeDamage(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthSystem controller = collision.gameObject.GetComponent<HealthSystem>();
        if (controller != null)
        {
            if (!targets.Contains(controller))
                targets.Add(controller);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        HealthSystem controller = collision.gameObject.GetComponent<HealthSystem>();
        if (controller != null)
        {
            if (targets.Contains(controller))
                targets.Remove(controller);
        }
    }

}
