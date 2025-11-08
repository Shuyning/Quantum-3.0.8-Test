using Quantum;
using QuantumTest.Contexts;
using Unity.Cinemachine;
using UnityEngine;

namespace QuantumTest
{
    public class PlayerCinemachineController : QuantumSceneViewComponent<ScenePlayerInputContext>
    {
        [SerializeField] private CinemachineCamera virtualCamera;
        [SerializeField] private QuantumEntityView playerView;
        [SerializeField] private Transform followRoot;
        [Space]
        [SerializeField] private float minPitch = -60f;
        [SerializeField] private float maxPitch = 75f;
        [SerializeField] private int localPriority = 20;
        [SerializeField] private bool disableWhenNotLocal = true;

        private int _defaultPriority;
        private float _visualYaw;
        private float _visualPitch;

        private void Awake()
        {
            if (virtualCamera != null)
                _defaultPriority = virtualCamera.Priority;
        }
        
        public override void OnUpdateView()
        {
            if (virtualCamera == null || playerView == null)
                return;

            var game = Game;
            var entity = playerView.EntityRef;

            bool isLocal = false;
            var verifiedFrame = VerifiedFrame;
            if (verifiedFrame != null && verifiedFrame.TryGet<PlayerController>(entity, out var playerController))
                isLocal = game.PlayerIsLocal(playerController.PlayerRef);

            if (disableWhenNotLocal)
            {
                SetActive(isLocal);
                
                if (!isLocal)
                    return;
            }
               
            virtualCamera.Priority = isLocal 
                ? localPriority 
                : _defaultPriority;

            var delta = ViewContext.InputProvider.LookDelta;
            if (delta.sqrMagnitude > 0f)
            {
                _visualYaw += delta.x;
                _visualPitch -= delta.y;
            }
            _visualPitch = Mathf.Clamp(_visualPitch, minPitch, maxPitch);

            if (followRoot != null)
                followRoot.rotation = Quaternion.Euler(_visualPitch, _visualYaw, 0f);
        }

        private void SetActive(bool value)
        {
            if (virtualCamera != null)
                virtualCamera.enabled = value;
        }
    }
}
