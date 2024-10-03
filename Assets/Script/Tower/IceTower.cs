using UnityEngine;

public class IceTower : Tower
{
    protected override void Update()
    {
        if (canAttack)
        {
            StartCoroutine(AttackCoolDown());
            if (targetEnemy != null && !targetEnemy.IsDead() && range >= Vector3.Distance(transform.position, targetEnemy.transform.position))
            {
                IceArrow.CreateIceArrow(firePosition.position, targetEnemy, damage, level);
            }
            else
            {
                Enemy newTargetEnemy = GetClosestEnemy();
                if (newTargetEnemy != null)
                {
                    targetEnemy = newTargetEnemy;
                    IceArrow.CreateIceArrow(firePosition.position, targetEnemy, damage, level);
                }
            }
        }
    }
   public override void UpgradeTower()
    {
        base.UpgradeTower();
    }
}
