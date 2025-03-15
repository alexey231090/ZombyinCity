using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDead : MonoBehaviour
{
    PlayerSwitchingStates switchingStates;
    void Start()
    {
        switchingStates = GameObject.FindObjectOfType<PlayerSwitchingStates>();
    }

    
    void Update()
    {
        
    }
}
