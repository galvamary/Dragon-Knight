using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private readonly Enemy enemy;

    public EnemyAttackState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter() { }

    public void Update() { }

    public void FixedUpdate()
    {
        if (enemy.Player == null) return;

        float distX = enemy.Player.position.x - enemy.transform.position.x;
        float direction = Mathf.Sign(distX);

        // 스프라이트 방향 전환
        Vector3 scale = enemy.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        enemy.transform.localScale = scale;

        // 공격 사거리 안이면 멈춤 (공격 판정 추후 구현)
        if (Mathf.Abs(distX) <= enemy.AttackRange)
        {
            enemy.Rb.linearVelocity = new Vector2(0f, enemy.Rb.linearVelocity.y);
            return;
        }

        // 플레이어 방향으로 추격
        enemy.Rb.linearVelocity = new Vector2(direction * enemy.ChaseSpeed, enemy.Rb.linearVelocity.y);
    }

    public void Exit()
    {
        enemy.Rb.linearVelocity = new Vector2(0f, enemy.Rb.linearVelocity.y);
    }
}
