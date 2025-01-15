using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    Player player;
    void Start()
    {
           player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
      if(Input.GetKeyDown(KeyCode.Space))
        {
            
        }

      if(Input.GetKeyDown(KeyCode.E))
        {
            
        }
    }
}
