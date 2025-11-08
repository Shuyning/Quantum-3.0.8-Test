using Quantum;
using UnityEngine;

namespace QuantumTest.Contexts
{
    public sealed class ScenePlayerInputContext : MonoBehaviour, IQuantumViewContext
    {
        [SerializeField] private KccInputCollector inputCollector;
        
        public IInputProvider InputProvider => inputCollector;
    }
}