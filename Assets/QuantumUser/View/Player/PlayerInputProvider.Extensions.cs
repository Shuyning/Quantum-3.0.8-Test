namespace Quantum
{
    using UnityEngine;

    public partial class PlayerInputProvider
    {
        public Vector2 GetMovementInput()
        {
            return movementInput;
        }

        public Vector2 GetLookInput()
        {
            return lookInput;
        }

        public bool IsJumpPressed()
        {
            return jumpPressed;
        }

        public bool IsJumpHeld()
        {
            return jumpHeld;
        }
    }
}
