using Cysharp.Threading.Tasks;
using Quantum;
using QuantumTest.Scenes;
using UnityEngine.SceneManagement;

namespace QuantumTest.Matchmaking
{
    public class SceneLoadCoordinator : ISceneLoadCoordinator
    {
        public async UniTask LoadMapAsync(AssetRef<Map> mapRef)
        {
            if (!mapRef.Id.IsValid)
                return;

            if (!QuantumUnityDB.TryGetGlobalAsset(mapRef, out Map map))
                return;

            if (QuantumRunner.Default != null && QuantumRunner.Default.Session.IsRunning)
            {
                var command = new ChangeMapCommand { MapRef = mapRef };
                QuantumRunner.Default.Game.SendCommand(command);
            }
        }
    }
}
