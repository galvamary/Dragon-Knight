# Enemy Patrol System Design

**Date:** 2026-07-06  
**Status:** Approved  
**Scope:** 적 순찰(Patrol) 상태 구현 — 추후 추격/공격 상태 확장 기반 포함

---

## 개요

드래곤 나이트의 기본 적 AI 첫 번째 단계. 적은 스폰 위치를 중심으로 정해진 반지름 안에서 왕복 순찰한다.  
나중에 "플레이어 감지 → 추격 → 공격" 상태를 추가하기 위해 상태 패턴(State Pattern) 기반으로 설계한다.

---

## 파일 구조

```
DragonKnight/Assets/Scripts/Enemy/
├── Enemy.cs              ← 메인 컨트롤러
├── IEnemyState.cs        ← 상태 인터페이스
└── EnemyPatrolState.cs   ← 순찰 상태 구현
```

---

## 컴포넌트 설계

### IEnemyState.cs

상태 인터페이스. 모든 상태가 구현한다.

```
Enter()       — 상태 진입 시 초기화
Update()      — 매 프레임 로직
FixedUpdate() — 물리 연산
Exit()        — 상태 종료 시 정리
```

### Enemy.cs

- `RequireComponent(Rigidbody2D)`
- `Awake()`에서 `patrolCenter = transform.position` 기록
- `currentState` 보유, `ChangeState(IEnemyState)` 메서드로 전환
- `Update()` / `FixedUpdate()`를 현재 상태에 위임
- Inspector 노출 필드:

| 필드 | 타입 | 기본값 | 설명 |
|------|------|--------|------|
| `patrolSpeed` | float | 2f | 순찰 이동 속도 |
| `patrolRadius` | float | 4f | 스폰 위치 기준 순찰 반지름 |
| `groundLayer` | LayerMask | - | 지면 레이어 마스크 |
| `ledgeCheckDistance` | float | 0.6f | 낙하 감지 Raycast 길이 |
| `ledgeCheckOffset` | float | 0.3f | 발 앞쪽 오프셋 (낙하 감지 시작점) |

- `OnDrawGizmosSelected()`에서 순찰 범위 원 시각화

### EnemyPatrolState.cs

- `Enemy` 참조를 생성자로 받음
- `Enter()`: 이동 방향 초기화 (+1 또는 -1)
- `FixedUpdate()`:
  1. `rb.linearVelocity.x = direction * patrolSpeed` (Y는 유지)
  2. **반전 조건 A** — 스폰 위치로부터 수평 거리 > `patrolRadius`
  3. **반전 조건 B** — 진행 방향 발 앞 아래로 Raycast → groundLayer 미검출
  4. 반전 시 `direction *= -1`, `transform.localScale.x *= -1`

---

## 물리 동작

- Rigidbody2D 기본 중력 적용 (플레이어와 동일)
- `freezeRotation = true`
- `FixedUpdate`에서 수평 velocity만 조작, Y velocity는 건드리지 않음

---

## Gizmos 시각화

- `OnDrawGizmosSelected()`: 스폰 위치 중심 반지름 `patrolRadius` 원 (노란색)
- 낙하 감지 Raycast 방향 선 (빨간색)

---

## 확장 계획 (이번 구현 범위 밖)

| 상태 | 트리거 | 파일 |
|------|--------|------|
| ChaseState | 플레이어가 감지 범위 진입 | EnemyChaseState.cs |
| AttackState | 플레이어가 공격 범위 진입 | EnemyAttackState.cs |

---

## 구현 순서

1. `IEnemyState.cs` 작성
2. `Enemy.cs` 작성 (상태 관리 + Inspector 필드 + Gizmos)
3. `EnemyPatrolState.cs` 작성
4. Unity 씬에서 Enemy GameObject 구성 및 테스트
