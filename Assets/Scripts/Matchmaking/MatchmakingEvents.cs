namespace QuantumTest.Matchmaking
{
    public record MatchmakingSearchStarted : IEvent { }
    public record MatchmakingSearchCompleted : IEvent { }

    public class MatchmakingError : IEvent
    {
        public string Message;

        public MatchmakingError(string m)
        {
            Message = m;
        }
    }
}
