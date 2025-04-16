using UnityEngine;
using TMPro;
using UniRx;
using System;
using UnityEngine.UI;



public class CanvasFirstPerson : MonoBehaviour
{
   
    public TextMeshProUGUI canvasAmmoText;
    public TextMeshProUGUI PlayerLifeText;
    public TextMeshProUGUI canvasPlayerShopWepon;

    public GameObject FaceBlood;

    public GameObject[] AmmoIcon; // Массив иконок патронов

    Animator animatorIcon;
    TextMeshProUGUI textAmmo;

    static public int canvasAmmoInt = 0;

    Image faceBloodImage;

    CompositeDisposable _disposable = new();

    private void Start()
    {
        // Получаем компонент Image из дочернего объекта FaceBlood
        faceBloodImage = FaceBlood.GetComponentInChildren<Image>();
        // Устанавливаем начальную прозрачность в 0
        SetImageAlpha(0);
    }


    private void SetImageAlpha(float alpha)
    {
        Color color = faceBloodImage.color; // Получаем текущий цвет
        color.a = alpha; // Устанавливаем новый alpha
        faceBloodImage.color = color; // Присваиваем цвет обратно
    }


    private void OnEnable()
    {
        var ak47Observable = PlayerAk74Behaviour.ak47ammoSubject
            .Select(value => value.ToString());

        var gunObservable = PlayerGunBehaviur.gunIntSubject
            .Select(value => value.ToString());

        var bennObservable = PlayerBennelliBehaviour.bennelliIntSubject
            .Select(value => value.ToString());

        var crowObservable = PlayerCrowbarBehaviur.crowIntSubject
            .Select(value => value.ToString());

        // Объединяем все Observable Патроны в магазине
        ak47Observable.Merge(gunObservable, bennObservable, crowObservable)
            .Subscribe(value =>
            {
                canvasAmmoText.text = value;
            })
            .AddTo(_disposable);

        // Жизнь
        PlayerSwitchingStates.lifeIntSubject.Subscribe(value =>
        {
            if (value < 0)
            {
                PlayerLifeText.text = 0.ToString();
            }
            else
            {
                PlayerLifeText.text = value.ToString();
            }
        }).AddTo(_disposable);

        // Кровь на лице
        PlayerSwitchingStates.lifeIntSubject
            .Skip(1)
            .Subscribe(value =>
            {
                // Устанавливаем высокую прозрачность
                SetImageAlpha(180f / 255f);

                // Плавное затухание
                Observable.Range(0, 100) // 100 кадров для затухания
                    .Select(i => (float)i / 100f) // Нормализуем значения от 0 до 1
                    .Delay(TimeSpan.FromSeconds(0.01f)) // Задержка между кадрами
                    .Subscribe(t =>
                    {
                        float newAlpha = Mathf.Lerp(180f / 255f, 0f, t); // Линейная интерполяция
                        SetImageAlpha(newAlpha);
                    })
                    .AddTo(_disposable);
            })
            .AddTo(_disposable);

        PlayerSwitchingStates.allBulletsDictSubject.Subscribe(value =>
        {
            // Общее количество патронов на оружия
            string weaponKey = PlayerSwitchingStates.weapon.ToString();

            // Проверяем, есть ли ключ в словаре
            if (value.TryGetValue(weaponKey, out int bulletCount))
            {
                canvasPlayerShopWepon.text = "/" + bulletCount.ToString();
            }
            else
            {
                // Обработка случая, если ключ не найден
                canvasPlayerShopWepon.text = "/0";
            }
        }).AddTo(_disposable);

        // Устанавливаем количество жизни на 0 при смерти Player
        PlayerSwitchingStates.isDead.Subscribe(value =>
        {
            print("isDead");
        }).AddTo(_disposable);







        // Подписка на событие в __EffectAddBullets Value содержит enum и int от BulletsTeam
        __EffectIconAddBullets.bulletsSubject.Subscribe(value =>
        {
            switch (value.Key)
            {
                //Gun
                case BulletsTeam.BulletWeapons.Gun:

                    //Если включен то меняем текст и запускаем анимацию
                    if (AmmoIcon[0].activeInHierarchy)
                    {
                        
                        Animator animatorIcon = AmmoIcon[0].GetComponent<Animator>();
                        TextMeshProUGUI textAmmo = AmmoIcon[0].GetComponentInChildren<TextMeshProUGUI>();
                        textAmmo.text = "+" + value.Value.ToString();
                        animatorIcon.SetTrigger("Add");
                    }
                    //Если обьект выключен то включаем его,меняем текст и включаем анимацию
                    else
                    {
                        AmmoIcon[0].SetActive(true);
                        Animator animatorIcon = AmmoIcon[0].GetComponent<Animator>();
                        TextMeshProUGUI textAmmo = AmmoIcon[0].GetComponentInChildren<TextMeshProUGUI>();
                        textAmmo.text = "+" + value.Value.ToString();
                        animatorIcon.SetTrigger("Add");
                    }
                       

                    break;

                //Bennelli_M4
                case BulletsTeam.BulletWeapons.Bennelli_M4:
                    //Если включен то меняем текст и запускаем анимацию
                    if (AmmoIcon[1].activeInHierarchy)
                    {

                        Animator animatorIcon = AmmoIcon[1].GetComponent<Animator>();
                        TextMeshProUGUI textAmmo = AmmoIcon[1].GetComponentInChildren<TextMeshProUGUI>();
                        textAmmo.text = "+" + value.Value.ToString();
                        animatorIcon.SetTrigger("Add");
                    }
                    //Если обьект выключен то включаем его,меняем текст и включаем анимацию
                    else
                    {
                        AmmoIcon[1].SetActive(true);
                        Animator animatorIcon = AmmoIcon[1].GetComponent<Animator>();
                        TextMeshProUGUI textAmmo = AmmoIcon[1].GetComponentInChildren<TextMeshProUGUI>();
                        textAmmo.text = "+" + value.Value.ToString();
                        animatorIcon.SetTrigger("Add");
                    }
                    break;

                //AK74
                case BulletsTeam.BulletWeapons.AK74:
                    //Если включен то меняем текст и запускаем анимацию
                    if (AmmoIcon[2].activeInHierarchy)
                    {

                        Animator animatorIcon = AmmoIcon[2].GetComponent<Animator>();
                        TextMeshProUGUI textAmmo = AmmoIcon[2].GetComponentInChildren<TextMeshProUGUI>();
                        textAmmo.text = "+" + value.Value.ToString();
                        animatorIcon.SetTrigger("Add");
                    }
                    //Если обьект выключен то включаем его,меняем текст и включаем анимацию
                    else
                    {
                        AmmoIcon[2].SetActive(true);
                        Animator animatorIcon = AmmoIcon[2].GetComponent<Animator>();
                        TextMeshProUGUI textAmmo = AmmoIcon[2].GetComponentInChildren<TextMeshProUGUI>();
                        textAmmo.text = "+" + value.Value.ToString();
                        animatorIcon.SetTrigger("Add");
                    }


                    break;
                case BulletsTeam.BulletWeapons.None:
                    Debug.Log("No wepon");
                    break;
                default:
                    Debug.Log("No wepon");
                    break;
            }
        }).AddTo(_disposable);


        __StartLevel.IconActivateWepons.Subscribe(value =>
        {
            
        }).AddTo(_disposable);


    }



   

    

    private void OnDisable()
    {

        

        _disposable.Dispose();
    }



}

