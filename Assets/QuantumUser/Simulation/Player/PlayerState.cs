namespace Quantum
{
    using Photon.Deterministic;

    public enum PlayerMovementState
    {
        Idle,
        Walking,
        Sprinting,
        Jumping,
        Falling,
        Landing
    }

    public unsafe partial struct PlayerState
    {
        public PlayerMovementState MovementState;
        public FP CurrentSpeed;
        public FP TimeInAir;
        public bool WasGroundedLastFrame;
    }
}
