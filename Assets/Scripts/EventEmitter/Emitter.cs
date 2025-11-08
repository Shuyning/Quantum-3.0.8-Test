using System;
using System.Collections.Generic;
using System.Linq;

namespace QuantumTest
{
	public class Emitter : IEmitterPublish, IEmitterSubscribe
	{
		private readonly HashSet<IListener> _listeners = new();
		private readonly Dictionary<Type, IEvent> _eventsCache = new Dictionary<Type, IEvent>();

		public void Publish(IEvent ev)
		{
			var publisher = TypedPublishersPool.Fetch(ev);

			foreach (var listener in _listeners.ToArray())
				publisher.TryPublish(ev, listener);
		}

		public void Publish<T>() where T : IEvent, new()
		{
			var type = typeof(T);
			if (!_eventsCache.TryGetValue(type, out var currentEvent))
			{
				currentEvent = new T();
				_eventsCache.Add(type, currentEvent);
			}

			Publish(currentEvent);
		}

		public void SubscribeAll(params IListener[] listeners)
		{
			foreach (var listener in listeners)
				_listeners.Add(listener);
		}

		public void Subscribe(IListener listener)
		{
			if (_listeners.Add(listener) == false)
				throw new($"Listener is already subscribed in Emitter: {listener}");
		}

		public void Unsubscribe(IListener listener)
		{
			_listeners.Remove(listener);
		}

		public void UnsubscribeAll(params IListener[] listeners)
		{
			foreach (var listener in listeners)
				_listeners.Remove(listener);
		}

		public void UnsubscribeAll()
		{
			_listeners.Clear();
		}
	}
}