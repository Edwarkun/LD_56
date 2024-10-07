using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repulser : MonoBehaviour
{
    public HashSet<EnemyController> targets = new HashSet<EnemyController>();
    public GameObject particles;

    public float forceScale = 10f;
    IEnumerator Start()
    {
        particles.transform.parent = null;
        yield return new WaitForFixedUpdate();

        foreach (var target in targets)
        {
            target.GetComponent<Rigidbody2D>().AddForce((target.transform.position - transform.position) * forceScale, ForceMode2D.Impulse);
        }
        Destroy(this.gameObject);
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
