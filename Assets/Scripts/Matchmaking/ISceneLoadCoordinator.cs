using Cysharp.Threading.Tasks;
using Quantum;
using QuantumTest.Scenes;

namespace QuantumTest.Matchmaking
{
    public interface ISceneLoadCoordinator
    {
        UniTask LoadGameSceneAsync(string sceneName);
        UniTask LoadLocalAsync(SceneId sceneId);
        UniTask LoadMapAsync(AssetRef<Map> mapRef);
    }
}
