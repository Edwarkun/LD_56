using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DamageArea : MonoBehaviour
{
    public HashSet<EnemyController> targets = new HashSet<EnemyController>();

    public int damage;

    IEnumerator Start()
    {
        yield return new WaitForFixedUpdate();
        Queue<HealthSystem> hitTargets = new Queue<HealthSystem>();
        foreach (var target in targets)
        {
            HealthSystem hp = target.GetComponent<HealthSystem>();
            if (hp != null)
                hitTargets.Enqueue(hp);
        }
        while (hitTargets.Count > 0)
        {
            hitTargets.Dequeue().TakeDamage(damage);
        }
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

}
