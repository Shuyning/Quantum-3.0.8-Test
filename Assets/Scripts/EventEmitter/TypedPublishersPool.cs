using System;
using System.Collections.Generic;

namespace QuantumTest
{
	public static class TypedPublishersPool
	{
		private static readonly Dictionary<Type, TypedPublisher> _cache = new();

		public static TypedPublisher Fetch(IEvent ev)
		{
			return Fetch(ev.GetType());
		}

		private static TypedPublisher Fetch(Type eventType)
		{
			if (_cache.TryGetValue(eventType, out var cached))
				return cached;
				
			return _cache[eventType] = CreatePublisher(eventType);
		}

		private static TypedPublisher CreatePublisher(Type eventType)
		{
			var publisherClass = typeof(TypedPublisher<>).MakeGenericType(eventType);
			return (TypedPublisher) Activator.CreateInstance(publisherClass);
		}
	}
}