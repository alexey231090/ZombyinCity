using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerStartOneLevl : MonoBehaviour
{
   public AudioSource SoundPeoplesPanic;


    private void OnTriggerEnter(Collider other)
    {
        SoundPeoplesPanic.Play();
        gameObject.SetActive(false);
    }
}
