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

    public Animator hendAnim;

   static public bool CrowbarIsActive,GunIsActive,BennelliIsActive,AK74IsActive = false;

    public enum Weapons
    {
        None,
        Crowbar,
        Gun,
        Bennelli_M4,
        AK74
        

    }
    public static Weapons weapon;

    private Weapons previousWeapon;

    public Player player;    

    public GameObject[] weponArray;   

    public ParticleSystem[] FireWeaponPart; // Particle effects for weapons

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

        weapon = Weapons.None;

        playerLife = 100;

        lifeIntSubject.OnNext(playerLife);

        previousWeapon = weapon;

        SelectWepon();


        // Initialize dictionary
        weaponBullets["Gun"] = 0;
        weaponBullets["Bennelli_M4"] = 0;
        weaponBullets["AK74"] = 0;

        // Send dictionary to Subject
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




        // Проверяем, двигается ли игрок
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










    // Воспроизведение звуков шагов
    private void PlayStepSound()
    {
        
        
         int currentStepIndex = 0;

        if (!soundManager.stepsSource.isPlaying) // Проверяем, не воспроизводится ли уже звук
        {
            soundManager.stepsSource.clip = soundManager.Steps[currentStepIndex]; // Устанавливаем текущий звук
            soundManager.stepsSource.Play(); // Воспроизводим звук

            // Переходим к следующему звуку
            currentStepIndex++;
            if (currentStepIndex >= soundManager.Steps.Length)
            {
                currentStepIndex = 0; // Сбрасываем счетчик, если достигнут конец массива
            }
        }
    }




    private void StopStepSound()
    {
        if (soundManager.stepsSource.isPlaying) // Проверяем, воспроизводится ли звук
        {
            soundManager.stepsSource.Stop(); // Останавливаем звук
        }
    }





   



    private void OnEnable()
    {
        StateZombiAttack.ZombiEndAttack += ZombiAttack;



        // Подписываемся на события триггера для подбора патронов
        trigger.OnTriggerEnterAsObservable().Where(t => t.gameObject.layer == LayerMask.NameToLayer("Bullets")).Subscribe(other =>
            {


                int bullets = other.gameObject.GetComponent<BulletsTeam>().quantity;
                string weapon = other.gameObject.GetComponent<BulletsTeam>().weaponsBullets.ToString();



                // Проверяем, существует ли оружие в словаре
                if (weaponBullets.ContainsKey(weapon))
                {
                    // Добавляем патроны к существующему количеству оружия
                    weaponBullets[weapon] += bullets;
                }

                allBulletsDictSubject.OnNext(weaponBullets);

                var delayedEventStream = Observable.Return(weaponBullets[weapon]) // Создаем событие
                .Delay(System.TimeSpan.FromSeconds(1f)); // Задержка 1 секунда


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

        soundManager.aScreamFromBlow.Play();// Воспроизводим звук "Ааууф" игрока

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

            soundManager.aScreamFromBlow.Stop();// Останавливаем звук "Ааууф" игрока
        }
    }




    // Луч из центра камеры
    public Ray RayCastCameraCenter()
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        return ray;
    }





    





    // Выбор оружия
    void SelectWepon()
    {

        switch (weapon)
        {


            // --------------------------------------------- Без оружия
            case Weapons.None:


                print("No weapon");

                hendAnim.SetInteger("WeponNum", -1);

               foreach(GameObject wepon in weponArray)
                {
                    wepon.SetActive(false);
                    
                }

                break;



            // --------------------------------------------- Монтировка
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

            // -------------------------------------------------- АК74
            case Weapons.AK74:

                this.player.SetBehavior(player.SetBehaviorAk74());
                allBulletsDictSubject.OnNext(weaponBullets);

                break;


            default:


                break;
        }

    }












    // Обработка ввода оружия
    void InputWepon()
    {
        if (playerLife > 0)
        {


            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                weapon = Weapons.None;

                selectWepon.OnNext(weapon.ToString());

            }




            if (Input.GetKeyDown(KeyCode.Alpha1) && CrowbarIsActive)
            {
                weapon = Weapons.Crowbar;

                selectWepon.OnNext(weapon.ToString());

            }

            if (Input.GetKeyDown(KeyCode.Alpha2) && GunIsActive)
            {

                weapon = Weapons.Gun;

                selectWepon.OnNext(weapon.ToString());

            }

            if (Input.GetKeyDown(KeyCode.Alpha3) && BennelliIsActive)
            {


                weapon = Weapons.Bennelli_M4;

                selectWepon.OnNext(weapon.ToString());

            }

            if (Input.GetKeyDown(KeyCode.Alpha4) && AK74IsActive)
            {
                weapon = Weapons.AK74;

                selectWepon.OnNext(weapon.ToString());

            }
        }
    }

   

    // Обработка стрельбы по стенам/обнаружение столкновений
    public void ShootInWall(Ray ray, GameObject bullet)
    {
        // Создаем LayerMask для игнорирования определенных слоев
        int playerLayer = LayerMask.NameToLayer("Player");
        int ammoLayer = LayerMask.NameToLayer("Bullets");
        int colliderWall = LayerMask.NameToLayer("colliderWall");// Слой коллизии стены

        int layerMask = ~(1 << playerLayer | 1 << ammoLayer | 1 << colliderWall); // Объединяем слои для игнорирования

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            // Получаем нормаль поверхности в точке попадания
            Vector3 surfaceNormal = hit.normal;

            // Вычисляем позицию появления с небольшим смещением от поверхности
            spawnPosition = hit.point + (surfaceNormal * 0.008f); // Небольшое смещение для предотвращения z-fighting

            // Вычисляем поворот на основе нормали поверхности
            Vector3 spawnRotation = Quaternion.LookRotation(surfaceNormal) * Vector3.forward;

            // Создаем эффект пулевого отверстия

            // Проверяем слой объекта попадания и обрабатываем соответственно
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Zombi"))
            {
                // Эффект крови
                HitGameObject = hit.collider.gameObject;
                bullet = bulletBlood;

                // Звук попадания
                soundManager.onTarget.Play();

                // Показываем маркер попадания используя UniRx
                OnTargetZombi.SetActive(true);
                IDisposable disposable = Observable.Timer(TimeSpan.FromSeconds(0.3f))
                    .Subscribe(_ => OnTargetZombi.SetActive(false));
            }

            else
            {
                // Эффект камня
                bullet = bulletStone;
            }

            PlayerFire?.Invoke(Demage, hit); // Уведомляем об уроне

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
