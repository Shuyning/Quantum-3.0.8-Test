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
            var input = f.GetPlayerInput(playerRef);
            if (input == null)
                return;
            
            FPVector2 move = input->ThumbSticks.HighRes->_leftThumb;
            if (move == default) 
            {
                  if (input->IsDown(InputButtons._left))
                      move.X -= FP._1;
                  if (input->IsDown(InputButtons._right))
                      move.X += FP._1;
                  if (input->IsDown(InputButtons._down))
                      move.Y -= FP._1;
                  if (input->IsDown(InputButtons._up))
                      move.Y += FP._1;
                  if (move.SqrMagnitude > FP._1)
                      move = move.Normalized;
            }
            
            FP pitchDelta = input->ThumbSticks.HighRes->_rightThumb.Pitch;
            FP yawDelta = input->ThumbSticks.HighRes->_rightThumb.Yaw;
            if (pitchDelta != FP._0 || yawDelta != FP._0)
                filter.KCC->AddLookRotation(pitchDelta, yawDelta);

            FP yaw = filter.KCC->Data.LookYaw;
            FP sy = FPMath.Sin(yaw);
            FP cy = FPMath.Cos(yaw);
            FPVector3 forward = new FPVector3(sy, FP._0, cy);
            FPVector3 right   = new FPVector3(cy, FP._0, -sy);
            FPVector3 worldDir = right * move.X + forward * move.Y;

            filter.KCC->SetInputDirection(worldDir);
            
            if (input->WasPressed(InputButtons._a) && filter.KCC->Data.IsGrounded) 
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
