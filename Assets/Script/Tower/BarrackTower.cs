using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BarrackTower : Tower
{
    [SerializeField] private int maxSoldiers = 3;
    [SerializeField] private float spawnCooldown = 20f;
    [SerializeField] private float soldierDamage = 15f;
    [SerializeField] private float soldierHealth = 50f;
    [SerializeField] private float attackRange = 30f;
    [SerializeField] private float groupRadius = 5f; // Radius for the soldier group
    private Vector3 groupPosition;
    private Grid grid;
    private List<Soldier> activeSoldiers = new List<Soldier>();

    public float GetGroupRadius()
    {
        return groupRadius;
    }
    protected override void Awake()
    {
        base.Awake();
        grid = FindAnyObjectByType<Testing>().GetGrid();
        groupPosition = GetClosestPathSprite() + new Vector3(grid.GetCellSize() / 2, grid.GetCellSize() / 2);
        range = attackRange;
        for (int i = 0; i < maxSoldiers; i++)
        {
            SpawnSoldier();
        }
        StartCoroutine(SpawnSoldierRoutine());
    }

    protected override void Attack()
    {
        // Barracks don't attack directly, soldiers do the fighting
    }

    private Vector3 GetClosestPathSprite()
    {
        float minDistance = Mathf.Infinity;
        grid.GetXY(transform.position, out int x, out int y);
        int a = 0, b = 0;
        for(int i = x - 2; i < x + 2; i++)
        {
            for(int j = y - 2; j < y + 2; j++)
            {
                if(i ==x && j == y)
                {
                    continue;
                }
                if (grid.GetValue(i, j) == 1)
                {
                    if(Vector3.Distance(transform.position, grid.GetWorldPosition(i, j)) < minDistance)
                    {
                        minDistance = Vector3.Distance(transform.position, grid.GetWorldPosition(i, j));
                        a = i;
                        b = j;
                    }
                }
            }
        }
        return grid.GetWorldPosition(a,b);
    }
    private IEnumerator SpawnSoldierRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnCooldown);
            if (activeSoldiers.Count < maxSoldiers)
            {
                SpawnSoldier();
            }
        }
    }

    private void SpawnSoldier()
    {
        Vector2 randomCircle = Random.insideUnitCircle * groupRadius;
        Vector3 spawnOffset = new Vector3(randomCircle.x, randomCircle.y, 0);
        Vector3 soldierSpawnPoint = groupPosition + spawnOffset;
        Transform soldierObj = Soldier.Create(soldierSpawnPoint, this, soldierDamage, soldierHealth, groupPosition);
        if (soldierObj.TryGetComponent<Soldier>(out var soldier))
        {
            activeSoldiers.Add(soldier);
            
        }
    }

    public List<Soldier> GetActiveSoldiers()
    {
        return activeSoldiers;
    }

    public void RemoveSoldier(Soldier soldier)
    {
        activeSoldiers.Remove(soldier);
    }

    public override void UpgradeTower()
    {
        base.UpgradeTower();
        maxSoldiers += 1;
        soldierDamage += 5f * level;
        soldierHealth += 15f * level;
    }
}
