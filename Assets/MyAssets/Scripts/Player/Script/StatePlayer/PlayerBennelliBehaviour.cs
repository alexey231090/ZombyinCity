using UnityEngine;
using FpsZomby;
using UniRx;
using System;

public class PlayerBennelliBehaviour : IPlayerBehaviour,IStateFire,IStateReload
{

    Animator animator;
    PlayerSwitchingStates switchState;    
    SoundManager soundManager;
    int myIndex = 2;   
    int spread = 70; // Разброс
    static int ammo = 4; // Патроны в магазине
    float myDemage = 15;
    static int allammo;
    int maxMagazine = 8; // Максимальное количество патронов
    

   public static readonly Subject<int> bennelliIntSubject = new();


    CompositeDisposable _disposable = new();

    void IPlayerBehaviour.Enter()
    {
        Debug.Log("Enter _ Bennelli");



        animator = GameObject.FindGameObjectWithTag("Hand").GetComponent<Animator>();
        switchState = GameObject.FindObjectOfType<PlayerSwitchingStates>();
        soundManager = GameObject.FindObjectOfType<SoundManager>();

        bennelliIntSubject.OnNext(ammo);

        switchState.weponArray[myIndex].SetActive(true);

        animator.SetInteger("WeponNum", 2);

        switchState.Demage = myDemage;

        soundManager.SelectOther.Play();

        // Отправить количество пуль в аниматор
        animator.SetInteger("AllAmmoBennelli", allammo);

        // Отправить количество пуль в Словарь
        PlayerSwitchingStates.allBulletsDictSubject.Subscribe(value =>
        {

            allammo = value["Bennelli_M4"];

        }).AddTo(_disposable);

        
        StateReload.reloadBennelliSubject.Subscribe(value =>
        {
            SoundReload();

        }).AddTo(_disposable);


        PlayerSwitchingStates.isDead.Subscribe(x =>
        {
            animator.SetInteger("WeponNum", -1);
        }).AddTo(_disposable);


    }

    void IPlayerBehaviour.Exit()
    {
        Debug.Log("Exit _ Bennelli");


       

    }

    // Обновление
    void IPlayerBehaviour.Update()
    {
        if (Input.GetMouseButtonDown(0) && ammo > 0 && IsReloading() == false) 
        {            
            animator.SetTrigger("OnFire");

            // Отправить количество пуль в аниматор
            animator.SetInteger("AllAmmoBennelli", allammo);
        }

        if (Input.GetMouseButtonDown(0) && ammo <= 0 && IsReloading() == false)
        {
            soundManager.noAmmoSound.Play();
        }

        // Перезарядка
        if (Input.GetKeyDown(KeyCode.R) && ammo < maxMagazine && allammo > 0)
        {
            animator.SetBool("ReloadBool",true);
            //Задержка bool для переключения анимации
            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ => animator.SetBool("ReloadBool", false)).AddTo(_disposable);
            bennelliIntSubject.OnNext(ammo);
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
            
            Debug.Log(allammo);
        }

        

        // Если магазин равен 0
        else if(ammo == 0 && allammo != 0 && animator.GetBool("ReloadBool") == false)
        {
            animator.SetBool("ReloadBool", true);
            Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ => animator.SetBool("ReloadBool", false)).AddTo(_disposable);

            bennelliIntSubject.OnNext(ammo);


            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
        }




        animator.SetInteger("Patron", ammo);
    }


    void SoundReload()
    {
        soundManager.ReloadAudio[1].Play();

    }

    // Стрельба из оружия! Этот метод вызывается из аниматора
    public void FireWepon(PlayerSwitchingStates playerSwitching, SoundManager soundManager)
    {
        soundManager.FireAudio[1].Play();               // Звук
        playerSwitching.FireWeaponPart[1].Play();       // Частицы

        // Луч из центра камеры
        Ray ray = playerSwitching.RayCastCameraCenter();

        // Найти GameObject
        GameObject bullet = playerSwitching.bullets;

        // Для дроби
        for (int i = 0; i < 7; i++)
        {
            playerSwitching.ShootInWall(playerSwitching.TraceShot(spread), bullet);

            // Отправить луч в playerSwitching
            playerSwitching.rayWepon = playerSwitching.TraceShot(spread);
        }

        ammo--;
      
        bennelliIntSubject.OnNext(ammo);




    }


    // Перезарядка
    public void Reload(PlayerSwitchingStates playerSwitching, Animator animator)
    {
        // Проверить, есть ли место в магазине и есть ли патроны для перезарядки
        if (ammo < maxMagazine && allammo > 0)
        {            
            // Добавить патрон в магазин и удалить его из общего количества
            ammo++;
            allammo--;


            // Обновить значения в словаре и отправить уведомления
            PlayerSwitchingStates.weaponBullets["Bennelli_M4"] = allammo;
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
            bennelliIntSubject.OnNext(ammo); // Отправить количество пуль в интерфейс


            // Отправить количество пуль в аниматор
            animator.SetInteger("AllAmmoBennelli", allammo);
        }
    }


   



    private bool IsReloading()
    {
        // Получить текущее состояние аниматора
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // Проверить, является ли текущее состояние "BenneliReload"

        if(stateInfo.IsName("BenneliReload") || stateInfo.IsName("BennelliOneAmmo") || stateInfo.IsName("BennelliEndReload"))
        {
            return true;
        }
        else
        {
            return false;
        }

       
    }


}
