using UnityEngine;

namespace QuantumTest
{
    public interface IInputProvider
    {
        public Vector2 LookDelta { get; }
    }
}