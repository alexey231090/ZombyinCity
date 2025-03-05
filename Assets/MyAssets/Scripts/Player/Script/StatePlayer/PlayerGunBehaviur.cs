using UnityEngine;
using FpsZomby;
using UniRx;

public class PlayerGunBehaviur : IPlayerBehaviour, IStateFire
{
    Animator animator;
    Player player;
    int myIndex = 1;
    int spread = 10; // Разброс выстрелов
    float myDemage = 16;
    static public int ammo = 12; // Патроны
    int allammo;
    int maxMagazine = 12;
    bool isReloading = false;

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

        SubscribeToSubjects();
        SetupReloadObservable();
    }

    public void Exit()
    {
        Debug.Log("Выход _ Gun");
        _disposable.Dispose();
    }

    public void Update()
    {
        HandleFireInput();
        HandleReloadInput();
    }

    private void HandleFireInput()
    {
        if (Input.GetMouseButtonDown(0) && ammo > 0 && !isReloading)
        {
            animator.SetTrigger("OnFire");
        }
        else if (Input.GetMouseButtonDown(0) && ammo <= 0 && !isReloading)
        {
            soundmanager.noAmmoSound.Play();
        }
    }

    private void HandleReloadInput()
    {
        if (ammo == 0 && allammo != 0 && !isReloading)
        {
            Reload();
        }
    }

    private void Reload()
    {
        if (isReloading) return;

        soundmanager.ReloadAudio[0].Play();
        animator.SetBool("ReloadBool", true);
        isReloading = true;
    }

    private void FinishRelad()
    {
        isReloading = false;
        int neededAmmo = maxMagazine - ammo;

        if (allammo > 0)
        {
            if (allammo >= neededAmmo)
            {
                ammo += neededAmmo;
                allammo -= neededAmmo;
            }
            else
            {
                ammo += allammo;
                allammo = 0;
            }

            PlayerSwitchingStates.weaponBullets["Gun"] = allammo;
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
            gunIntSubject.OnNext(ammo);
        }
    }

    private bool IsReloading()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("GunReload");
    }

    public void FireWepon(PlayerSwitchingStates playerSwitching, SoundManager soundManager)
    {
        soundManager.FireAudio[0].Play();
        playerSwitching.FireWeaponPart[0].Play();

        GameObject bullet = playerSwitching.bullets;
        playerSwitching.ShootInWall(playerSwitching.TraceShot(spread), bullet);

        playerSwitching.rayWepon = playerSwitching.TraceShot(spread);

        ammo--;
        gunIntSubject.OnNext(ammo);
    }

    private void SubscribeToSubjects()
    {
        PlayerSwitchingStates.allBulletsDictSubject
            .Subscribe(value => { allammo = value["Gun"]; })
            .AddTo(_disposable);

        StateReload.reloadGunSubject
            .Subscribe(_ => FinishRelad())
            .AddTo(_disposable);

        PlayerSwitchingStates.isDead
            .Subscribe(_ => animator.SetInteger("WeponNum", -1))
            .AddTo(_disposable);
    }

    private void SetupReloadObservable()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.R) && ammo < maxMagazine && allammo != 0)
            .Subscribe(_ => Reload())
            .AddTo(_disposable);
    }
}
