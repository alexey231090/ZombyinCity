using System;
using System.Collections.Generic;
using UnityEngine;
using FpsZomby;

public class Player : MonoBehaviour
{
 private Dictionary<Type, IPlayerBehaviour> behaviorsMap = new Dictionary<Type, IPlayerBehaviour>();

 private IPlayerBehaviour behaviorCurrent; // Текущее состояние

    

    


    private void Awake()
    {
        this.InitBehaviors();
    }



    //------------------------------------------------------------------Добавить новые скрипты наследуемые от IPlayer... в массив
    private void InitBehaviors()
    {     

        this.behaviorsMap = new Dictionary<Type, IPlayerBehaviour>();

        this.behaviorsMap[typeof(PlayerCrowbarBehaviur)] = new PlayerCrowbarBehaviur();
        this.behaviorsMap[typeof(PlayerGunBehaviur)] = new PlayerGunBehaviur();
        this.behaviorsMap[typeof(PlayerBennelliBehaviour)] = new PlayerBennelliBehaviour();
        this.behaviorsMap[typeof(PlayerAk74Behaviour)] = new PlayerAk74Behaviour();


    }

    public void SetBehavior(IPlayerBehaviour newBehaviour)
    {
        if (this.behaviorCurrent != null)
        {
            this.behaviorCurrent.Exit();
        }   
            this.behaviorCurrent = newBehaviour; 
            this.behaviorCurrent.Enter();

        
    }


   



    public IPlayerBehaviour GetBihevior<T>() where T : IPlayerBehaviour
    {
        var type = typeof(T);

        return this.behaviorsMap[type];

    }

    private void Update()
    {
            this.behaviorCurrent?.Update();          
    }




    //------------------------------------------------------------------Добавить новые методы для вызова
    public IPlayerBehaviour SetBehaviourCrowbar()                                                          
    {
        IPlayerBehaviour behaviour = this.GetBihevior<PlayerCrowbarBehaviur>();     //---------------------- Добавить код
       return behaviour;
        
    }

    public IPlayerBehaviour SetBehaviourGun()
    {
        IPlayerBehaviour behaviour = this.GetBihevior<PlayerGunBehaviur>();
        return behaviour;
    }

    public IPlayerBehaviour SetBehaviorBennelli()
    {
        IPlayerBehaviour behaviour = this.GetBihevior<PlayerBennelliBehaviour>();

        return behaviour;
        
    }

    public IPlayerBehaviour SetBehaviorAk74()
    {
        IPlayerBehaviour behaviour = this.GetBihevior<PlayerAk74Behaviour>();

        return behaviour;
    }


}
