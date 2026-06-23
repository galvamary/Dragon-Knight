using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement movement;
    private PlayerCombat combat;

    // Animator 파라미터 이름을 해시로 캐싱
    private static readonly int HashSpeed = Animator.StringToHash("Speed");
    private static readonly int HashIsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int HashVelocityY = Animator.StringToHash("VelocityY");
    private static readonly int HashIsDashing = Animator.StringToHash("IsDashing");
    private static readonly int HashAttack = Animator.StringToHash("Attack");

    private bool wasAttacking;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        animator.SetFloat(HashSpeed, movement.MoveSpeed);
        animator.SetBool(HashIsGrounded, movement.IsGrounded);
        animator.SetFloat(HashVelocityY, movement.VelocityY);
        animator.SetBool(HashIsDashing, movement.IsDashing);

        // 공격 시작 순간에 트리거 발동
        bool attacking = combat.IsAttacking;
        if (attacking && !wasAttacking)
            animator.SetTrigger(HashAttack);
        wasAttacking = attacking;
    }
}
