using CodeMonkey.Utils;
using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] protected Transform firePosition;
    [SerializeField] protected float damage = 25f;
    [SerializeField] protected float upgradeMoney = 25f;
    [SerializeField] public float towerCost = 50f;
    [SerializeField] protected float attackCoolDown = 0.5f;
    protected int level = 1;
    [SerializeField]
    protected float range = 100f;
    protected bool canAttack = true;
    protected Enemy targetEnemy;
    [Header("Sprites")] // Tower Sprites
    protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Sprite[] sprites;
    
    protected virtual void Awake()
    {
        //firePosition = transform.Find("FirePosition");
        //Debug.DrawLine(firePosition.position, firePosition.position + new Vector3(range, 0), Color.red, 100f);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public float GetRange()
    {
        return range;
    }

    protected virtual void Update()
    {
        if(canAttack)
        {
           Attack();
        }
    }

    protected virtual void Attack()
    {
        StartCoroutine(AttackCoolDown());
        if (targetEnemy != null && !targetEnemy.IsDead() && range >= Vector3.Distance(transform.position, targetEnemy.transform.position))
        {
            ProjectileArrow.Create(firePosition.position, targetEnemy, damage);
        }
        else
        {
            Enemy newTargetEnemy = GetClosestEnemy();
            if (newTargetEnemy != null)
            {
                targetEnemy = newTargetEnemy;
                ProjectileArrow.Create(firePosition.position, targetEnemy, damage);
            }
        }
    }

    protected Enemy GetClosestEnemy()
    {
        return Enemy.GetClosetEnemy(transform.position, range);
    }
    
    protected virtual IEnumerator AttackCoolDown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCoolDown);
        canAttack = true;
    }

    protected virtual void UpGradeRange(float ammount)
    {
        range += ammount;
    }
    protected virtual void UpGradeDamage(float ammount)
    {
        damage += ammount;
    }

    public void SellTower()
    {
        Destroy(gameObject);
    }

    public virtual void UpgradeTower()
    {
        Testing testingScript = FindObjectOfType<Testing>();
        if(testingScript.Money < upgradeMoney * level || level >=3)
        {
            return;
        }
        testingScript.Money -= upgradeMoney * level;
        level++;
        UpGradeDamage(5 * level);
        UpGradeRange(5 * level);
       
        ChangeSprite();
    }

    protected void ChangeSprite()
    {
        spriteRenderer.sprite = sprites[level - 1];
    }

    protected void OnMouseEnter()
    {
        UpgradeOverlay.Show_Static(this);
    }

}
