namespace Quantum 
{
    using Photon.Deterministic;

    public unsafe class KccPlayerInputSystem : SystemMainThreadFilter<KccPlayerInputSystem.Filter> 
    {
        public struct Filter 
        {
            public EntityRef Entity;
            public KCC* KCC;
            public PlayerController* PlayerController;
        }

        public override void Update(Frame f, ref Filter filter) 
        {
            var playerRef = filter.PlayerController->PlayerRef;
            KccPlayerInput input = *f.GetPlayerInput(playerRef);
            
            FP pitchDelta = input.Pitch;
            FP yawDelta = input.Yaw;
            if (pitchDelta != FP._0 || yawDelta != FP._0)
                filter.KCC->AddLookRotation(pitchDelta, yawDelta);

            filter.KCC->SetInputDirection(filter.KCC->Data.TransformRotation * input.MoveDirection.XOY);
            
            if (input.Jump && filter.KCC->Data.IsGrounded) 
            {
                  FP gravity = -filter.KCC->Data.Gravity.Y;
                  if (gravity <= FP._0) 
                      gravity = 20;

                  FP jumpHeight = FP._2;
                  if (f.Unsafe.TryGetPointer<KccJumpConfig>(filter.Entity, out var cfg) && cfg->JumpHeight > FP._0) {
                      jumpHeight = cfg->JumpHeight;
                  }

                  FP force = FPMath.Sqrt((FP)2 * gravity * jumpHeight);
                  filter.KCC->Jump(new FPVector3(FP._0, force, FP._0));
            }
        }
    }
}
