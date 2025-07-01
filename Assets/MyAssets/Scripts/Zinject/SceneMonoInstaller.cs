using UnityEngine;
using Zenject;
using FpsZomby;

public class SceneMonoInstaller : MonoInstaller
{
    public PlayerSwitchingStates playerSwitchingStates;
    public SoundManager soundManager;
    public __StartLevel __StartLevel;



    public override void InstallBindings()
    {
        Container.Bind<PlayerSwitchingStates>().FromInstance(playerSwitchingStates);
        Container.Bind<SoundManager>().FromInstance(soundManager);
        Container.Bind<__StartLevel>().FromInstance(__StartLevel);
    }
}