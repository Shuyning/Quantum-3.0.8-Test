namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerAnimationSystem : SystemMainThreadFilter<PlayerAnimationSystem.Filter>
    {
        public override void Update(Frame frame, ref Filter filter)
        {
            var animator = filter.AnimatorComponent;
            var state = filter.PlayerState;
            var kcc = filter.KCC;
            var input = filter.PlayerInput;

            FPVector2 moveInput = input->MovementInput;
            FP moveSpeed = state->CurrentSpeed;

            animator->SetFloat(frame, "MoveX", moveInput.X);
            animator->SetFloat(frame, "MoveY", moveInput.Y);
            animator->SetFloat(frame, "MoveSpeed", moveSpeed);

            animator->SetBoolean(frame, "IsGrounded", kcc->IsGrounded);
            animator->SetBoolean(frame, "Locomotion", moveSpeed > FP._0_10);

            bool isFalling = !kcc->IsGrounded && kcc->Data.DynamicVelocity.Y < -FP._1;
            animator->SetBoolean(frame, "IsFall", isFalling);

            if (state->MovementState == PlayerMovementState.Jumping)
            {
                animator->SetTrigger(frame, "IsJump");
            }
        }

        public struct Filter
        {
            public EntityRef Entity;
            public AnimatorComponent* AnimatorComponent;
            public PlayerState* PlayerState;
            public KCC* KCC;
            public PlayerInput* PlayerInput;
        }
    }
}
