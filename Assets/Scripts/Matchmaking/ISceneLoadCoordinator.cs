using Cysharp.Threading.Tasks;
using Photon.Realtime;
using Quantum;
using QuantumTest.Scenes;

namespace QuantumTest.Matchmaking
{
    public interface ISceneLoadCoordinator
    {
        public UniTask LoadGameForAllAsync(RuntimeConfig runtimeConfig, RealtimeClient client);
        public UniTask LoadLocalAsync(SceneId sceneId);
    }
}
