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
    static public int ammo = 12; //�������  
    int allammo;
    int maxMagazine = 12;
    bool ExitTime = false;



    public static readonly Subject<int> gunIntSubject = new();
    
    PlayerSwitchingStates switchState;
    SoundManager soundmanager;

    CompositeDisposable _disposable = new();
    

    public void Enter()
    {
        Debug.Log("���� _ Gun");      

        animator = GameObject.FindGameObjectWithTag("Hand").GetComponent<Animator>(); 
        switchState = GameObject.FindObjectOfType<PlayerSwitchingStates>();                
        soundmanager = GameObject.FindObjectOfType<SoundManager>();

        switchState.weponArray[myIndex].SetActive(true);

        animator.SetInteger("WeponNum", myIndex);        

        gunIntSubject.OnNext(ammo); // �������� � ������

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


        // ������� Observable ��� ������������ ������� ������� R
        Observable.EveryUpdate()

            .Where(_ => Input.GetKeyDown(KeyCode.R) && ammo < maxMagazine && allammo != 0) // ��������� �������            
            .Subscribe(_ => Reload()) // ������������� �� ������� ������� R
            .AddTo(_disposable);





        PlayerSwitchingStates.isDead.Subscribe(x =>
        {
            animator.SetInteger("WeponNum", -1);
        }).AddTo(_disposable);




    }

    

    public void Exit()
    {      
        Debug.Log("����� _ Gun");

        
       
    }




    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && ammo > 0 && IsReloading() == false) // ������ �������
        {


            animator.SetTrigger("OnFire");
            


        }

        if (Input.GetMouseButtonDown(0) && ammo <= 0 && IsReloading() == false)
        {
           
            soundmanager.noAmmoSound.Play();
        }

        



        //���� ��������� ������ �� ������ �����������
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
        // ����������, ������� �������� �� ������� ��� ���������� maxMagazine
        int neededAmmo = maxMagazine - ammo;

        if (allammo > 0) // ���������, ���� �� ������� ��� �����������
        {
            // ���������, ���������� �� �������� � allammo ��� ���������� �� maxMagazine
            if (allammo >= neededAmmo)
            {
                ammo += neededAmmo; // ��������� ����������� �������
                allammo -= neededAmmo; // �������� ����������� ������� �� allammo
            }
            else
            {
                ammo += allammo; // ��������� ��� ���������� �������
                allammo = 0; // �������� allammo
            }

            // ��������� ������� � ���������� ���������
            PlayerSwitchingStates.weaponBullets["Gun"] = allammo;
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
            gunIntSubject.OnNext(ammo); // ��������� ��������
        }
    }

    private bool IsReloading()
    {
        // �������� ������� ��������� ��������
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // ���������, ������������� �� ������� ��������� "GunReload"
        return stateInfo.IsName("GunReload");
    }

        



    // ������� ��������! ��� ������ ���������� ������ �������� � ��������
    public void FireWepon(PlayerSwitchingStates playerSwitching, SoundManager soundManager)
    {

        //���� � ��������
        soundManager.FireAudio[0].Play();
        playerSwitching.FireWeaponPart[0].Play();

        //���� Gameobject
        GameObject bullet = playerSwitching.bullets;

        //��� ������� �������
        playerSwitching.ShootInWall(playerSwitching.TraceShot(spread), bullet);


        // ��������� ���� � playerSwiching
        playerSwitching.rayWepon = playerSwitching.TraceShot(spread);

     

        ammo--;

        gunIntSubject.OnNext(ammo);
         

    }


    
}



