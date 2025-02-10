using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using UniRx;
using UniRx.Triggers;
using System.Collections.Generic;
using Zenject;
using EvolveGames;
using UnityEngine.UI;


public class PlayerSwitchingStates : MonoBehaviour
{  

    public float Demage = 0;
    public static event Action<float,RaycastHit> PlayerFire;

    public static int playerLife = 100;

    private readonly int allBullets = 0;

    public Animator AnimatorIsDead;

    public enum Weapons
    {
        Crowbar,
        Gun,
        Bennelli_M4,
        AK74

    }
    public static Weapons weapon;

    private Weapons previousWeapon;

    public Player player;    

    public GameObject[] weponArray;   

    public ParticleSystem[] FireWeaponPart; // ����� �� ������

    public AudioSource[] FireGunAudio;

    public new Camera  camera;

    Vector3 spawnPosition;   

    public GameObject bullets;
    public GameObject bulletBlood;
    public GameObject bulletStone;

    public GameObject HitGameObject;

    public Ray rayWepon;



    CompositeDisposable _disposable = new CompositeDisposable();


    public Collider trigger;
    public static readonly Subject<int> lifeIntSubject = new();


    public static Dictionary<string, int> weaponBullets = new();
    public static Subject<Dictionary<string,int>> allBulletsDictSubject = new();
    public static Subject<Unit> isDead = new ();

    public static Subject<string> selectWepon = new();

    [Inject]
    SoundManager soundManager;

    bool isGround = true;
    bool isDown = false;
    
   
    PlayerController playerController;

    public GameObject OnTargetZombi;


    private void Start()
    {
        playerController = this.GetComponent<PlayerController>();
              

        playerLife = 100;

        lifeIntSubject.OnNext(playerLife);

        previousWeapon = weapon;

        SelectWepon();


        //���������� � �������
        weaponBullets["Gun"] = 0;
        weaponBullets["Bennelli_M4"] = 0;
        weaponBullets["AK74"] = 0;

        // �������� ������� � Subject
        allBulletsDictSubject.OnNext(weaponBullets);



        PlayerController.isGround.Subscribe(value => 
        {       
            isGround = value; 

            JumpSound(isGround);
            
        }).AddTo(_disposable);

        PlayerController.isRun.Subscribe(value =>
        {

            WalcingPitchAudio(value, soundManager.stepsSource);

        }).AddTo(_disposable);

        PlayerController.isDown.Subscribe(value =>
        {

            isDown = value;


        }).AddTo(_disposable);
        
        


    }











