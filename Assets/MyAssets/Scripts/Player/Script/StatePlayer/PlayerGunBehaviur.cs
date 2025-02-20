using UnityEngine;
using FpsZomby;
using UniRx;


public class PlayerGunBehaviur : IPlayerBehaviour , IStateFire
{
    Animator animator;   
    Player player;
    int myIndex = 1;
    int spread = 10; //разброс выстрела
    float myDemage = 16;
    static public int ammo = 12; //патроны  
    int allammo;
    int maxMagazine = 12;

    

    public static readonly Subject<int> gunIntSubject = new();
    
    PlayerSwitchingStates switchState;
    SoundManager soundmanager;

    CompositeDisposable _disposable = new();
    

    public void Enter()
    {
        Debug.Log("Вход _ Gun");      

        animator = GameObject.FindGameObjectWithTag("Hand").GetComponent<Animator>(); 
        switchState = GameObject.FindObjectOfType<PlayerSwitchingStates>();                
        soundmanager = GameObject.FindObjectOfType<SoundManager>();

        switchState.weponArray[myIndex].SetActive(true);

        animator.SetInteger("WeponNum", myIndex);        

        gunIntSubject.OnNext(ammo); // Передача в канвас

        switchState.Demage = myDemage;

        soundmanager.SelectOther.Play();
       

        PlayerSwitchingStates.allBulletsDictSubject
            .Subscribe(value =>
        {            
            allammo = value["Gun"];

        }).AddTo(_disposable);


       




        StateReload.reloadGunSubject.Subscribe(value =>
        {

            FinishRelad();


        }).AddTo(_disposable);


        // Создаем Observable для отслеживания нажатий клавиши R
        Observable.EveryUpdate()

            .Where(_ => Input.GetKeyDown(KeyCode.R) && ammo < maxMagazine && allammo != 0) // Проверяем условия            
            .Subscribe(_ => Reload()) // Подписываемся на событие Нажатия R
            .AddTo(_disposable);





        PlayerSwitchingStates.isDead.Subscribe(x =>
        {
            animator.SetInteger("WeponNum", -1);
        }).AddTo(_disposable);




    }

    

    public void Exit()
    {      
        Debug.Log("Выход _ Gun");

        
       
    }




    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && ammo > 0 && IsReloading() == false) // Тригер Выстрел
        {


            animator.SetTrigger("OnFire");
            


        }

        if (Input.GetMouseButtonDown(0) && ammo <= 0 && IsReloading() == false)
        {
           
            soundmanager.noAmmoSound.Play();
        }

        



        //Если кончилась обоима то делаем перезарядку
        if (ammo == 0 && allammo != 0)
        {
            Reload();
            
        }

    }


    private void Reload()
    {

        
        soundmanager.ReloadAudio[0].Play();
        animator.SetBool("ReloadBool",true);

        
    }


    void FinishRelad()
    {
        // Определяем, сколько патронов не хватает для достижения maxMagazine
        int neededAmmo = maxMagazine - ammo;

        if (allammo > 0) // Проверяем, есть ли патроны для перезарядки
        {
            // Проверяем, достаточно ли патронов в allammo для заполнения до maxMagazine
            if (allammo >= neededAmmo)
            {
                ammo += neededAmmo; // Добавляем недостающие патроны
                allammo -= neededAmmo; // Вычитаем добавленные патроны из allammo
            }
            else
            {
                ammo += allammo; // Добавляем все оставшиеся патроны
                allammo = 0; // Обнуляем allammo
            }

            // Обновляем словарь и отправляем изменения
            PlayerSwitchingStates.weaponBullets["Gun"] = allammo;
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
            gunIntSubject.OnNext(ammo); // Обновляем количест
        }
    }

    private bool IsReloading()
    {
        // Получаем текущее состояние анимации
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // Проверяем, соответствует ли текущее состояние "GunReload"
        return stateInfo.IsName("GunReload");
    }

        



    // Событие выстрела! Для точной синхронной работы эффектов и действий
    public void FireWepon(PlayerSwitchingStates playerSwitching, SoundManager soundManager)
    {

        //звук и партиклы
        soundManager.FireAudio[0].Play();
        playerSwitching.FireWeaponPart[0].Play();

        //Пуля Gameobject
        GameObject bullet = playerSwitching.bullets;

        //луч выстрел разброс
        playerSwitching.ShootInWall(playerSwitching.TraceShot(spread), bullet);


        // Параметры луча в playerSwiching
        playerSwitching.rayWepon = playerSwitching.TraceShot(spread);

     

        ammo--;

        gunIntSubject.OnNext(ammo);
         

    }


    
}



