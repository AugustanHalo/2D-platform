using System;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("AttackParameter")]
    [SerializeField] private float range;
    [SerializeField] private float damage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject[] fireballs;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Collider Parameter")]
    [SerializeField] private BoxCollider2D box;
    [SerializeField] private float colliderDistance;

    private float cooldownTimer = Mathf.Infinity;
    private Animator animator;
    private Patroling patroling;
    [SerializeField] private AudioClip fireballSound;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        patroling = GetComponent<Patroling>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (PlayerInRange())
        {
            if (cooldownTimer > attackCooldown)
            {
                cooldownTimer = 0;
                animator.SetTrigger("rangedAttack");

            }
        }

        if (patroling != null)
        {
            patroling.enabled = !PlayerInRange();
        }
    }

    private bool PlayerInRange()
    {
        Vector3 size = box.bounds.size;
        size.x *= range * transform.localScale.x * range;
        Vector3 boxLocation = box.bounds.center + transform.right * range * transform.localScale.x * colliderDistance;
        RaycastHit2D hit = Physics2D.BoxCast(boxLocation, box.bounds.size * range, 0, Vector2.left, 0, playerLayer);
        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 size = box.bounds.size;
        size.x *= range * transform.localScale.x * range;
        Vector3 boxLocation = box.bounds.center + transform.right * range * transform.localScale.x * colliderDistance;
        Gizmos.DrawWireCube(boxLocation, size);
    }

    private void RangedAttack()
    {
        if (fireballSound)
        {
            AudioManager.instance.PlayAudioClip(fireballSound);
        }
        cooldownTimer = 0;
        fireballs[FindFireball()].transform.position = attackPoint.position;
        fireballs[FindFireball()].GetComponent<EnemyProjectile>().ActiveProjectile();
        fireballs[FindFireball()].GetComponent<EnemyProjectile>().SetDirection(transform.localScale.x > 0 ? 0 : 180);
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }
}
