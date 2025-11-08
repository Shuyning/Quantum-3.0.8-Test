using System;
using UnityEngine.Events;

namespace QuantumTest
{
	public class LocalEvents
	{
		public delegate void Curry();
		
		private IListener _listener;

		public void Fire<TEvent>(TEvent ev)
			where TEvent : IEvent
		{
			if (_listener == null) return;
			
			var tp = TypedPublishersPool.Fetch(ev);
			tp.TryPublish(ev, _listener);
		}

		public UnityAction Box<TEvent>()
			where TEvent : IEvent, new()
			=> () => Fire(new TEvent());

		public UnityAction Box<TEvent>(TEvent ev)
			where TEvent : IEvent
			=> () => Fire(ev);

		public UnityAction Box<TEvent>(Func<TEvent> func)
			where TEvent : IEvent
			=> () => Fire(func());

		public void SetListener(IListener listener)
		{
			_listener = listener;
		}

		public void UnsetListener(IListener listener)
		{
			if (_listener == listener)
				_listener = null;
		}
	}
}