using System;

namespace QuantumTest
{
	public abstract class TypedPublisher
	{
		public abstract void TryPublish(IEvent ev, IListener listener);
		
		public abstract Type EventType { get; }
	}
	
	public class TypedPublisher<TEvent> : TypedPublisher where TEvent : IEvent
	{
		public override void TryPublish(IEvent ev, IListener listener)
		{
			if (listener is IListener<TEvent> casted) {
				var tEvent = (TEvent) ev;
				casted.On(tEvent);
			}
		}

		public override Type EventType => typeof(TEvent);
	}
}