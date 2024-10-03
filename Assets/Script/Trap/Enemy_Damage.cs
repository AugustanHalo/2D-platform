using UnityEngine;

public class Enemy_Damage : MonoBehaviour
{
    [SerializeField] private float damage;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(collision.GetComponent<Health>() != null && !collision.GetComponent<Health>().isInvulnerable)
            {
                collision.GetComponent<Health>().TakeDamage(damage);
            }
            
        }
    }
}
