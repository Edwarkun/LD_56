using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    protected GameObject target;

    [SerializeField]
    protected Rigidbody2D rb;

    public bool canMove = true;

    [SerializeField]
    private Animator anim;


    private void FixedUpdate()
    {
        if (target == null)
            return;

        Move();
    }

    private void Update()
    {
        anim.SetBool("Moving", rb.velocity.sqrMagnitude > 0.01f);
    }
    private void Move()
    {
        if (!canMove)
            return;

        Vector2 movementVector = target.transform.position - this.transform.position;
        rb.AddForce(movementVector.normalized * movementSpeed, ForceMode2D.Force);
    }
    public void SetTarget(GameObject target)
    {
        this.target = target;
        if (movementSpeed == 1.75)
        {
            AudioManager.Instance.PlaySounds("Enemies_ExplotenKitten");
        }
    }
}
