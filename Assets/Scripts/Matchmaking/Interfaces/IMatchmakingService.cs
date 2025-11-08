using Cysharp.Threading.Tasks;

namespace QuantumTest.Matchmaking
{
    public interface IMatchmakingService
    {
        public UniTask StartQuickMatchAsync();
        public UniTask LeaveToMenuAsync();
    }
}
