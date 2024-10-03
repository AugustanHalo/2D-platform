using CodeMonkey.Utils;
using UnityEngine;

public class ProjectileMagic  : MonoBehaviour
{
    [SerializeField]
    protected float speed = 50f;
    protected float damage;
    protected float distanceToEnemy;

    public static void Create(Vector3 spawnPosition, Enemy enemy, float damageAmount)
    {
        Transform magicTransform = Instantiate(GameAssets.i.pfProjectileMagic, spawnPosition, Quaternion.identity);

        ProjectileMagic projectileArrow = magicTransform.GetComponent<ProjectileMagic>();
        projectileArrow.Setup(enemy, damageAmount);
    }

    protected Enemy enemy;

    protected void Setup(Enemy enemy, float damageAmount)
    {
        this.enemy = enemy;
        this.damage = damageAmount;
    }

    protected virtual void Update()
    {
        if(enemy == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector3 targetPosition = enemy.transform.position;
        Vector3 moveDir = (targetPosition - transform.position).normalized;

        transform.position += moveDir * speed * Time.deltaTime;

        float moveAngle = UtilsClass.GetAngleFromVectorFloat(moveDir);
        transform.eulerAngles = new Vector3(0, 0, moveAngle - 90);

        distanceToEnemy = Vector3.Distance(transform.position, targetPosition);
        if ( distanceToEnemy < 1f)
        {
            enemy.TakeDamage(Random.Range(damage-5,damage+5));
            Destroy(gameObject);
        }
    }
}