    private void Update()
    {

        

        if (weapon != previousWeapon)
        {
            SelectWepon();
            previousWeapon = weapon;
        }



        InputWepon();




        // ���������, �������� �� �����
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 && isGround == true)
        {

            if (!isDown)
            {

                PlayStepSound();
            }
        }
        else
        {
            StopStepSound();
        }




    }





    void JumpSound(bool isGround)
    {
        if (isGround)
        {
            soundManager.JumpSurce[0].Play();

        }
    }


    public void WalcingPitchAudio(bool run, AudioSource stepsSurce)
    {
        if (run)
        {
            stepsSurce.pitch = 1.16f;
            
        }
        else
        {
            stepsSurce.pitch = 1f;
        }



    }










    //���� �����
    private void PlayStepSound()
    {
        
        
         int currentStepIndex = 0;

        if (!soundManager.stepsSource.isPlaying) // ���������, �� ������ �� ���� � ������ ������
        {
            soundManager.stepsSource.clip = soundManager.Steps[currentStepIndex]; // ������������� ������� ����
            soundManager.stepsSource.Play(); // ����������� ����

            // ��������� � ���������� �����
            currentStepIndex++;
            if (currentStepIndex >= soundManager.Steps.Length)
            {
                currentStepIndex = 0; // ���������� ������, ���� ��������� ����� �������
            }
        }
    }




    private void StopStepSound()
    {
        if (soundManager.stepsSource.isPlaying) // ���������, ������ �� ����
        {
            soundManager.stepsSource.Stop(); // ������������� ����
        }
    }





   



    private void OnEnable()
    {
        StateZombiAttack.ZombiEndAttack += ZombiAttack;



        //����������� ���������� � �����
        trigger.OnTriggerEnterAsObservable().Where(t => t.gameObject.layer == LayerMask.NameToLayer("Bullets")).Subscribe(other =>
            {


                int bullets = other.gameObject.GetComponent<BulletsTeam>().quantity;
                string weapon = other.gameObject.GetComponent<BulletsTeam>().weaponsBullets.ToString();



                // ��������, ���������� �� ���� � �������
                if (weaponBullets.ContainsKey(weapon))
                {
                    // ���������� ���������� ���� � �������� ��������
                    weaponBullets[weapon] += bullets;
                }

                allBulletsDictSubject.OnNext(weaponBullets);

                var delayedEventStream = Observable.Return(weaponBullets[weapon]) // ��������� �������
                .Delay(System.TimeSpan.FromSeconds(1f)); // �������� � 1 �������


                Destroy(other.gameObject);
                soundManager.UppAmmo.Play();

            }).AddTo(_disposable);


    


    }

    private void OnDisable()
    {
        StateZombiAttack.ZombiEndAttack -= ZombiAttack;

        _disposable.Dispose();
    }





    void ZombiAttack()
    { 
        playerLife -= 13;

        soundManager.aScreamFromBlow.Play();//���� �� Player "Aauuf"

        PlayerIsDead(playerLife);

        lifeIntSubject.OnNext(playerLife);
    }




   void PlayerIsDead(int playerLife)
    {
        if (playerLife < 0)
        {
            isDead.OnNext(Unit.Default);

            playerController.enabled = false;

            AnimatorIsDead.SetBool("isDead", true);

            soundManager.aScreamFromBlow.Stop();//���� �� Player "Aauuf"
        }
    }




    //��� �� ������ ������
    public Ray RayCastCameraCenter()
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        return ray;
    }





    





    //���������
    void SelectWepon()
    {

        switch (weapon)
        {
            // --------------------------------------------- �������
            case Weapons.Crowbar:
              

                this.player.SetBehavior(player.SetBehaviourCrowbar());
                allBulletsDictSubject.OnNext(weaponBullets);

                break;


            // -------------------------------------------- �������� 
            case Weapons.Gun:

                this.player.SetBehavior(player.SetBehaviourGun());
                allBulletsDictSubject.OnNext(weaponBullets);
                break;

            // ---------------------------------------------- ��������
            case Weapons.Bennelli_M4:

                this.player.SetBehavior(player.SetBehaviorBennelli());
                allBulletsDictSubject.OnNext(weaponBullets);


                break;

            // -------------------------------------------------- ��74 
            case Weapons.AK74:

                this.player.SetBehavior(player.SetBehaviorAk74());
                allBulletsDictSubject.OnNext(weaponBullets);

                break;


            default:


                break;
        }

    }












    // ����� ������ �� �����
    void InputWepon()
    {
        if (playerLife > 0)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                weapon = Weapons.Crowbar;

                selectWepon.OnNext(weapon.ToString());

            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {

                weapon = Weapons.Gun;

                selectWepon.OnNext(weapon.ToString());

            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {


                weapon = Weapons.Bennelli_M4;

                selectWepon.OnNext(weapon.ToString());

            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                weapon = Weapons.AK74;

                selectWepon.OnNext(weapon.ToString());

            }
        }
    }

   

    // ��� ��������� � ����� / ������������ ��������� ����
    public void ShootInWall(Ray ray, GameObject bullet)
    {
        // ��������� LayerMask ��� ������������� ���� ��� ���������
        int playerLayer = LayerMask.NameToLayer("Player");
        int ammoLayer = LayerMask.NameToLayer("Bullets");
        int colliderWall = LayerMask.NameToLayer("colliderWall");//�������� ��������� ����� 

        int layerMask = ~(1 << playerLayer | 1 << ammoLayer | 1 << colliderWall); // ����������� �����, ����� ������������ ����

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            // �������� ������� �����������, �� ������� ����� ���
            Vector3 surfaceNormal = hit.normal;

            // ��������� ������� ��� ��������� ����� �� ����
            spawnPosition = hit.point + (surfaceNormal * 0.008f); // ����� ���� �� ���� �� ��������� ������ �����

            // ��������� ���������� ����������� (������� Z) ��� ������� ����� �� ����
            Vector3 spawnRotation = Quaternion.LookRotation(surfaceNormal) * Vector3.forward;

            // ������� ���� �� ���� � ��������� ������� � � ���������� ������������

            //������ ��������

            //����������� ������ � ������� ��� �������
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Zombi"))
            {
                //�����
                HitGameObject = hit.collider.gameObject;
                bullet = bulletBlood;

                //���� ��������� � �����
                soundManager.onTarget.Play();

                // ��������� ����������� ����� UniRx
                OnTargetZombi.SetActive(true);
                IDisposable disposable = Observable.Timer(TimeSpan.FromSeconds(0.3f))
                    .Subscribe(_ => OnTargetZombi.SetActive(false));
            }

            else
            {
                //������
                bullet = bulletStone;
            }

            PlayerFire?.Invoke(Demage, hit); //��������� � �����

            GameObject bulletHole = Instantiate(bullet, spawnPosition, Quaternion.LookRotation(spawnRotation));
        }
    }












    public Ray TraceShot(float radius)
    {      

        Vector2 randomOffset = Random.insideUnitCircle * radius;

        Ray  ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2 + randomOffset.x, Screen.height / 2 + randomOffset.y, 0));

        return ray;

    }











    private IEnumerator Delay( Action action,float delay )
    {


        while (true)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

    }












    public void ShootDelay(Action function, float delay)
    {
        StartCoroutine(Delay(function, delay));
    }

}


