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

    public ParticleSystem[] FireWeaponPart; // Огонь от оружия

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


        //Добавление в Словарь
        weaponBullets["Gun"] = 0;
        weaponBullets["Bennelli_M4"] = 0;
        weaponBullets["AK74"] = 0;

        // Отправка словаря в Subject
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




        // Проверяем, движется ли игрок
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










    //Звук шагов
    private void PlayStepSound()
    {
        
        
         int currentStepIndex = 0;

        if (!soundManager.stepsSource.isPlaying) // Проверяем, не играет ли звук в данный момент
        {
            soundManager.stepsSource.clip = soundManager.Steps[currentStepIndex]; // Устанавливаем текущий звук
            soundManager.stepsSource.Play(); // Проигрываем звук

            // Переходим к следующему звуку
            currentStepIndex++;
            if (currentStepIndex >= soundManager.Steps.Length)
            {
                currentStepIndex = 0; // Сбрасываем индекс, если достигнут конец массива
            }
        }
    }




    private void StopStepSound()
    {
        if (soundManager.stepsSource.isPlaying) // Проверяем, играет ли звук
        {
            soundManager.stepsSource.Stop(); // Останавливаем звук
        }
    }





   



    private void OnEnable()
    {
        StateZombiAttack.ZombiEndAttack += ZombiAttack;



        //Пересечение коллайдера с слоем
        trigger.OnTriggerEnterAsObservable().Where(t => t.gameObject.layer == LayerMask.NameToLayer("Bullets")).Subscribe(other =>
            {


                int bullets = other.gameObject.GetComponent<BulletsTeam>().quantity;
                string weapon = other.gameObject.GetComponent<BulletsTeam>().weaponsBullets.ToString();



                // Проверка, существует ли ключ в словаре
                if (weaponBullets.ContainsKey(weapon))
                {
                    // Прибавляем количество пуль к текущему значению
                    weaponBullets[weapon] += bullets;
                }

                allBulletsDictSubject.OnNext(weaponBullets);

                var delayedEventStream = Observable.Return(weaponBullets[weapon]) // Эмитируем событие
                .Delay(System.TimeSpan.FromSeconds(1f)); // Задержка в 1 секунду


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

        soundManager.aScreamFromBlow.Play();//Звук от Player "Aauuf"

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

            soundManager.aScreamFromBlow.Stop();//Звук от Player "Aauuf"
        }
    }




    //Луч от центра камеры
    public Ray RayCastCameraCenter()
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        return ray;
    }





    





    //Состояния
    void SelectWepon()
    {

        switch (weapon)
        {
            // --------------------------------------------- Выдерга
            case Weapons.Crowbar:
              

                this.player.SetBehavior(player.SetBehaviourCrowbar());
                allBulletsDictSubject.OnNext(weaponBullets);

                break;


            // -------------------------------------------- Пистолет 
            case Weapons.Gun:

                this.player.SetBehavior(player.SetBehaviourGun());
                allBulletsDictSubject.OnNext(weaponBullets);
                break;

            // ---------------------------------------------- Дробовик
            case Weapons.Bennelli_M4:

                this.player.SetBehavior(player.SetBehaviorBennelli());
                allBulletsDictSubject.OnNext(weaponBullets);


                break;

            // -------------------------------------------------- Ак74 
            case Weapons.AK74:

                this.player.SetBehavior(player.SetBehaviorAk74());
                allBulletsDictSubject.OnNext(weaponBullets);

                break;


            default:


                break;
        }

    }












    // Смена оружия на цыфры
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

   

    // При поподании в стену / Обрабатываем поподание Луча
    public void ShootInWall(Ray ray, GameObject bullet)
    {
        // Настройки LayerMask для игнорирования слоёв при поподании
        int playerLayer = LayerMask.NameToLayer("Player");
        int ammoLayer = LayerMask.NameToLayer("Bullets");
        int colliderWall = LayerMask.NameToLayer("colliderWall");//колайдер невидемой стены 

        int layerMask = ~(1 << playerLayer | 1 << ammoLayer | 1 << colliderWall); // Инвертируем маску, чтобы игнорировать слои

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            // Получаем нормаль поверхности, на которую попал луч
            Vector3 surfaceNormal = hit.normal;

            // Вычисляем позицию для появления следа от пули
            spawnPosition = hit.point + (surfaceNormal * 0.008f); // Чтобы след от пули не находился внутри стены

            // Вычисляем правильное направление (сторону Z) для объекта следа от пули
            Vector3 spawnRotation = Quaternion.LookRotation(surfaceNormal) * Vector3.forward;

            // Создаем след от пули в случайной позиции и с правильным направлением

            //Момент выстрела

            //Присваеваем обьект в который был выстрел
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Zombi"))
            {
                //Кровь
                HitGameObject = hit.collider.gameObject;
                bullet = bulletBlood;

                //Звук поподания в зомби
                soundManager.onTarget.Play();

                // Включение изображения через UniRx
                OnTargetZombi.SetActive(true);
                IDisposable disposable = Observable.Timer(TimeSpan.FromSeconds(0.3f))
                    .Subscribe(_ => OnTargetZombi.SetActive(false));
            }

            else
            {
                //Камень
                bullet = bulletStone;
            }

            PlayerFire?.Invoke(Demage, hit); //поподание в зомби

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


