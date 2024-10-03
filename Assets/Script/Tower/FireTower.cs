using UnityEngine;

public class FireTower : Tower
{
    [SerializeField] private float fireDamage = 100f;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float burnDuration = 2f;
    [SerializeField] private float burnTickTime = 0.25f;
    [SerializeField] private float burnDamage = 10f;


    protected override void Update()
    {
        if (canAttack)
        {
            StartCoroutine(AttackCoolDown());
            if (targetEnemy != null && !targetEnemy.IsDead() && range >= Vector3.Distance(transform.position, targetEnemy.transform.position))
            {
                FireArrow.CreateFireArrow(firePosition.position, targetEnemy, damage, level);
            }
            else
            {
                Enemy newTargetEnemy = GetClosestEnemy();
                if (newTargetEnemy != null)
                {
                    targetEnemy = newTargetEnemy;
                    FireArrow.CreateFireArrow(firePosition.position, targetEnemy, damage, level);
                }
            }
        }
    }

    public override void UpgradeTower()
    {
        base.UpgradeTower();
    }
}
