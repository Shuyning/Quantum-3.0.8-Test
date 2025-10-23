namespace Quantum
{
    using UnityEngine;
    using Cinemachine;
    using Zenject;

    public class PlayerCameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        
        [Header("Rotation Settings")]
        [SerializeField] private float mouseSensitivity = 2.0f;
        [SerializeField] private float gamepadSensitivity = 100.0f;
        [SerializeField] private float minVerticalAngle = -40f;
        [SerializeField] private float maxVerticalAngle = 70f;
        [SerializeField] private bool invertY = false;

        private PlayerInputProvider inputProvider;
        private float currentVerticalAngle;
        private float currentHorizontalAngle;
        private QuantumEntityView entityView;

        [Inject]
        public void Construct(PlayerInputProvider provider)
        {
            inputProvider = provider;
        }

        private void Awake()
        {
            entityView = GetComponent<QuantumEntityView>();
            
            if (cameraTarget == null)
            {
                GameObject targetObj = new GameObject("CameraTarget");
                targetObj.transform.SetParent(transform);
                targetObj.transform.localPosition = new Vector3(0, 1.6f, 0);
                cameraTarget = targetObj.transform;
            }

            if (virtualCamera == null)
            {
                virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            }

            if (virtualCamera != null)
            {
                virtualCamera.Follow = cameraTarget;
                virtualCamera.LookAt = cameraTarget;
            }
        }

        private void Start()
        {
            currentHorizontalAngle = transform.eulerAngles.y;
            currentVerticalAngle = 0f;
        }

        private void LateUpdate()
        {
            if (inputProvider == null || cameraTarget == null)
                return;

            UpdateCameraRotation();
            UpdateQuantumCameraReference();
        }

        private void UpdateCameraRotation()
        {
            Vector2 lookInput = Vector2.zero;
            
            if (inputProvider != null)
            {
                lookInput = inputProvider.GetLookInput();
            }

            float sensitivity = Mathf.Abs(lookInput.x) > 1f || Mathf.Abs(lookInput.y) > 1f 
                ? mouseSensitivity 
                : gamepadSensitivity;

            float horizontalDelta = lookInput.x * sensitivity * Time.deltaTime;
            float verticalDelta = lookInput.y * sensitivity * Time.deltaTime;

            if (invertY)
            {
                verticalDelta = -verticalDelta;
            }

            currentHorizontalAngle += horizontalDelta;
            currentVerticalAngle -= verticalDelta;
            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);

            cameraTarget.rotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0f);
        }

        private void UpdateQuantumCameraReference()
        {
            if (entityView == null || !entityView.EntityRef.IsValid)
                return;

            var game = QuantumRunner.Default?.Game;
            if (game == null)
                return;

            var frame = game.Frames.Predicted;
            if (frame == null)
                return;

            if (frame.TryGet<CameraReference>(entityView.EntityRef, out var cameraRef))
            {
                Vector3 forward = cameraTarget.forward;
                Vector3 right = cameraTarget.right;

                cameraRef.Forward = forward.ToFPVector3();
                cameraRef.Right = right.ToFPVector3();

                frame.Set(entityView.EntityRef, cameraRef);
            }
        }

        public Transform GetCameraTarget()
        {
            return cameraTarget;
        }

        public void SetSensitivity(float mouse, float gamepad)
        {
            mouseSensitivity = mouse;
            gamepadSensitivity = gamepad;
        }
    }
}
