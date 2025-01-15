using UnityEngine;
using FpsZomby;
using UniRx;

public class PlayerAk74Behaviour : IPlayerBehaviour,IStateFire
{
    public static readonly Subject<int> ak47ammoSubject = new();
    public static readonly Subject<int> ak4ammoAllSubject = new();

    PlayerSwitchingStates switchState;
    
    SoundManager SoundManager;

    Player player;

    Animator animator;

    int myIndex = 3;

    float timer = 0;
    float delay = 0.15f;  //Задержка между выстрелами
    float nextFire;
    float spread = 30f; //Разброс выстрела
    static public int ammo = 30; //кол патронов    
    int maxMagazine = 30;
    int allammo;


    CompositeDisposable _disposable = new();

    public void Enter()
    {
        Debug.Log("Вход _ AK74");
        switchState = GameObject.FindObjectOfType<PlayerSwitchingStates>();
        SoundManager = GameObject.FindObjectOfType<SoundManager>();
        animator = GameObject.FindGameObjectWithTag("Hand").GetComponent<Animator>();
        player = GameObject.FindObjectOfType(typeof(Player)) as Player;
        

        ak47ammoSubject.OnNext(ammo);

        switchState.weponArray[myIndex].SetActive(true);

        animator.SetInteger("WeponNum", 3);

        SoundManager.SelectOther.Play();



        PlayerSwitchingStates.allBulletsDictSubject.Subscribe(value =>
        {


            allammo = value["AK74"];


        }).AddTo(_disposable);




        PlayerSwitchingStates.isDead.Subscribe(x =>
        {
            animator.SetInteger("WeponNum", -1);
        }).AddTo(_disposable);
    }

    public void Exit()
    {
        Debug.Log("Выход _ AK74");

        animator.SetBool("AutoFire", false);

    }

   

    public void Update()
    {

        // Выстрел вкл
        if (Input.GetMouseButtonDown(0) && ammo > 0) 
        {
            animator.SetBool("AutoFire", true);
                   

        }

        //Звук нет патронов
        if (Input.GetMouseButtonDown(0) && ammo <= 0)
        {
            
            SoundManager.noAmmoSound.Play();

        }


        // Перезарядка
        else if (Input.GetKeyDown(KeyCode.R) && ammo < 30 && animator.GetBool("AutoFire") == false && allammo != 0)
        {

            animator.SetTrigger("Reload");
            
            SoundManager.ReloadAudio[2].Play();

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
                PlayerSwitchingStates.weaponBullets["AK74"] = allammo;
                PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
                ak47ammoSubject.OnNext(ammo); // Обновляем количество патронов в оружии
            }


            ak47ammoSubject.OnNext(ammo);

           
        }

        // Выстрел выкл2
        if (Input.GetMouseButtonUp(0) || ammo <= 0) 
        {
            animator.SetBool("AutoFire", false);

           
          
        }
       

    }

    //Событие выстрел в Update анимации    
    // Событие выстрела! Для точной синхронной работы эффектов и действий
    public void FireWepon(PlayerSwitchingStates playerSwitching,SoundManager soundManager)
    {

       

        if (Time.time > nextFire)
        {

            nextFire = Time.time + delay;
            DataShoot(playerSwitching,soundManager);

        }

        
    }

    // Действия и данные для выстрела
    void DataShoot(PlayerSwitchingStates playerSwitching, SoundManager SoundM)
    {
        // Генерируем случайное значение pitch в диапазоне от 0.8 до 1.2
        float randomPitch = Random.Range(0.9f, 1.25f);

        // Устанавливаем pitch и воспроизводим звук выстрела с помощью PlayOneShot
        SoundM.FireAudio[3].pitch = randomPitch;
        SoundM.FireAudio[3].PlayOneShot(SoundM.FireAudio[3].clip);

        // Воспроизводим визуальные эффекты выстрела
        playerSwitching.FireWeaponPart[2].Play();

        // Луч центра камеры
        Ray ray = playerSwitching.RayCastCameraCenter();

        // Пуля GameObject
        GameObject bullet = playerSwitching.bullets;

        // Луч выстрел разброс
        playerSwitching.ShootInWall(playerSwitching.TraceShot(spread), bullet);

        // Параметры луча в playerSwitching
        playerSwitching.rayWepon = playerSwitching.TraceShot(spread);

        ammo--;

        ak47ammoSubject.OnNext(ammo);
    }
}
