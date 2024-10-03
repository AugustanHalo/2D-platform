using UnityEngine;

public class TankEnemy : Enemy
{
    public override float Speed => base.Speed * 0.7f;
    public override float MaxHealth => base.MaxHealth * 2f;

    [SerializeField] private float damageReduction = 0.2f;

    protected override void Awake()
    {
        base.Awake();
        //GetComponent<SpriteRenderer>().color = Color.blue;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage * (1-damageReduction)); // 20% damage reduction
    }
}