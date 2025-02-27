using UnityEngine;
using FpsZomby;
using UniRx;

public class PlayerBennelliBehaviour : IPlayerBehaviour,IStateFire,IStateReload
{

    Animator animator;
    PlayerSwitchingStates switchState;    
    SoundManager soundManager;
    int myIndex = 2;   
    int spread = 70; // Разброс
    static int ammo = 4; //Макс патронов
    float myDemage = 15;
    static int allammo;
    int maxMagazine = 8;
    

   public static readonly Subject<int> bennelliIntSubject = new();


    CompositeDisposable _disposable = new();

    void IPlayerBehaviour.Enter()
    {
        Debug.Log("���� _ Bennelli");



        animator = GameObject.FindGameObjectWithTag("Hand").GetComponent<Animator>();
        switchState = GameObject.FindObjectOfType<PlayerSwitchingStates>();
        soundManager = GameObject.FindObjectOfType<SoundManager>();

        bennelliIntSubject.OnNext(ammo);

        switchState.weponArray[myIndex].SetActive(true);

        animator.SetInteger("WeponNum", 2);

        switchState.Demage = myDemage;

        soundManager.SelectOther.Play();


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
        Debug.Log("����� _ Bennelli");


       

    }

    // �������
    void IPlayerBehaviour.Update()
    {
        if (Input.GetMouseButtonDown(0) && ammo > 0 && IsReloading() == false) 
        {            
            animator.SetTrigger("OnFire");

            //����� ���������� ������ � ��������
            animator.SetInteger("AllAmmoBennelli", allammo);
        }

        if (Input.GetMouseButtonDown(0) && ammo <= 0 && IsReloading() == false)
        {
            soundManager.noAmmoSound.Play();
        }

        // �����������
        if (Input.GetKeyDown(KeyCode.R) && ammo < 8 && allammo != 0) 
        {




            animator.SetTrigger("Reload");


            bennelliIntSubject.OnNext(ammo);


            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);

        }
        //���� ����������� ��� 0
        else if(ammo == 0 && allammo != 0)
        {
            animator.SetTrigger("Reload");


            bennelliIntSubject.OnNext(ammo);


            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
        }




        animator.SetInteger("Patron", ammo);
    }


    void SoundReload()
    {
        soundManager.ReloadAudio[1].Play();

    }

    // ������� ��������! ��� ������ ���������� ������ �������� � ��������
    public void FireWepon(PlayerSwitchingStates playerSwitching, SoundManager soundManager)
    {
        soundManager.FireAudio[1].Play();               //����
        playerSwitching.FireWeaponPart[1].Play();       //��������

        //��� ������ ������
        Ray ray = playerSwitching.RayCastCameraCenter();

        //���� Gameobject
        GameObject bullet = playerSwitching.bullets;

        //��� ������� 
        for (int i = 0; i < 7; i++)
        {
            playerSwitching.ShootInWall(playerSwitching.TraceShot(spread), bullet);

            // ��������� ���� � playerSwiching
            playerSwitching.rayWepon = playerSwitching.TraceShot(spread);
        }

        ammo--;
      
        bennelliIntSubject.OnNext(ammo);




    }


    //�����������
    public void Reload(PlayerSwitchingStates playerSwitching, Animator animator)
    {
        // ���������, ���� �� ����� � �������� � ���� �� ������� ��� �����������
        if (ammo < maxMagazine && allammo > 0)
        {            
            // ��������� ������ � ������� � ������� ��� �� ������ ����������
            ammo++;
            allammo--;


            // ��������� ������� � ���������� ���������
            PlayerSwitchingStates.weaponBullets["Bennelli_M4"] = allammo;
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
            bennelliIntSubject.OnNext(ammo); // ��������� ���������� �������� � ���������


            //����� ���������� ������ � ��������
            animator.SetInteger("AllAmmoBennelli", allammo);
        }
    }


   



    private bool IsReloading()
    {
        // �������� ������� ��������� ��������
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // ���������, ������������� �� ������� ��������� "BenneliReload"

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
