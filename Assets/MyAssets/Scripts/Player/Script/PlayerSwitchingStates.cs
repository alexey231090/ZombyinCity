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
    public static event Action<float, RaycastHit> PlayerFire;
    public static int playerLife = 100;

    private readonly int allBullets = 0;
    public Animator AnimatorIsDead;
    public Animator hendAnim;

    public static bool CrowbarIsActive, GunIsActive, BennelliIsActive, AK74IsActive = false;

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
    public ParticleSystem[] FireWeaponPart; // Эффекты частиц для оружия
    public AudioSource[] FireGunAudio;
    public new Camera camera;

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
    public static Subject<Dictionary<string, int>> allBulletsDictSubject = new();
    public static Subject<Unit> isDead = new();
    public static Subject<string> selectWepon = new();

    [Inject]
    SoundManager soundManager;

    bool isGround = true;
    bool isDown = false;
    PlayerController playerController;
    public GameObject OnTargetZombi;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        weapon = Weapons.None;
        playerLife = 100;
        lifeIntSubject.OnNext(playerLife);
        previousWeapon = weapon;
        SelectWepon();

        // Инициализация словаря
        weaponBullets["Gun"] = 0;
        weaponBullets["Bennelli_M4"] = 0;
        weaponBullets["AK74"] = 0;

        // Отправляем словарь в Subject
        allBulletsDictSubject.OnNext(weaponBullets);

        SubscribeToPlayerController();
    }

    private void Update()
    {
        if (weapon != previousWeapon)
        {
            SelectWepon();
            previousWeapon = weapon;
        }

        InputWepon();
        HandleMovementSound();
    }

    private void OnEnable()
    {
        StateZombiAttack.ZombiEndAttack += ZombiAttack;

        // Подписываемся на события триггера для подбора патронов
        trigger.OnTriggerEnterAsObservable()
            .Where(t => t.gameObject.layer == LayerMask.NameToLayer("Bullets"))
            .Subscribe(other =>
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

                Destroy(other.gameObject);
                soundManager.UppAmmo.Play();
            }).AddTo(_disposable);
    }

    private void OnDisable()
    {
        StateZombiAttack.ZombiEndAttack -= ZombiAttack;
        _disposable.Dispose();
    }


    // Подписка на события PlayerController
    private void SubscribeToPlayerController()
    {
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


    // Обработка звука шагов
    private void HandleMovementSound()
    {
        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && isGround)
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


    // Звук прыжка
    private void JumpSound(bool isGround)
    {
        if (isGround)
        {
            soundManager.JumpSurce[0].Play();
        }
    }

    // Изменения питч звука шагов
    public void WalcingPitchAudio(bool run, AudioSource stepsSurce)
    {
        stepsSurce.pitch = run ? 1.16f : 1f;
    }


    // Проигрывание звука шагов
    private void PlayStepSound()
    {
        int currentStepIndex = 0;

        if (!soundManager.stepsSource.isPlaying)
        {
            soundManager.stepsSource.clip = soundManager.Steps[currentStepIndex];
            soundManager.stepsSource.Play();

            currentStepIndex++;
            if (currentStepIndex >= soundManager.Steps.Length)
            {
                currentStepIndex = 0;
            }
        }
    }

    // Остановка звука шагов
    private void StopStepSound()
    {
        if (soundManager.stepsSource.isPlaying)
        {
            soundManager.stepsSource.Stop();
        }
    }

    // Атака зомби
    private void ZombiAttack()
    {
        playerLife -= 13;
        soundManager.aScreamFromBlow.Play();
        PlayerIsDead(playerLife);
        lifeIntSubject.OnNext(playerLife);
    }


    // Проверка жизни игрока
    private void PlayerIsDead(int playerLife)
    {
        if (playerLife < 0)
        {
            isDead.OnNext(Unit.Default);
            playerController.enabled = false;
            AnimatorIsDead.SetBool("isDead", true);
            soundManager.aScreamFromBlow.Stop();
        }
    }

    // Луч из центра камеры
    public Ray RayCastCameraCenter()
    {
        return camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
    }

    // Выбор оружия
    private void SelectWepon()
    {
        switch (weapon)
        {
            case Weapons.None:
                hendAnim.SetInteger("WeponNum", -1);
                foreach (GameObject wepon in weponArray)
                {
                    wepon.SetActive(false);
                }
                break;

            case Weapons.Crowbar:
                player.SetBehavior(player.SetBehaviourCrowbar());
                allBulletsDictSubject.OnNext(weaponBullets);
                break;

            case Weapons.Gun:
                player.SetBehavior(player.SetBehaviourGun());
                allBulletsDictSubject.OnNext(weaponBullets);
                break;

            case Weapons.Bennelli_M4:
                player.SetBehavior(player.SetBehaviorBennelli());
                allBulletsDictSubject.OnNext(weaponBullets);
                break;

            case Weapons.AK74:
                player.SetBehavior(player.SetBehaviorAk74());
                allBulletsDictSubject.OnNext(weaponBullets);
                break;

            default:
                break;
        }
    }

    // Обработка ввода оружия
    private void InputWepon()
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
                Debug.Log(weapon+"------------------!!!!!!!!true");
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
        int playerLayer = LayerMask.NameToLayer("Player");
        int ammoLayer = LayerMask.NameToLayer("Bullets");
        int colliderWall = LayerMask.NameToLayer("colliderWall");

        int layerMask = ~(1 << playerLayer | 1 << ammoLayer | 1 << colliderWall);


        // Проверка столкновения луча с объектом
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            Vector3 surfaceNormal = hit.normal;
            spawnPosition = hit.point + (surfaceNormal * 0.008f);
            Vector3 spawnRotation = Quaternion.LookRotation(surfaceNormal) * Vector3.forward;

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Zombi"))
            {
                HitGameObject = hit.collider.gameObject;
                bullet = bulletBlood;
                soundManager.onTarget.Play();

                OnTargetZombi.SetActive(true);
                Observable.Timer(TimeSpan.FromSeconds(0.3f))
                    .Subscribe(_ => OnTargetZombi.SetActive(false))
                    .AddTo(_disposable);
            }
            else
            {
                bullet = bulletStone;
            }

            // Вызов события стрельбы
            PlayerFire?.Invoke(Demage, hit);
            Instantiate(bullet, spawnPosition, Quaternion.LookRotation(spawnRotation));
        }
    }

    // Создание луча с разбросом
    public Ray TraceShot(float radius)
    {
        Vector2 randomOffset = Random.insideUnitCircle * radius;
        return camera.ScreenPointToRay(new Vector3(Screen.width / 2 + randomOffset.x, Screen.height / 2 + randomOffset.y, 0));
    }

    // Задержка выполнения действия
    private IEnumerator Delay(Action action, float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }

    // Запуск действия с задержкой
    public void ShootDelay(Action function, float delay)
    {
        StartCoroutine(Delay(function, delay));
    }
}
