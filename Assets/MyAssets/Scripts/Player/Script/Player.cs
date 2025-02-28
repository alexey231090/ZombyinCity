using System;
using System.Collections.Generic;
using UnityEngine;
using FpsZomby;


public class Player : MonoBehaviour
{
    //Словарь поведений
    private Dictionary<Type, IPlayerBehaviour> behaviorsMap = new Dictionary<Type, IPlayerBehaviour>();

    //Текущее поведение
    private IPlayerBehaviour behaviorCurrent; 

    private void Awake()
    {
        this.InitBehaviors();
    }

    //Инициализация всех поведений наследуемых от IPlayerBehaviour в словарь
    private void InitBehaviors()
    {     
        this.behaviorsMap = new Dictionary<Type, IPlayerBehaviour>();

        this.behaviorsMap[typeof(PlayerCrowbarBehaviur)] = new PlayerCrowbarBehaviur(); //Лом   
        this.behaviorsMap[typeof(PlayerGunBehaviur)] = new PlayerGunBehaviur();         //Пистолет
        this.behaviorsMap[typeof(PlayerBennelliBehaviour)] = new PlayerBennelliBehaviour(); //Дробовик
        this.behaviorsMap[typeof(PlayerAk74Behaviour)] = new PlayerAk74Behaviour();         //АК74
    }

    //Установка поведения
    public void SetBehavior(IPlayerBehaviour newBehaviour)
    {
        if (this.behaviorCurrent != null)
        {
            this.behaviorCurrent.Exit();
        }   
            this.behaviorCurrent = newBehaviour; 
            this.behaviorCurrent.Enter();
    }

    //Получение поведения
    public IPlayerBehaviour GetBihevior<T>() where T : IPlayerBehaviour
    {
        var type = typeof(T);
        return this.behaviorsMap[type];
    }

    //Обновление поведения
    private void Update()
    {
            this.behaviorCurrent?.Update();          
    }

    //Методы выбора оружия для игрока
    public IPlayerBehaviour SetBehaviourCrowbar()                                                          
    {
        IPlayerBehaviour behaviour = this.GetBihevior<PlayerCrowbarBehaviur>();     //Выбор лома
        return behaviour;
    }

    public IPlayerBehaviour SetBehaviourGun()
    {
        IPlayerBehaviour behaviour = this.GetBihevior<PlayerGunBehaviur>();          //Выбор пистолета
        return behaviour;
    }

    public IPlayerBehaviour SetBehaviorBennelli()
    {
        IPlayerBehaviour behaviour = this.GetBihevior<PlayerBennelliBehaviour>();      //Выбор дробовика
        return behaviour;
    }

    public IPlayerBehaviour SetBehaviorAk74()
    {
        IPlayerBehaviour behaviour = this.GetBihevior<PlayerAk74Behaviour>();          //Выбор АК74
        return behaviour;
    }
}
