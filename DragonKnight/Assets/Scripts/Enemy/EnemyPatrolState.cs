using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    private readonly Enemy enemy;
    private float direction = 1f;

    public EnemyPatrolState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        direction = 1f;
        enemy.transform.localScale = new Vector3(direction, 1f, 1f);
    }

    public void Update() { }

    public void FixedUpdate()
    {
        if (ShouldReverse())
        {
            direction *= -1f;
            enemy.transform.localScale = new Vector3(direction, 1f, 1f);
        }

        enemy.Rb.linearVelocity = new Vector2(
            direction * enemy.PatrolSpeed,
            enemy.Rb.linearVelocity.y
        );
    }

    public void Exit() { }

    private bool ShouldReverse()
    {
        // 반전 조건 A: 순찰 반지름 초과
        float distanceX = enemy.transform.position.x - enemy.PatrolCenter.x;
        if (Mathf.Abs(distanceX) >= enemy.PatrolRadius)
            return true;

        // 반전 조건 B: 발 앞 낙하 감지
        Vector2 ledgeOrigin = (Vector2)enemy.transform.position
            + new Vector2(direction * enemy.LedgeCheckOffset, 0f);
        bool hasGround = Physics2D.Raycast(
            ledgeOrigin,
            Vector2.down,
            enemy.LedgeCheckDistance,
            enemy.GroundLayer
        );
        return !hasGround;
    }
}
