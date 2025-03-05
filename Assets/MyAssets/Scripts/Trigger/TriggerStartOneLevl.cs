using UnityEngine;

public class TriggerStartOneLevl : MonoBehaviour
{
   public AudioSource SoundPeoplesPanic;

    //Старт уровня
    private void OnTriggerEnter(Collider other)
    {
        SoundPeoplesPanic.Play();
        gameObject.SetActive(false);
    }

    
}
