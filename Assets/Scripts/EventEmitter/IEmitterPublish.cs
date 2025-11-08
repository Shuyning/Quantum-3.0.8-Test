namespace QuantumTest
{
	public interface IEmitterPublish
	{
		void Publish(IEvent ev);
		void Publish<T>() where T : IEvent, new();
	}
}