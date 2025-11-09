using Cysharp.Threading.Tasks;
using Quantum;
using QuantumTest.Scenes;

namespace QuantumTest.Matchmaking
{
    public interface ISceneLoadCoordinator
    {
        UniTask LoadMapAsync(AssetRef<Map> mapRef);
    }
}
