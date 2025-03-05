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
    float spread = 20f; //Разброс выстрелов
    static public int ammo = 30; //Патроны в обойме   
    int maxMagazine = 30;//Мах патронов 
    int allammo;//Патроны в глобальном хранилище


    CompositeDisposable _disposable = new();

    public void Enter()
    {
        Debug.Log("???? _ AK74");
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
        Debug.Log("????? _ AK74");

        animator.SetBool("AutoFire", false);

    }

   

    public void Update()
    {
        // Основной код
        if (Input.GetMouseButtonDown(0) && ammo > 0) 
        {
            animator.SetBool("AutoFire", true);
        }

        
        //Звук при выстреле
        if (Input.GetMouseButtonDown(0) && ammo <= 0)
        {
            SoundManager.noAmmoSound.Play();
        }

        // Автоматическая перезарядка при пустом магазине
        if (ammo == 0 && allammo > 0 && !animator.GetBool("AutoFire"))
        {
            animator.SetTrigger("Reload");
            SoundManager.ReloadAudio[2].Play();

            // Проверяем, хватает ли патронов для заполнения maxMagazine
            if (allammo > 0) // Проверяем, есть ли патроны для перезарядки
            {
                // Проверяем, достаточно ли патронов в allammo для заполнения до maxMagazine
                int neededAmmo = Mathf.Min(maxMagazine - ammo, allammo);
                ammo += neededAmmo; // Добавляем необходимые патроны
                allammo -= neededAmmo; // Вычитаем добавленные патроны из allammo
            }
            else
            {
                ammo += allammo; // Добавляем все оставшиеся патроны
                allammo = 0; // Обнуляем allammo
            }

            // Обновляем патроны в интерфейсе магазина
            PlayerSwitchingStates.weaponBullets["AK74"] = allammo;
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
            ak47ammoSubject.OnNext(ammo); // Обновляем количество патронов в обойме
        }
        // Перезарядка по кнопке R
        else if (Input.GetKeyDown(KeyCode.R) && ammo < 30 && animator.GetBool("AutoFire") == false && allammo != 0)
        {
            animator.SetTrigger("Reload");
            
            SoundManager.ReloadAudio[2].Play();

            // Проверяем, хватает ли патронов для заполнения maxMagazine
            if (allammo > 0) // Проверяем, есть ли патроны для перезарядки
            {
                // Проверяем, достаточно ли патронов в allammo для заполнения до maxMagazine
                int neededAmmo = Mathf.Min(maxMagazine - ammo, allammo);
                ammo += neededAmmo; // Добавляем необходимые патроны
                allammo -= neededAmmo; // Вычитаем добавленные патроны из allammo
            }
            else
            {
                ammo += allammo; // Добавляем все оставшиеся патроны
                allammo = 0; // Обнуляем allammo
            }

            // Обновляем патроны в интерфейсе магазина
            PlayerSwitchingStates.weaponBullets["AK74"] = allammo;
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
            ak47ammoSubject.OnNext(ammo); // Обновляем количество патронов в обойме
        }

        // Звуковой клип2
        if (Input.GetMouseButtonUp(0) || ammo <= 0) 
        {
            animator.SetBool("AutoFire", false);
        }
    }

    //Вызываем функцию в Update анимации
    // Внимание внимание! Эта строка обязательно должна остаться в анимации
    public void FireWepon(PlayerSwitchingStates playerSwitching,SoundManager soundManager)
    {

       

        if (Time.time > nextFire)
        {

            nextFire = Time.time + delay;
            DataShoot(playerSwitching,soundManager);

        }

        
    }

    // Вызываем функцию в анимации
    void DataShoot(PlayerSwitchingStates playerSwitching, SoundManager SoundM)
    {
        // Генерируем случайное значение pitch в диапазоне от 0.8 до 1.2
        float randomPitch = Random.Range(0.9f, 1.25f);

        // Устанавливаем pitch и воспроизводим звук выстрела с помощью PlayOneShot
        SoundM.FireAudio[3].pitch = randomPitch;
        SoundM.FireAudio[3].PlayOneShot(SoundM.FireAudio[3].clip);

        // Устанавливаем следующее время выстрела
        nextFire = Time.time + delay;

        // Луч нашего прицела
        Ray ray = playerSwitching.RayCastCameraCenter();

        // Наши GameObject
        GameObject bullet = playerSwitching.bullets;

        // Для системы событий
        playerSwitching.ShootInWall(playerSwitching.TraceShot(spread), bullet);

        // Добавляем себя в playerSwitching
        playerSwitching.rayWepon = playerSwitching.TraceShot(spread);

        ammo--;

        ak47ammoSubject.OnNext(ammo);
    }
}
