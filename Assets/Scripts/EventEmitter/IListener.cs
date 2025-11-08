namespace QuantumTest
{
	public interface IListener { }

	public interface IListener<in TEvent> : IListener where TEvent : IEvent
	{
		void On(TEvent ev);
	}
}