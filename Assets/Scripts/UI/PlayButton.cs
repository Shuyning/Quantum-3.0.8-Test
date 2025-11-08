using Cysharp.Threading.Tasks;
using QuantumTest.Matchmaking;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace QuantumTest.UI
{
    [RequireComponent(typeof(Button))]
    public class PlayButton : MonoBehaviour, IListener<MatchmakingError>
    {
        private Button _button;
        
        private IMatchmakingService _matchmakingService;
        private IEmitterSubscribe _emitter;

        [Inject]
        public void Construct(IMatchmakingService matchmakingService, IEmitterSubscribe emitterSubscribe)
        {
            _matchmakingService = matchmakingService;
            _emitter = emitterSubscribe;
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnPlayButtonClicked);
            _emitter.Subscribe(this);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnPlayButtonClicked);
            _emitter.Unsubscribe(this);
        }

        private void OnPlayButtonClicked()
        {
            _button.interactable = false;
            _matchmakingService.StartQuickMatchAsync().Forget();
        }

        public void On(MatchmakingError ev)
        {
            _button.interactable = true;
        }
    }
}
