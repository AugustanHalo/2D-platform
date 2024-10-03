using UnityEngine;

public class Enemy_SideWalk : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float distance;
    private bool movingRight = true;
    private float leftEdge;
    private float rightEdge;

    private void Awake()
    {
        leftEdge = transform.position.x - distance;
        rightEdge = transform.position.x + distance;
    }

    private void Update()
    {
        if(movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }

        if(transform.position.x >= rightEdge)
        {
            movingRight = false;
        }
        else if(transform.position.x <= leftEdge)
        {
            movingRight = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(damage);
        }
    }


}
