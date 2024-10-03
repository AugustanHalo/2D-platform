using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrow : ProjectileArrow
{
    float percentTrigger = 0.05f;

    public static void CreateFireArrow(Vector3 spawnPosition, Enemy enemy, float damageAmount, int level)
    {
        Transform arrowTransform = Instantiate(GameAssets.i.pfFireArrow, spawnPosition, Quaternion.identity);
        FireArrow fireArrow = arrowTransform.GetComponent<FireArrow>();
        fireArrow.Setup(enemy, damageAmount, level);
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
                enemy.ApplyRed();
                if (enemy.isHitBlue && enemy.isHitRed)
                {
                    enemy.TriggerHollowPurpleEffect();
                }
            }
        }
    }
}
