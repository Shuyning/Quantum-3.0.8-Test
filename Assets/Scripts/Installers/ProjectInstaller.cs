using QuantumTest;
using QuantumTest.Config;
using QuantumTest.Matchmaking;
using Zenject;
using UnityEngine;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private GameMapsConfig gameMapsConfig;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<Emitter>().AsSingle();
            Container.Bind<GameMapsConfig>().FromInstance(gameMapsConfig).AsSingle();
            Container.BindInterfacesTo<MatchmakingService>().AsSingle();
            Container.BindInterfacesTo<SceneLoadCoordinator>().AsSingle();
        }
    }
}