using UnityEngine;
using System;

public class EnemyProjectile : Enemy_Damage
{
    [SerializeField] private float speed;
    [SerializeField] private float resetTime;
    private float lifeTime;
    private BoxCollider2D boxCollider;
    private bool hit;
    private Animator animator;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    public void ActiveProjectile()
    {
        lifeTime = 0;
        System.Random num = new System.Random();
        //SetDirection(Mathf.Clamp(num.Next(-31,31),-30,30));
        gameObject.SetActive(true);
        boxCollider.enabled = true;

    }
    private void Update()
    {
        if (hit) return;
        lifeTime += Time.deltaTime;
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);   
        if (lifeTime > resetTime)
        {
            gameObject.SetActive(false);
            lifeTime = 0;
        }
    }

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        base.OnTriggerEnter2D(collision);
        boxCollider.enabled = false;

        if (animator != null)
        {
            animator.SetTrigger("Explode");
           
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Deactive()
    {
        gameObject.SetActive(false);
        hit = false;
    }
    public void SetDirection(float direction)
    {
        Debug.Log(direction);
        Vector3 fwd = new Vector3(0,0,direction);
        transform.Rotate(fwd, Space.Self);
    }

}
