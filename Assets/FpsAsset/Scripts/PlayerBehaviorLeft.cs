using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSGame;

public class PlayerBehaviorLeft : IPlayerBihevior
{

    Player player;
    public void Enter()
    {
       player = GameObject.FindObjectOfType<Player>();

        Debug.Log("Left ¬ход");
    }

    public void Exit()
    {
        Debug.Log("Left ¬ход");
    }


    void IPlayerBihevior.Update()
    {
        player.transform.position += new Vector3(-0.03f, 0, 0);


        Debug.Log("Idle Update");
    }
}
