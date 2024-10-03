using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private float lifeTime = 0;
    private float direction;
    private bool hit;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    
    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);
        lifeTime += Time.deltaTime;
        if (lifeTime > 3)
        {
            Deactive();
        }
       
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        hit = true;
        boxCollider.enabled = false;
        anim.SetTrigger("Explode");

        if(other.CompareTag("Enemy"))
        {
            if(other.GetComponent<Health>() != null)
            {
                other.GetComponent<Health>().TakeDamage(1);
            }
        }
    }

    public void SetDirection(float _direction)
    {
        lifeTime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if(Mathf.Sign(localScaleX) != _direction)
        {
            localScaleX *= -1; 
        }
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactive()
    {
        gameObject.SetActive(false);
    }
}
