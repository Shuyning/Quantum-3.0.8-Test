namespace Quantum
{
    using UnityEngine;

    public class LocalPlayerInputPoller : MonoBehaviour
    {
        private PlayerInputProvider inputProvider;
        private QuantumGame game;
        private PlayerRef localPlayerRef;

        private void Awake()
        {
            inputProvider = GetComponent<PlayerInputProvider>();
            if (inputProvider == null)
            {
                inputProvider = gameObject.AddComponent<PlayerInputProvider>();
            }
        }

        private void OnEnable()
        {
            QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
        }

        private void OnDisable()
        {
            QuantumCallback.UnsubscribeListener(this);
        }

        private void PollInput(CallbackPollInput callback)
        {
            if (inputProvider == null)
                return;

            Quantum.Input input = new Quantum.Input();
            
            unsafe
            {
                Quantum.Input* inputPtr = &input;
                inputProvider.PollInput(inputPtr);
            }

            callback.SetInput(input, DeterministicInputFlags.Repeatable);
        }

        public void SetLocalPlayer(PlayerRef playerRef)
        {
            localPlayerRef = playerRef;
        }
    }
}
