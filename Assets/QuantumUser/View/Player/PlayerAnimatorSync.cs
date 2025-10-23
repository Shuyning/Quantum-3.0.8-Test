namespace Quantum
{
    using UnityEngine;

    public class PlayerAnimatorSync : MonoBehaviour
    {
        [SerializeField] private Animator unityAnimator;
        private QuantumEntityView entityView;

        private void Awake()
        {
            entityView = GetComponent<QuantumEntityView>();
            
            if (unityAnimator == null)
            {
                unityAnimator = GetComponentInChildren<Animator>();
            }
        }

        private void Update()
        {
            if (entityView == null || !entityView.EntityRef.IsValid || unityAnimator == null)
                return;

            var game = QuantumRunner.Default?.Game;
            if (game == null)
                return;

            var frame = game.Frames.Predicted;
            if (frame == null)
                return;

            if (frame.TryGet<PlayerState>(entityView.EntityRef, out var playerState))
            {
                SyncAnimatorParameters(playerState);
            }

            if (frame.TryGet<KCC>(entityView.EntityRef, out var kcc))
            {
                unityAnimator.SetBool("IsGrounded", kcc.IsGrounded);
            }
        }

        private void SyncAnimatorParameters(PlayerState playerState)
        {
            float speed = playerState.CurrentSpeed.AsFloat;
            unityAnimator.SetFloat("MoveSpeed", speed);
            unityAnimator.SetBool("Locomotion", speed > 0.1f);

            switch (playerState.MovementState)
            {
                case PlayerMovementState.Jumping:
                    unityAnimator.SetTrigger("IsJump");
                    break;
                case PlayerMovementState.Falling:
                    unityAnimator.SetBool("IsFall", true);
                    break;
                default:
                    unityAnimator.SetBool("IsFall", false);
                    break;
            }
        }
    }
}
