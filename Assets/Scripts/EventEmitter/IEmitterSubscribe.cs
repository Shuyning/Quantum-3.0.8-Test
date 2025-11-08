namespace QuantumTest
{
	public interface IEmitterSubscribe
	{
		void SubscribeAll(params IListener[] listeners);

		void Subscribe(IListener listener);

		void Unsubscribe(IListener listener);

		void UnsubscribeAll();
	}
}