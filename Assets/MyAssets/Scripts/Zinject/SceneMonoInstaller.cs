using Zenject;

public class SceneMonoInstaller : MonoInstaller
{
    public PlayerSwitchingStates playerSwitchingStates;
    public SoundManager soundManager;
   
    public override void InstallBindings()
    {
        Container.Bind<PlayerSwitchingStates>().FromInstance(playerSwitchingStates);
        Container.Bind<SoundManager>().FromInstance(soundManager);
    }
}