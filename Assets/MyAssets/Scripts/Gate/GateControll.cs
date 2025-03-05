using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateControll : MonoBehaviour
{
    public Animator animatorGate;
    public Animator helicopterAnim;
    public AudioSource openDoor;
    public AudioSource helicopterFly;
    public AudioSource music;
    public AudioSource birdsSound;

    // Старт уровня, включение анимации открытия ворот
    private void OnTriggerEnter(Collider other)
    {
        animatorGate.SetBool("Open",true);
        helicopterAnim.SetBool("Fly",true);

        openDoor.Play();
        helicopterFly.Play();
        music.Play();
        birdsSound.Stop();

        Destroy(this.gameObject);

        
    }
}
