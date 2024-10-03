using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header ("AttackParameter")]
    [SerializeField] private float range;
    [SerializeField] private float damage;   
    [SerializeField] private float attackCooldown;
    [SerializeField] private AudioClip meleeSound;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Collider Parameter")]
    [SerializeField] private BoxCollider2D box;
    [SerializeField] private float colliderDistance;

    private float cooldownTimer = Mathf.Infinity;
    private Animator animator;
    private Health player;
    private Patroling patroling;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        patroling = GetComponent<Patroling>();
    }
    private void Update()
    {
        cooldownTimer += Time.deltaTime;
        if(PlayerInRange())
        {
            if (cooldownTimer > attackCooldown)
            {
                cooldownTimer = 0;
                animator.SetTrigger("meleeAttack");
                
            }
        }

        if(patroling != null)
        {
            patroling.enabled = !PlayerInRange();
        }
    }

    private bool PlayerInRange()
    {
        Vector3 size = box.bounds.size;
        size.x *= range  * transform.localScale.x * range;
        Vector3 boxLocation = box.bounds.center + transform.right * range * transform.localScale.x * colliderDistance;
        RaycastHit2D hit = Physics2D.BoxCast(boxLocation, box.bounds.size * range, 0, Vector2.left, 0, playerLayer);
        if(hit.collider != null)
        {
            player = hit.collider.GetComponent<Health>();
        }

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 size = box.bounds.size;
        size.x *= range * transform.localScale.x * range;
        Vector3 boxLocation = box.bounds.center + transform.right * range * transform.localScale.x * colliderDistance;
        Gizmos.DrawWireCube(boxLocation,size);
    }

    private void DamagePlayer()
    {
        if(PlayerInRange())
        {
            if(!player.isInvulnerable)
            {
                player.TakeDamage(damage);
            }
        }
        if(meleeSound)
        {
            AudioManager.instance.PlayAudioClip(meleeSound);
        }
    }
}
