using UnityEngine;
using FPSGame;

public class PlayerBehavierActiv : IPlayerBihevior
{
    Player Player;

    public void Enter()
    {
        Player = GameObject.FindObjectOfType<Player>();
        Debug.Log("Active ����");
    }

    public void Exit()
    {
        Debug.Log("Active �����");
    }

    void IPlayerBihevior.Update()
    {
        Debug.Log("Active Update");

        Player.transform.position += new Vector3(0, 0.03f, 0);
    }
}
