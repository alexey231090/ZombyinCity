using UnityEngine;
using FPSGame;


public class PlayerBehavierIdle : IPlayerBihevior
{
  
   
    public void Enter()
    {
        

        Debug.Log("Idle ����");

    }

    public void Exit()
    {


        Debug.Log("Idle �����");
    }

    void IPlayerBihevior.Update()
    {
       
    }
}
