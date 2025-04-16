using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

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
        animatorGate.SetBool("Open", true);

        // Задержка в 1 секунду перед включением анимации вертолета
        Observable.Timer(System.TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            helicopterFly.Play();
            helicopterAnim.SetBool("Fly", true); 
            Destroy(this.gameObject); 

        }).AddTo(this);

        openDoor.Play();
        music.Play();
        birdsSound.Stop();
        helicopterAnim.SetBool("Fly", true);
    }
}
