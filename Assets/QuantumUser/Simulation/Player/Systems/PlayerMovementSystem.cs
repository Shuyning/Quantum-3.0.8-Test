namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerMovementSystem : SystemMainThreadFilter<PlayerMovementSystem.Filter>
    {
        public override void Update(Frame frame, ref Filter filter)
        {
            var kcc = filter.KCC;
            var input = filter.PlayerInput;
            var config = filter.PlayerMovementConfig;
            var state = filter.PlayerState;
            var transform = filter.Transform;

            bool wasGrounded = kcc->IsGrounded;
            state->WasGroundedLastFrame = wasGrounded;

            FPVector2 moveInput = input->MovementInput;
            FP moveSpeed = config->WalkSpeed;

            if (moveInput.SqrMagnitude > FP._0)
            {
                FPVector3 cameraForward = FPVector3.Forward;
                FPVector3 cameraRight = FPVector3.Right;
                
                if (frame.TryGet<CameraReference>(filter.Entity, out var cameraRef))
                {
                    cameraForward = cameraRef.Forward;
                    cameraRight = cameraRef.Right;
                }

                cameraForward.Y = FP._0;
                cameraForward = FPVector3.Normalize(cameraForward);
                cameraRight.Y = FP._0;
                cameraRight = FPVector3.Normalize(cameraRight);

                FPVector3 moveDirection = (cameraForward * moveInput.Y + cameraRight * moveInput.X);
                moveDirection = FPVector3.Normalize(moveDirection);

                kcc->SetInputDirection(moveDirection);

                if (moveDirection.SqrMagnitude > FP._0_01)
                {
                    FPQuaternion targetRotation = FPQuaternion.LookRotation(moveDirection);
                    transform->Rotation = FPQuaternion.Slerp(transform->Rotation, targetRotation, config->RotationSpeed * frame.DeltaTime);
                }

                state->CurrentSpeed = kcc->RealSpeed;
                state->MovementState = PlayerMovementState.Walking;
            }
            else
            {
                kcc->SetInputDirection(FPVector3.Zero);
                state->CurrentSpeed = FP._0;
                state->MovementState = PlayerMovementState.Idle;
            }

            if (input->JumpPressed && kcc->IsGrounded)
            {
                kcc->Jump(config->JumpImpulse);
                state->MovementState = PlayerMovementState.Jumping;
                state->TimeInAir = FP._0;
            }

            if (!kcc->IsGrounded)
            {
                state->TimeInAir += frame.DeltaTime;
                
                if (kcc->Data.DynamicVelocity.Y < -FP._1)
                {
                    state->MovementState = PlayerMovementState.Falling;
                }
            }
            else
            {
                if (!wasGrounded && state->TimeInAir > FP._0_10)
                {
                    state->MovementState = PlayerMovementState.Landing;
                }
                
                state->TimeInAir = FP._0;
            }
        }

        public struct Filter
        {
            public EntityRef Entity;
            public KCC* KCC;
            public PlayerInput* PlayerInput;
            public PlayerMovementConfig* PlayerMovementConfig;
            public PlayerState* PlayerState;
            public Transform3D* Transform;
        }
    }
}
