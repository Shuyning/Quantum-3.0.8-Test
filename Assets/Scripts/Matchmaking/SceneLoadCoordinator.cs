using Cysharp.Threading.Tasks;
using Photon.Deterministic;
using Photon.Realtime;
using Quantum;
using QuantumTest.Config;
using QuantumTest.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace QuantumTest.Matchmaking
{
    public class SceneLoadCoordinator : ISceneLoadCoordinator
    {
        public async UniTask LoadGameForAllAsync(RuntimeConfig runtimeConfig, RealtimeClient client)
        {
            if (!runtimeConfig.Map.Id.IsValid)
            {
                Debug.LogError("RuntimeConfig.Map is not valid");
                return;
            }

            if (!QuantumUnityDB.TryGetGlobalAsset(runtimeConfig.Map, out Map map))
            {
                Debug.LogError($"Map asset not found: {runtimeConfig.Map}");
                return;
            }

            await LoadSceneAsync(map.Scene);
            await StartQuantumRunner(runtimeConfig, client);
        }

        public async UniTask LoadLocalAsync(SceneId sceneId)
        {
            var sceneName = SceneCatalog.GetName(sceneId);
            await LoadSceneAsync(sceneName);
        }

        private static async UniTask LoadSceneAsync(string sceneName)
        {
            var loadSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (loadSceneAsync is { isDone: false })
                await UniTask.Yield();
        }

        private async UniTask StartQuantumRunner(RuntimeConfig runtimeConfig, RealtimeClient client)
        {
            if (client is not { InRoom: true })
            {
                Debug.LogError("Cannot start Quantum runner: not in room");
                return;
            }

            var args = new SessionRunner.Arguments
            {
                RunnerFactory = QuantumRunnerUnityFactory.DefaultFactory,
                GameParameters = QuantumRunnerUnityFactory.CreateGameParameters,
                ClientId = client.UserId ?? System.Guid.NewGuid().ToString(),
                RuntimeConfig = runtimeConfig,
                SessionConfig = QuantumDeterministicSessionConfigAsset.Global.Config,
                GameMode = DeterministicGameMode.Multiplayer,
                PlayerCount = client.CurrentRoom.MaxPlayers,
                Communicator = new QuantumNetworkCommunicator(client),
                DeltaTimeType = SimulationUpdateTime.Default,
                StartGameTimeoutInSeconds = 10
            };

            var runner = (QuantumRunner)await SessionRunner.StartAsync(args);

            var localPlayerData = new RuntimePlayer();

            runner.Game.AddPlayer(localPlayerData);
        }
    }
}
