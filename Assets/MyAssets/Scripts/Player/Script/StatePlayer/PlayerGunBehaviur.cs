using UnityEngine;
using FpsZomby;
using UniRx;


public class PlayerGunBehaviur : IPlayerBehaviour , IStateFire
{
    Animator animator;   
    Player player;
    int myIndex = 1;
    int spread = 10; //Разброс выстрелов
    float myDemage = 16;
    static public int ammo = 12; //Патроны  
    int allammo;
    int maxMagazine = 12;
    bool ExitTime = false;



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

        gunIntSubject.OnNext(ammo); // Отправка в патроны

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


        // Создаем Observable для отслеживания нажатия клавиши R
        Observable.EveryUpdate()

            .Where(_ => Input.GetKeyDown(KeyCode.R) && ammo < maxMagazine && allammo != 0) // Проверяем условия            
            .Subscribe(_ => Reload()) // Подписываемся на нажатие клавиши R
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
        if (Input.GetMouseButtonDown(0) && ammo > 0 && IsReloading() == false) // Нажата клавиша
        {


            animator.SetTrigger("OnFire");
            


        }

        if (Input.GetMouseButtonDown(0) && ammo <= 0 && IsReloading() == false)
        {
           
            soundmanager.noAmmoSound.Play();
        }

        



        //Если закончились патроны то оружие перезаряжается
        if (ammo == 0 && allammo != 0 && ExitTime == false)
        {
            Reload();
            
        }

        

    }


    private void Reload()
    {

        
        soundmanager.ReloadAudio[0].Play();
        animator.SetBool("ReloadBool",true);

        ExitTime = true;
    }


    void FinishRelad()
    {
        ExitTime = false;
        // Определяем, сколько патронов не хватает для достижения maxMagazine
        int neededAmmo = maxMagazine - ammo;

        if (allammo > 0) // Проверяем, есть ли патроны для перезарядки
        {
            // Проверяем, достаточно ли патронов в allammo для заполнения до maxMagazine
            if (allammo >= neededAmmo)
            {
                ammo += neededAmmo; // Добавляем необходимые патроны
                allammo -= neededAmmo; // Вычитаем добавленные патроны из allammo
            }
            else
            {
                ammo += allammo; // Добавляем все оставшиеся патроны
                allammo = 0; // Обнуляем allammo
            }

            // Обновляем патроны в глобальном хранилище
            PlayerSwitchingStates.weaponBullets["Gun"] = allammo;
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
            gunIntSubject.OnNext(ammo); // Обновляем значение
        }
    }

    private bool IsReloading()
    {
        // Получаем текущее состояние анимации
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // Проверяем, соответствует ли текущее состояние "GunReload"
        return stateInfo.IsName("GunReload");
    }

        



    // Стрельба активная! При выборе следующего оружия анимация не работает
    public void FireWepon(PlayerSwitchingStates playerSwitching, SoundManager soundManager)
    {

        //Звук и частицы
        soundManager.FireAudio[0].Play();
        playerSwitching.FireWeaponPart[0].Play();

        //Пуля Gameobject
        GameObject bullet = playerSwitching.bullets;

        //Луч выстрела стрельбы
        playerSwitching.ShootInWall(playerSwitching.TraceShot(spread), bullet);


        // Сохраняем луч в playerSwiching
        playerSwitching.rayWepon = playerSwitching.TraceShot(spread);

     

        ammo--;

        gunIntSubject.OnNext(ammo);
         

    }


    
}



