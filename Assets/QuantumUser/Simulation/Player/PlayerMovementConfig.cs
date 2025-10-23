namespace Quantum
{
    using Photon.Deterministic;

    public unsafe partial struct PlayerMovementConfig
    {
        public FP WalkSpeed;
        public FP SprintSpeed;
        public FP JumpImpulse;
        public FP AirControl;
        public FP GroundAcceleration;
        public FP GroundDeceleration;
        public FP AirAcceleration;
        public FP RotationSpeed;
    }
}
