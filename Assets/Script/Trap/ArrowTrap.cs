using System.Collections;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] arrows;
    [SerializeField] private AudioClip arrowSound;


    private float cooldownTimer;

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer > attackCooldown)
        {
            for (int i = 0; i < 5; i++)
            {
                Attack();
            }
        }
        
    }

    private void Attack()
    {
        if (arrowSound)
        {
            AudioManager.instance.PlayAudioClip(arrowSound);
        }
        cooldownTimer = 0;
        arrows[FindInActiveArrow()].transform.position = firePoint.position;
        arrows[FindInActiveArrow()].GetComponent<EnemyProjectile>().ActiveProjectile();
    }

    private int FindInActiveArrow()
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            if (!arrows[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }

    private IEnumerator DelayShot()
    {
        yield return new WaitForSeconds(1);
    }
}
