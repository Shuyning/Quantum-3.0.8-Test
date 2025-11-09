using Cysharp.Threading.Tasks;
using Quantum;

namespace QuantumTest.Matchmaking
{
    public interface IMatchmakingService
    {
        UniTask StartQuickMatchAsync();
        UniTask LeaveToMenuAsync();
    }
}
