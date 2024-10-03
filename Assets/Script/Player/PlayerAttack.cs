using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float AttackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;
    private Animator anim;
    private PlayerMovememt playerMovememt;
    [SerializeField] private float cooldownTimer = 2;
    [SerializeField] private AudioClip fireballSound;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovememt = GetComponent<PlayerMovememt>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && playerMovememt.CanAttack() && cooldownTimer > AttackCooldown)
        {
            Attack();
        }
        cooldownTimer += Time.deltaTime;
    }
    private void Attack()
    {
        if(fireballSound)
        {
            AudioManager.instance.PlayAudioClip(fireballSound);
        }
        anim.SetTrigger("Attack");
        cooldownTimer = 0;

        fireballs[FindInActiveFireball()].transform.position = firePoint.position;
        fireballs[FindInActiveFireball()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }
    
    private int FindInActiveFireball()
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
