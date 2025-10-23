namespace Quantum
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Zenject;

    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<InputSystem_Actions>()
                .AsSingle()
                .NonLazy();

            Container.Bind<PlayerInputProvider>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();
        }
    }
}
