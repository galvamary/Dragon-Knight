using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolRadius = 4f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float ledgeCheckDistance = 0.6f;
    [SerializeField] private float ledgeCheckOffset = 0.3f;

    public float PatrolSpeed => patrolSpeed;
    public float PatrolRadius => patrolRadius;
    public LayerMask GroundLayer => groundLayer;
    public float LedgeCheckDistance => ledgeCheckDistance;
    public float LedgeCheckOffset => ledgeCheckOffset;
    public Vector2 PatrolCenter { get; private set; }
    public Rigidbody2D Rb { get; private set; }

    private IEnemyState currentState;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Rb.freezeRotation = true;
        PatrolCenter = transform.position;
    }

    private void Start()
    {
        ChangeState(new EnemyPatrolState(this));
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void Update()
    {
        currentState?.Update();
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    private void OnDrawGizmosSelected()
    {
        // 순찰 범위 원 (노란색)
        Gizmos.color = Color.yellow;
        Vector3 center = Application.isPlaying ? (Vector3)PatrolCenter : transform.position;
        Gizmos.DrawWireSphere(center, patrolRadius);

        // 낙하 감지 Raycast 시각화 (빨간색, 양방향)
        Gizmos.color = Color.red;
        for (int dir = -1; dir <= 1; dir += 2)
        {
            Vector3 origin = transform.position + new Vector3(dir * ledgeCheckOffset, 0f, 0f);
            Gizmos.DrawLine(origin, origin + Vector3.down * ledgeCheckDistance);
        }
    }
}
