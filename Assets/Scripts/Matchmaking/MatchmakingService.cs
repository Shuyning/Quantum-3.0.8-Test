using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Realtime;
using Quantum;
using QuantumTest.Config;
using UnityEngine;
using Zenject;

namespace QuantumTest.Matchmaking
{
    public class MatchmakingService : IMatchmakingService, ITickable, IInitializable, IDisposable
    {
        private const byte MaxPlayers = 1;

        private readonly IEmitterPublish _emitter;
        private readonly ISceneLoadCoordinator _sceneLoader;
        private readonly GameMapsConfig _mapsConfig;

        private RealtimeClient _client;
        private CancellationTokenSource _cts;
        private IDisposable _playerCountListener;
        private bool _gameStarted;

        [Inject]
        public MatchmakingService(IEmitterPublish emitter, ISceneLoadCoordinator sceneLoader, GameMapsConfig mapsConfig)
        {
            _emitter = emitter;
            _sceneLoader = sceneLoader;
            _mapsConfig = mapsConfig;
        }

        public void Initialize()
        {
            _client = new RealtimeClient();
            var appSettings = PhotonServerSettings.Global.AppSettings;
            appSettings.AppVersion = Application.version;
            _client.ConnectUsingSettings(appSettings);
        }

        public void Dispose()
        {
            CleanupAsync().Forget();
        }

        private async UniTask CleanupAsync()
        {
            _playerCountListener?.Dispose();
            _playerCountListener = null;
            
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            
            if (QuantumRunner.Default != null)
            {
                await QuantumRunner.ShutdownAllAsync();
            }
            
            if (_client is { InRoom: true })
            {
                _client.OpLeaveRoom(false);
                await UniTask.WaitUntil(() => !_client.InRoom || _client == null);
            }
            
            if (_client != null)
            {
                _client.Disconnect();
                _client = null;
            }
            
            _gameStarted = false;
        }

        public void Tick()
        {
            _client?.Service();
        }

        public async UniTask StartQuickMatchAsync()
        {
            _emitter.Publish(new MatchmakingSearchStarted());
            _cts = new CancellationTokenSource();

            var matchArgs = new MatchmakingArguments
            {
                PhotonSettings = PhotonServerSettings.Global.AppSettings,
                MaxPlayers = MaxPlayers,
                CanOnlyJoin = false,
                EmptyRoomTtlInSeconds = 0,
                PlayerTtlInSeconds = 0,
                PluginName = "QuantumPlugin",
                NetworkClient = _client,
                AsyncConfig = new AsyncConfig
                {
                    TaskFactory = AsyncConfig.CreateUnityTaskFactory(),
                    CancellationToken = _cts.Token
                }
            };

            try
            {
                _client = await MatchmakingExtensions.ConnectToRoomAsync(matchArgs);
                _emitter.Publish(new MatchmakingSearchCompleted());
                
                _playerCountListener = _client.CallbackMessage.ListenManual<OnPlayerEnteredRoomMsg>(OnPlayerEnteredRoom);
                
                CheckAndStartGame();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _emitter.Publish(new MatchmakingError(e.Message));
                await CleanupAsync();
            }
        }

        public async UniTask LeaveToMenuAsync()
        {
            await CleanupAsync();
        }
        
        private void OnPlayerEnteredRoom(OnPlayerEnteredRoomMsg msg)
        {
            CheckAndStartGame();
        }
        
        private void CheckAndStartGame()
        {
            if (_gameStarted)
                return;
                
            if (_client is not { InRoom: true })
                return;
                
            if (_client.CurrentRoom.PlayerCount < MaxPlayers)
                return;
                
            _gameStarted = true;
            LoadGameScene().Forget();
        }

        private async UniTask LoadGameScene()
        {
            try
            {
                var runtimeConfig = new RuntimeConfig
                {
                    Map = _mapsConfig.FirstLevelMap,
                    SimulationConfig = _mapsConfig.SimulationConfig,
                    SystemsConfig = _mapsConfig.SystemsConfig,
                    Seed = 0
                };

                await _sceneLoader.LoadGameForAllAsync(runtimeConfig, _client);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _emitter.Publish(new MatchmakingError(e.Message));
                await CleanupAsync();
            }
        }
    }
}
