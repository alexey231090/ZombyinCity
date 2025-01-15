using UnityEngine;
using FPSGame;


public class PlayerBehavierIdle : IPlayerBihevior
{
  
   
    public void Enter()
    {
        

        Debug.Log("Idle Вход");

    }

    public void Exit()
    {


        Debug.Log("Idle Выход");
    }

    void IPlayerBihevior.Update()
    {
       
    }
}
