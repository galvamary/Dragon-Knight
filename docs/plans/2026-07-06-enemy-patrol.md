# Enemy Patrol Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 스폰 위치 기준 반지름 안에서 왕복 순찰하는 적 AI 기반 구조 구현

**Architecture:** 상태 패턴(State Pattern) 기반. `Enemy.cs`가 Rigidbody2D와 Inspector 필드를 보유하고 현재 상태(`IEnemyState`)에 Update/FixedUpdate를 위임한다. `EnemyPatrolState`가 수평 이동, 거리 반전, 낙하 감지 반전을 처리한다.

**Tech Stack:** Unity 6 (6000.4.3f1), C#, Rigidbody2D, Physics2D.Raycast, URP 2D

---

## 파일 맵

| 경로 | 역할 |
|------|------|
| `DragonKnight/Assets/Scripts/Enemy/IEnemyState.cs` | 상태 인터페이스 (Enter/Update/FixedUpdate/Exit) |
| `DragonKnight/Assets/Scripts/Enemy/Enemy.cs` | 메인 컨트롤러, Inspector 필드, Gizmos, 상태 전환 |
| `DragonKnight/Assets/Scripts/Enemy/EnemyPatrolState.cs` | 순찰 로직 (이동, 반전 조건 2가지) |

---

## Task 1: IEnemyState 인터페이스 작성

**Files:**
- Create: `DragonKnight/Assets/Scripts/Enemy/IEnemyState.cs`

- [ ] **Step 1: Enemy 폴더 생성 확인 후 IEnemyState.cs 작성**

```csharp
public interface IEnemyState
{
    void Enter();
    void Update();
    void FixedUpdate();
    void Exit();
}
```

- [ ] **Step 2: Unity 에디터에서 컴파일 오류 없음 확인**

  Console 창 열기 → 오류 없으면 OK. (Unity가 자동으로 컴파일)

- [ ] **Step 3: 커밋**

```bash
git add DragonKnight/Assets/Scripts/Enemy/IEnemyState.cs DragonKnight/Assets/Scripts/Enemy/IEnemyState.cs.meta
git commit -m "Add IEnemyState interface"
```

---

## Task 2: Enemy 메인 컨트롤러 작성

**Files:**
- Create: `DragonKnight/Assets/Scripts/Enemy/Enemy.cs`

- [ ] **Step 1: Enemy.cs 작성**

```csharp
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
```

- [ ] **Step 2: Unity 에디터에서 컴파일 오류 없음 확인**

  Console 창 → 오류 없으면 OK.

- [ ] **Step 3: 커밋**

```bash
git add DragonKnight/Assets/Scripts/Enemy/Enemy.cs DragonKnight/Assets/Scripts/Enemy/Enemy.cs.meta
git commit -m "Add Enemy controller with state machine and Gizmos"
```

---

## Task 3: EnemyPatrolState 작성

**Files:**
- Create: `DragonKnight/Assets/Scripts/Enemy/EnemyPatrolState.cs`

- [ ] **Step 1: EnemyPatrolState.cs 작성**

```csharp
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

        // 반전 조건 B: 발 앞 낙하 감지 (Raycast 아래로)
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
```

- [ ] **Step 2: Unity 에디터에서 컴파일 오류 없음 확인**

  Console 창 → 오류 없으면 OK.

- [ ] **Step 3: 커밋**

```bash
git add DragonKnight/Assets/Scripts/Enemy/EnemyPatrolState.cs DragonKnight/Assets/Scripts/Enemy/EnemyPatrolState.cs.meta
git commit -m "Add EnemyPatrolState with distance and ledge reversal"
```

---

## Task 4: Unity 씬에서 Enemy GameObject 구성 및 검증

**Files:**
- Modify: `DragonKnight/Assets/Scenes/SampleScene.unity` (Unity 에디터에서 직접)

- [ ] **Step 1: Enemy GameObject 생성**

  Unity 에디터 → Hierarchy 우클릭 → Create Empty → 이름 `Enemy`

- [ ] **Step 2: 컴포넌트 추가**

  Inspector에서 `Add Component` → `Enemy` 스크립트 추가
  (`RequireComponent`에 의해 `Rigidbody2D`가 자동 추가됨)

- [ ] **Step 3: Inspector 설정**

  | 필드 | 값 |
  |------|----|
  | Patrol Speed | 2 |
  | Patrol Radius | 4 |
  | Ground Layer | Ground (프로젝트에서 사용 중인 레이어 선택) |
  | Ledge Check Distance | 0.6 |
  | Ledge Check Offset | 0.3 |

  Rigidbody2D → Freeze Rotation Z 체크 확인 (코드에서 자동 설정되지만 확인)

- [ ] **Step 4: Enemy 선택 → 씬 뷰에서 Gizmos 확인**

  씬 뷰에서 노란 원(순찰 범위)과 빨간 선(낙하 감지)이 표시되면 OK.

- [ ] **Step 5: 스프라이트 또는 임시 표시용 컴포넌트 추가 (선택)**

  Hierarchy → Enemy → Add Component → `Sprite Renderer` → 임시 스프라이트 할당
  (없으면 빈 오브젝트로도 동작 확인 가능)

- [ ] **Step 6: Play Mode에서 순찰 동작 검증**

  체크 항목:
  - 적이 한 방향으로 이동하는가
  - 순찰 반지름 끝에서 방향이 반전되는가
  - 플랫폼 끝에서 낙하하지 않고 반전되는가 (플랫폼 끝에 배치 후 확인)
  - 스프라이트가 이동 방향에 맞게 flip되는가

- [ ] **Step 7: 씬 저장 및 커밋**

  Unity: Ctrl+S (씬 저장)

```bash
git add DragonKnight/Assets/Scenes/SampleScene.unity
git commit -m "Add Enemy patrol GameObject to SampleScene"
```

---

## 완료 조건

- [ ] 적이 스폰 위치 기준 설정된 반지름 안에서 왕복 이동
- [ ] 반지름 끝에서 자동 방향 반전 및 스프라이트 flip
- [ ] 플랫폼 끝(낙하 예상 지점)에서 자동 방향 반전
- [ ] 씬 뷰 Gizmos에서 순찰 범위 원 시각화
- [ ] 컴파일 오류 없음
