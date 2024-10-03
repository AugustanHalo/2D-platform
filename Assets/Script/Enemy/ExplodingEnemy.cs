using System.Collections;
using UnityEngine;

public class ExplodingEnemy : Enemy
{
    [SerializeField] private float explosionRadius = 50f;
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private Animator animator;

    protected override void Awake()
    {
        base.Awake();
        //GetComponent<SpriteRenderer>().color = Color.red;
        animator = animator == null ? GetComponent<Animator>() : animator;
    }
    protected override void Die()
    {
        animator.SetTrigger("Die");
        Explode();  
    }

    private void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D collider in colliders)
        {
            Debug.Log(collider.name);
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null && collider.gameObject != gameObject)
            {
                damageable.TakeDamage(explosionDamage);
            }
        }
        //Vector3 up = new Vector3(transform.position.x, transform.position.y + explosionRadius, transform.position.z);
        //Vector3 down = new Vector3(transform.position.x, transform.position.y - explosionRadius, transform.position.z);
        //Vector3 left = new Vector3(transform.position.x - explosionRadius, transform.position.y, transform.position.z);
        //Vector3 right = new Vector3(transform.position.x + explosionRadius, transform.position.y, transform.position.z);
        //Debug.DrawLine(transform.position,up, Color.red, 5f); 
        //Debug.DrawLine(transform.position, down, Color.white, 5f);
        //Debug.DrawLine(transform.position, left, Color.green, 5f);
        //Debug.DrawLine(transform.position, right, Color.blue, 5f);
        // You can add visual effects for the explosion here
    }
}