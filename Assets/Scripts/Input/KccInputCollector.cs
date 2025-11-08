using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QuantumTest
{
    public class KccInputCollector : MonoBehaviour, IInputProvider
    {
        [SerializeField] private float lookSensitivity = 0.1f;
        
        private InputSystem_Actions _actions;
        private Vector2 _move;
        private Vector2 _lookDelta;
        
        private bool _jumpThisFrame;
        
        public Vector2 LookDelta => _lookDelta;
        
        private void Awake()
        {
            _actions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _actions.Enable();

            _actions.Player.Move.performed += OnMove;
            _actions.Player.Move.canceled += OnMove;
            _actions.Player.Jump.performed += OnJumpPerformed;
            _actions.Player.Look.performed += OnLook;
            _actions.Player.Look.canceled += OnLook;

            QuantumCallback.Subscribe(this, (CallbackPollInput c) => PollInput(c));
        }

        private void OnDisable()
        {
            _actions.Player.Move.performed -= OnMove;
            _actions.Player.Move.canceled -= OnMove;
            _actions.Player.Jump.performed -= OnJumpPerformed;
            _actions.Player.Look.performed -= OnLook;
            _actions.Player.Look.canceled -= OnLook;

            _actions.Disable();
            QuantumCallback.UnsubscribeListener(this);
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            _move = ctx.ReadValue<Vector2>();
        }

        private void OnJumpPerformed(InputAction.CallbackContext ctx)
        {
            _jumpThisFrame = true;
        }

        private void OnLook(InputAction.CallbackContext ctx)
        {
            var delta = ctx.ReadValue<Vector2>();
            _lookDelta = delta;

            if (_lookDelta.sqrMagnitude <= float.Epsilon)
                return;
            
            _lookDelta.x *= lookSensitivity;
            _lookDelta.y *= lookSensitivity;
        }

        private void PollInput(CallbackPollInput callback)
        {
            KccPlayerInput input = default;
            input.MoveDirection = _move.ToFPVector2();

            if (_lookDelta.sqrMagnitude > 0.0f)
            {
                var yaw = (_lookDelta.x).ToFP();
                var pitch = (-_lookDelta.y).ToFP();
                input.Yaw = yaw;
                input.Pitch = pitch;
            }

            if (_jumpThisFrame)
                input.Jump = true;
            
            callback.SetInput(input, DeterministicInputFlags.Repeatable);
            _jumpThisFrame = false;
            _lookDelta = Vector2.zero;
        }
    }
}