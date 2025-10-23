namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerInputSystem : SystemMainThreadFilter<PlayerInputSystem.Filter>
    {
        public override void Update(Frame frame, ref Filter filter)
        {
            var input = frame.GetPlayerInput(filter.PlayerLink->PlayerRef);
            
            if (input == null)
                return;

            var playerInput = (Input*)input;
            
            filter.PlayerInput->MovementInput = playerInput->Movement;
            filter.PlayerInput->LookInput = playerInput->Look;
            filter.PlayerInput->JumpPressed = playerInput->Jump.WasPressed;
            filter.PlayerInput->JumpHeld = playerInput->Jump.IsDown;
        }

        public struct Filter
        {
            public EntityRef Entity;
            public PlayerLink* PlayerLink;
            public PlayerInput* PlayerInput;
        }
    }
}
