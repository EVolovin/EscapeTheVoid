using Zenject;
using EscapeTheVoid.World.Player;

namespace EscapeTheVoid.Core
{
    public class InjectionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Configure the Zenject bindings
            
            Container.Bind<IGameManager>().To<GameManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ISoundManager>().To<SoundManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IUIManager>().To<UIManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IThreatManager>().To<ThreatManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IPlayerController>().To<PlayerController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ITimer>().To<Timer>().FromComponentInHierarchy().AsSingle();
        }
    }
}