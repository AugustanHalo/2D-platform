using System.Collections;
using UnityEngine;

public class WizardTower : Tower
{
    [SerializeField] private float magicDamage = 40f;
    [SerializeField] private float magicRange = 80f;
    [SerializeField] private float magicCooldown = 1.5f;


    protected override void Awake()
    {
        base.Awake();
        range = magicRange;
        attackCoolDown = magicCooldown;
        damage = magicDamage;
    }

    protected override IEnumerator AttackCoolDown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCoolDown);
        canAttack = true;
    }

    protected override void Update()
    {
        if (canAttack)
        {
            StartCoroutine(AttackCoolDown());
            if (targetEnemy != null && !targetEnemy.IsDead() && range >= Vector3.Distance(transform.position, targetEnemy.transform.position))
            {
                ProjectileMagic.Create(firePosition.position, targetEnemy, damage);
            }
            else
            {
                Enemy newTargetEnemy = GetClosestEnemy();
                if (newTargetEnemy != null)
                {
                    targetEnemy = newTargetEnemy;
                    ProjectileMagic.Create(firePosition.position, targetEnemy, damage);
                }
            }
        }
    }


}
