namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerAnimatorSystem : SystemMainThreadFilter<PlayerAnimatorSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public KCC* KCC;
            public AnimatorComponent* AnimatorComponent;
        }

        private const string MoveXParam = "MoveX";
        private const string MoveYParam = "MoveY";
        private const string MoveSpeedParam = "MoveSpeed";
        private const string IsFallParam = "IsFall";
        private const string IsJumpParam = "IsJump";
        private const string LocomotionParam = "Locomotion";

        public override void Update(Frame f, ref Filter filter)
        {
            var kccData = filter.KCC->Data;
            var velocity = kccData.RealVelocity;
            var horizontalVelocity = new FPVector2(velocity.X, velocity.Z);
            var horizontalSpeed = horizontalVelocity.Magnitude;

            var isGrounded = kccData.IsGrounded;
            var minThreshold = FP._0_01;
            var isMoving = horizontalSpeed > minThreshold;

            var moveDirection = FPVector2.Zero;
            if (isMoving && isGrounded)
            {
                var forward = kccData.TransformRotation * FPVector3.Forward;
                var right = kccData.TransformRotation * FPVector3.Right;
                
                var forwardFlat = new FPVector2(forward.X, forward.Z).Normalized;
                var rightFlat = new FPVector2(right.X, right.Z).Normalized;
                
                var velocityFlat = new FPVector2(velocity.X, velocity.Z).Normalized;
                
                moveDirection.X = FPVector2.Dot(velocityFlat, forwardFlat);
                moveDirection.Y = FPVector2.Dot(velocityFlat, rightFlat);
            }

            AnimatorComponent.SetFixedPoint(f, filter.AnimatorComponent, MoveXParam, moveDirection.X);
            AnimatorComponent.SetFixedPoint(f, filter.AnimatorComponent, MoveYParam, moveDirection.Y);

            var fallThreshold = -FP._0_10;
            var isFalling = !isGrounded && velocity.Y < fallThreshold;
            AnimatorComponent.SetBoolean(f, filter.AnimatorComponent, IsFallParam, isFalling);

            var isLocomotion = isMoving || isFalling;
            AnimatorComponent.SetBoolean(f, filter.AnimatorComponent, LocomotionParam, isLocomotion);

            if (kccData.HasJumped)
            {
                AnimatorComponent.SetTrigger(f, filter.AnimatorComponent, IsJumpParam);
            }
        }
    }
}
