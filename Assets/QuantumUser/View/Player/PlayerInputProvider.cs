namespace Quantum
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Zenject;

    public class PlayerInputProvider : MonoBehaviour
    {
        private InputSystem_Actions inputActions;
        private PlayerInput.IPlayerActions playerActions;
        
        private Vector2 movementInput;
        private Vector2 lookInput;
        private bool jumpPressed;
        private bool jumpHeld;

        [Inject]
        public void Construct(InputSystem_Actions actions)
        {
            inputActions = actions;
        }

        private void Awake()
        {
            if (inputActions == null)
            {
                inputActions = new InputSystem_Actions();
            }
            
            playerActions = inputActions.Player;
        }

        private void OnEnable()
        {
            inputActions.Enable();
            
            playerActions.Move.performed += OnMove;
            playerActions.Move.canceled += OnMove;
            
            playerActions.Look.performed += OnLook;
            playerActions.Look.canceled += OnLook;
            
            playerActions.Jump.performed += OnJumpPerformed;
            playerActions.Jump.canceled += OnJumpCanceled;
        }

        private void OnDisable()
        {
            playerActions.Move.performed -= OnMove;
            playerActions.Move.canceled -= OnMove;
            
            playerActions.Look.performed -= OnLook;
            playerActions.Look.canceled -= OnLook;
            
            playerActions.Jump.performed -= OnJumpPerformed;
            playerActions.Jump.canceled -= OnJumpCanceled;
            
            inputActions.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            movementInput = context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            jumpPressed = true;
            jumpHeld = true;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            jumpHeld = false;
        }

        public void PollInput(Input* input)
        {
            input->Movement = movementInput.ToFPVector2();
            input->Look = lookInput.ToFPVector2();
            
            if (jumpPressed)
            {
                input->Jump.Set();
                jumpPressed = false;
            }
            else if (jumpHeld)
            {
                input->Jump.Set();
            }
            else
            {
                input->Jump.Clear();
            }
        }
    }
}
