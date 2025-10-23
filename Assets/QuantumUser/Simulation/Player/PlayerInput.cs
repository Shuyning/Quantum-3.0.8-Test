namespace Quantum
{
    using Photon.Deterministic;

    public unsafe partial struct PlayerInput
    {
        public FPVector2 MovementInput;
        public FPVector2 LookInput;
        public bool JumpPressed;
        public bool JumpHeld;
    }
}
