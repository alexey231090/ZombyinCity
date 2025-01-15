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
    float delay = 0.15f;  //�������� ����� ����������
    float nextFire;
    float spread = 30f; //������� ��������
    static public int ammo = 30; //��� ��������    
    int maxMagazine = 30;
    int allammo;


    CompositeDisposable _disposable = new();

    public void Enter()
    {
        Debug.Log("���� _ AK74");
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
        Debug.Log("����� _ AK74");

        animator.SetBool("AutoFire", false);

    }

   

    public void Update()
    {

        // ������� ���
        if (Input.GetMouseButtonDown(0) && ammo > 0) 
        {
            animator.SetBool("AutoFire", true);
                   

        }

        //���� ��� ��������
        if (Input.GetMouseButtonDown(0) && ammo <= 0)
        {
            
            SoundManager.noAmmoSound.Play();

        }


        // �����������
        else if (Input.GetKeyDown(KeyCode.R) && ammo < 30 && animator.GetBool("AutoFire") == false && allammo != 0)
        {

            animator.SetTrigger("Reload");
            
            SoundManager.ReloadAudio[2].Play();

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
                PlayerSwitchingStates.weaponBullets["AK74"] = allammo;
                PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
                ak47ammoSubject.OnNext(ammo); // ��������� ���������� �������� � ������
            }


            ak47ammoSubject.OnNext(ammo);

           
        }

        // ������� ����2
        if (Input.GetMouseButtonUp(0) || ammo <= 0) 
        {
            animator.SetBool("AutoFire", false);

           
          
        }
       

    }

    //������� ������� � Update ��������    
    // ������� ��������! ��� ������ ���������� ������ �������� � ��������
    public void FireWepon(PlayerSwitchingStates playerSwitching,SoundManager soundManager)
    {

       

        if (Time.time > nextFire)
        {

            nextFire = Time.time + delay;
            DataShoot(playerSwitching,soundManager);

        }

        
    }

    // �������� � ������ ��� ��������
    void DataShoot(PlayerSwitchingStates playerSwitching, SoundManager SoundM)
    {
        // ���������� ��������� �������� pitch � ��������� �� 0.8 �� 1.2
        float randomPitch = Random.Range(0.9f, 1.25f);

        // ������������� pitch � ������������� ���� �������� � ������� PlayOneShot
        SoundM.FireAudio[3].pitch = randomPitch;
        SoundM.FireAudio[3].PlayOneShot(SoundM.FireAudio[3].clip);

        // ������������� ���������� ������� ��������
        playerSwitching.FireWeaponPart[2].Play();

        // ��� ������ ������
        Ray ray = playerSwitching.RayCastCameraCenter();

        // ���� GameObject
        GameObject bullet = playerSwitching.bullets;

        // ��� ������� �������
        playerSwitching.ShootInWall(playerSwitching.TraceShot(spread), bullet);

        // ��������� ���� � playerSwitching
        playerSwitching.rayWepon = playerSwitching.TraceShot(spread);

        ammo--;

        ak47ammoSubject.OnNext(ammo);
    }
}
