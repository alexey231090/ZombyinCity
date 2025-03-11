using UnityEngine;
using FpsZomby;
using UniRx;


public class PlayerGunBehaviur : IPlayerBehaviour, IStateFire
{
    Animator animator;
    Player player;
    int myIndex = 1;
    int spread = 10; // Разброс пуль
    float myDemage = 16;
    static public int ammo = 12; // Патроны
    int allammo;
    int maxMagazine = 12;

    public static readonly Subject<int> gunIntSubject = new();

    PlayerSwitchingStates switchState;
    SoundManager soundmanager;

    CompositeDisposable _disposable = new();

    private bool hasReloaded = false;// Флаг для отслеживания перезарядки

    public void Enter()
    {
        Debug.Log("Вход _ Gun");

        animator = GameObject.FindGameObjectWithTag("Hand").GetComponent<Animator>();
        switchState = GameObject.FindObjectOfType<PlayerSwitchingStates>();
        soundmanager = GameObject.FindObjectOfType<SoundManager>();

        switchState.weponArray[myIndex].SetActive(true);

        animator.SetInteger("WeponNum", myIndex);

        gunIntSubject.OnNext(ammo); // Обновление количества патронов

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

        // Создание Observable для обработки нажатия клавиши R
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.R) && ammo < maxMagazine && allammo != 0) // Проверка условий перезарядки
            .Subscribe(_ => Reload()) // Выполнение перезарядки
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
        if (Input.GetMouseButtonDown(0) && ammo > 0 && IsReloading() == false) // Выстрел
        {
            animator.SetTrigger("OnFire");
        }

        if (Input.GetMouseButtonDown(0) && ammo <= 0 && IsReloading() == false)
        {
            soundmanager.noAmmoSound.Play();
        }

        CheckAndReload();
    }

    private void Reload()
    {
        animator.SetBool("ReloadBool", true);
        soundmanager.ReloadAudio[0].Play();

        
    }





    void FinishRelad()
    {
        // Определение количества патронов, необходимых для полной перезарядки
        int neededAmmo = maxMagazine - ammo;

        if (allammo > 0) // Проверка наличия патронов для перезарядки
        {
            // Проверка, достаточно ли патронов в запасе для полной перезарядки
            if (allammo >= neededAmmo)
            {
                ammo += neededAmmo; // Добавление патронов в магазин
                allammo -= neededAmmo; // Уменьшение количества патронов в запасе
            }
            else
            {
                ammo += allammo; // Добавление всех оставшихся патронов в магазин
                allammo = 0; // Обнуление запаса патронов
            }

            // Обновление данных о патронах
            PlayerSwitchingStates.weaponBullets["Gun"] = allammo;
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
            gunIntSubject.OnNext(ammo); // Обновление количества патронов
            hasReloaded = false;
        }
    }

    private bool IsReloading()
    {
        // Проверка состояния анимации перезарядки
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // Проверка, выполняется ли анимация "GunReload"
        return stateInfo.IsName("GunReload");
    }

    // Метод для стрельбы. Этот метод вызывается анимацией
    public void FireWepon(PlayerSwitchingStates playerSwitching, SoundManager soundManager)
    {
        // Звук выстрела
        soundManager.FireAudio[0].Play();
        playerSwitching.FireWeaponPart[0].Play();

        // Создание объекта пули
        GameObject bullet = playerSwitching.bullets;

        // Трассировка выстрела
        playerSwitching.ShootInWall(playerSwitching.TraceShot(spread), bullet);

        // Обновление данных о выстреле
        playerSwitching.rayWepon = playerSwitching.TraceShot(spread);

        ammo--;

        gunIntSubject.OnNext(ammo);
    }
    

   

    private void CheckAndReload()
    {
        if (ammo == 0 && allammo > 0 && !hasReloaded)
        {
            Reload();
            hasReloaded = true;
        }
    }
}


