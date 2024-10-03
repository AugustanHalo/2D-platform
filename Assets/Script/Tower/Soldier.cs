using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    private BarrackTower parentTower;
    private float damage;
    private float maxHealth;
    private float currentHealth;
    private Enemy targetEnemy;
    private float attackRange = 10f;
    [SerializeField]private float moveSpeed = 10f;
    [SerializeField] private float attackCooldown = 1f;
    private bool canAttack = true;
    private Vector3 groupPosition; // position of the group of soldiers
    private float separationRadius = 6.5f; // distance between soldiers

    public void Setup(BarrackTower tower, float damage, float health, Vector3 groupPos, Vector3 _position)
    {
        transform.SetParent(tower.transform);
        this.parentTower = tower;
        this.damage = damage;
        this.maxHealth = health;
        this.currentHealth = health;
        this.groupPosition = groupPos;
    }

    public static Transform Create(Vector3 position, BarrackTower tower, float damage, float health, Vector3 groupPos)
    {
        Transform soldierObj = Instantiate(GameAssets.i.pfSoldier, position, Quaternion.identity);
        Soldier soldier = soldierObj.GetComponent<Soldier>();
        soldier.Setup(tower, damage, health, groupPos, position);
        return soldierObj;
    }
    private void Update()
    {
        if (targetEnemy == null || targetEnemy.IsDead())
        {
            targetEnemy = Enemy.GetClosetEnemy(parentTower.transform.position, parentTower.GetRange());
        }

        if (targetEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);
            if (distanceToEnemy <= attackRange && canAttack)
            {
                StartCoroutine(Attack());
            }
            else
            {
                if (Vector3.Distance(parentTower.transform.position, targetEnemy.transform.position) < parentTower.GetRange() && canAttack)
                {
                    MoveTowardsEnemy();
                }
                else if(Vector3.Distance(parentTower.transform.position, targetEnemy.transform.position) > parentTower.GetRange())
                {
                    MoveToGroupPosition();
                }
            }
        }
        else
        {
            MoveToGroupPosition();
        }
    }

    private void MoveToGroupPosition()
    {
        Vector3 separation = CalculateSeparation(separationRadius);
        Vector3 targetPosition = groupPosition + separation;
        if(Vector3.Distance(transform.position, targetPosition) > separationRadius)
        {
            if(transform.position.magnitude - targetPosition.magnitude < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private Vector3 CalculateSeparation(float seperattion)
    {
        Vector3 separation = Vector3.zero;
        int neighborCount = 0;
        foreach (Soldier soldier in parentTower.GetActiveSoldiers())
        {
            if (soldier != this)
            {
                float distance = Vector3.Distance(transform.position, soldier.transform.position);
                if (distance < seperattion)
                {
                    Vector3 diff = transform.position - soldier.transform.position;
                    separation += diff.normalized / distance;
                    neighborCount++;
                }
            }
        }
        if (neighborCount > 0)
        {
            separation /= neighborCount;
            separation = separation.normalized * seperattion;
        }
        return separation + new Vector3(Random.insideUnitCircle.x,Random.insideUnitCircle.y);
    }

    private void MoveTowardsEnemy()
    {
        Vector3 separation = CalculateSeparation(5f);
        Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
        if(Vector3.Distance(transform.position, (direction + separation)) > attackRange - 2.5f)
        {
            if (transform.position.magnitude - (direction + separation).magnitude < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            transform.position += moveSpeed * Time.deltaTime * (direction + separation);
        }
    }

    private IEnumerator Attack()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        if (targetEnemy != null)
        {
            targetEnemy.TakeDamage(damage);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        parentTower.RemoveSoldier(this);
        Destroy(gameObject);
    }
}
