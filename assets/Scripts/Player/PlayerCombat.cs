using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private float attackCooldown = 0.4f;
    [SerializeField] private int attackDamage = 1;

    [Header("Hitbox")]
    [SerializeField] private Collider2D hitboxCollider;
    [SerializeField] private LayerMask enemyLayer;

    private PlayerMovement movement;
    private float cooldownTimer;
    private bool isAttacking;
    public bool IsAttacking => isAttacking;
    private float attackDuration = 0.1f;
    private float attackTimer;

    private readonly List<Collider2D> hitResults = new();
    private ContactFilter2D contactFilter;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        contactFilter.SetLayerMask(enemyLayer);
        contactFilter.useLayerMask = true;
        contactFilter.useTriggers = true;

        if (hitboxCollider != null)
            hitboxCollider.enabled = false;
    }

    private void OnEnable()
    {
        attackAction.action.Enable();
        attackAction.action.started += OnAttackPressed;
    }

    private void OnDisable()
    {
        attackAction.action.started -= OnAttackPressed;
        attackAction.action.Disable();
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
                hitboxCollider.enabled = false;
            }
        }
    }

    private void OnAttackPressed(InputAction.CallbackContext context)
    {
        if (cooldownTimer > 0f || movement.IsDashing)
            return;

        Attack();
    }

    private void Attack()
    {
        isAttacking = true;
        attackTimer = attackDuration;
        cooldownTimer = attackCooldown;
        hitboxCollider.enabled = true;

        // 히트박스 영역 내 적 검출
        int hitCount = hitboxCollider.Overlap(contactFilter, hitResults);

        for (int i = 0; i < hitCount; i++)
        {
            // TODO: 적에게 데미지 전달
            Debug.Log($"Hit: {hitResults[i].gameObject.name} ({attackDamage} damage)");
        }
    }
}
