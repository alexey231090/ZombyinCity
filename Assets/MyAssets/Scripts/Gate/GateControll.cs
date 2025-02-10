using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateControll : MonoBehaviour
{
    public Animator animatorGate;
    public AudioSource openDoor;


    private void OnTriggerEnter(Collider other)
    {
        animatorGate.SetBool("Open",true);
        openDoor.Play();
        Destroy(this.gameObject);

        
    }
}
