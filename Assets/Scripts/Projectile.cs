using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class Projectile : MonoBehaviour
{
    [HideInInspector]
    public GameObject target;
    [HideInInspector]
    public Vector2 targetPosition;
    [HideInInspector]
    private Vector2 startPosition;

    [Header("Movement")]
    public float speed = 1.0f;
    public float bezierTangentHeight;
    [HideInInspector]
    public float flyTime = 0.0f;
    [HideInInspector]
    public float maxFlyTime;
    public float randomSpread = 0.1f;
    public Vector2 randomSpreadPosition = Vector2.zero;

    [Header("Damaging")]
    [HideInInspector]
    public int damage;
    public GameObject areaEffect;

    public void SetTarget(GameObject target, int attack)
    {
        this.target = target;
        SetTarget(target.transform.position, attack);
    }
    public void SetTarget(Vector2 targetPosition, int attack)
    {
        this.damage = attack;
        targetPosition = target.transform.position;
        Fire();
    }
    private void Fire()
    {
        startPosition = this.transform.position;

        randomSpreadPosition = Random.insideUnitCircle * randomSpread;
        targetPosition += randomSpreadPosition;

        maxFlyTime = (targetPosition - startPosition).magnitude + bezierTangentHeight;
        Debug.Log(maxFlyTime + ", " + (targetPosition - startPosition).magnitude);
    }

    public void Update()
    {
        if(target != null)
            targetPosition = target.transform.position + (Vector3)randomSpreadPosition;

        flyTime += Time.deltaTime * speed;

        float t = Mathf.Min(1.0f, flyTime / maxFlyTime);

        transform.position = CalculateBezierPoint(startPosition + Vector2.up * bezierTangentHeight,
            startPosition,
            targetPosition,
            targetPosition + Vector2.up * bezierTangentHeight,
            t);

        if (t >= 1.0f)
        {
            Impact();
            Destroy(this.gameObject);
        }
    }

    public virtual void Impact()
    {
        if(areaEffect != null)
        {
            Instantiate(areaEffect, this.transform.position, Quaternion.identity, null);
        }
        else if (target != null)
        {
            HealthSystem hs = target.GetComponent<HealthSystem>();
            if(hs != null)
                hs.TakeDamage(damage);
        }
    }

    public Vector2 CalculateBezierPoint(Vector2 p1, Vector2 t1, Vector2 p2, Vector2 t2, float t)
    {
        Vector2 i0 = Vector2.Lerp(t1, p1, t);
        Vector2 i1 = Vector2.Lerp(t1, t2, t); 
        Vector2 i2 = Vector2.Lerp(t2, p2, t);

        Vector2 j0 = Vector2.Lerp(i0, i1, t);
        Vector2 j1 = Vector2.Lerp(i1, i2, t);

        return Vector2.Lerp(j0, j1, t);
    }
}
