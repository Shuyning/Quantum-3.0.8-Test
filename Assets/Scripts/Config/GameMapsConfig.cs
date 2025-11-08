using Quantum;
using UnityEngine;

namespace QuantumTest.Config
{
    [CreateAssetMenu(menuName = "QuantumTest/GameMapsConfig", fileName = "GameMapsConfig")]
    public class GameMapsConfig : ScriptableObject
    {
        public AssetRef<Map> FirstLevelMap;
        public AssetRef<Map> SecondLevelMap;
        public AssetRef<SimulationConfig> SimulationConfig;
        public AssetRef<SystemsConfig> SystemsConfig;
    }
}
