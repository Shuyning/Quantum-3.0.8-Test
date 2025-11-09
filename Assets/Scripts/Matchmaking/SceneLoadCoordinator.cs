using Cysharp.Threading.Tasks;
using Quantum;
using QuantumTest.Scenes;
using UnityEngine.SceneManagement;

namespace QuantumTest.Matchmaking
{
    public class SceneLoadCoordinator : ISceneLoadCoordinator
    {
        public async UniTask LoadGameSceneAsync(string sceneName)
        {
            var loadSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (loadSceneAsync is { isDone: false })
                await UniTask.Yield();
        }

        public async UniTask LoadLocalAsync(SceneId sceneId)
        {
            var sceneName = SceneCatalog.GetName(sceneId);
            await LoadGameSceneAsync(sceneName);
        }

        public async UniTask LoadMapAsync(AssetRef<Map> mapRef)
        {
            if (!mapRef.Id.IsValid)
                return;

            if (!QuantumUnityDB.TryGetGlobalAsset(mapRef, out Map map))
                return;

            await LoadGameSceneAsync(map.Scene);

            if (QuantumRunner.Default != null && QuantumRunner.Default.Session.IsRunning)
            {
                var command = new ChangeMapCommand { MapRef = mapRef };
                QuantumRunner.Default.Game.SendCommand(command);
            }
        }
    }
}
