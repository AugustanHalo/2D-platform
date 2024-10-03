using System.Collections;
using UnityEngine;
public class HealingEnemy : Enemy
{
    [SerializeField] private float healRadius = 5f;
    [SerializeField] private float healAmount = 10f;
    [SerializeField] private float healInterval = 2f;
    private bool canHeal = true;

    protected override void Awake()
    {
        base.Awake();
        //GetComponent<SpriteRenderer>().color = Color.green;
    }

    protected override void Update()
    {
        base.Update();
        if (canHeal && !IsDead())
        {
            HealNearbyEnemies();
            StartCoroutine(HealCooldown());
        }
    }

    private IEnumerator HealCooldown()
    {
        canHeal = false;
        yield return new WaitForSeconds(healInterval);
        canHeal = true;
    }

    private void HealNearbyEnemies()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, healRadius);
        foreach (Collider2D collider in colliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null && enemy != this && !enemy.IsDead())
            {
                enemy.Heal(healAmount);
                enemy.CreateDamagePopup(healAmount, Color.green);
            }
        }
    }
}