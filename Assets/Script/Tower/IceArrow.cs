using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceArrow : ProjectileArrow
{
    float percentTrigger = 0.05f;

    public static void CreateIceArrow(Vector3 spawnPosition, Enemy enemy, float damageAmount, int level)
    {
        Transform arrowTransform = Instantiate(GameAssets.i.pfIceArrow, spawnPosition, Quaternion.identity);
        
        IceArrow iceArrow = arrowTransform.GetComponent<IceArrow>();
        iceArrow.Setup(enemy, damageAmount, level);
    }

    private void Setup(Enemy enemy, float damageAmount, int level)
    {
        percentTrigger = 0.05f + (level - 1) * 0.025f;
        Setup(enemy, damageAmount);
    }

    protected override void Update()
    {
        base.Update();

        if (enemy == null)
        {
            Destroy(gameObject);
            return;
        }
        if(distanceToEnemy < 1)
        {
            if (Random.Range(0f, 1f) < percentTrigger)
            {
                enemy.ApplyBlue();
                if (enemy.isHitBlue && enemy.isHitRed)
                {
                    enemy.TriggerHollowPurpleEffect();
                }
            }
        }
    }
    
  
}
