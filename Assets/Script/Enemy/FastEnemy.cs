using UnityEngine;

public class FastEnemy : Enemy
{
    public override float Speed => base.Speed * 1.5f;
    public override float MaxHealth => base.MaxHealth * 0.8f;

    protected override void Awake()
    {
        base.Awake();
        //GetComponent<SpriteRenderer>().color = Color.yellow;
    }
}