using QuantumTest.Matchmaking;
using UnityEngine;
using Zenject;

namespace QuantumTest.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class MatchmakingSearchScreen : MonoBehaviour, 
        IListener<MatchmakingSearchStarted>,
        IListener<MatchmakingError>
    {
        private CanvasGroup _canvasGroup;
        private IEmitterSubscribe _emitter;

        [Inject]
        public void Construct(IEmitterSubscribe emitter)
        {
            _emitter = emitter;
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            HideScreen();
        }

        private void OnEnable()
        {
            _emitter.Subscribe(this);
        }

        private void OnDisable()
        {
            _emitter.Unsubscribe(this);
        }

        public void On(MatchmakingSearchStarted ev)
        {
            ShowScreen();
        }

        public void On(MatchmakingError ev)
        {
            HideScreen();
            Debug.LogError($"Matchmaking error: {ev.Message}");
        }

        private void ShowScreen()
        {
            ChangeScreenState(true);
        }

        private void HideScreen()
        {
            ChangeScreenState(false);
        }

        private void ChangeScreenState(bool isActive)
        {
            _canvasGroup.alpha = isActive ? 1f : 0f;
            _canvasGroup.interactable = isActive;
            _canvasGroup.blocksRaycasts = isActive;
        }
    }
}
