using UnityEngine;
using TMPro;
using UniRx;
using System;
using UnityEngine.UI;
using Zenject;



public class CanvasFirstPerson : MonoBehaviour
{
    [Inject]
    __StartLevel __StartLevel;

    public TextMeshProUGUI canvasAmmoText;
    public TextMeshProUGUI PlayerLifeText;
    public TextMeshProUGUI canvasPlayerShopWepon;

    public GameObject FaceBlood;

    public GameObject[] AmmoIcon; // ������ ������ ��������

    Animator animatorIcon;
    TextMeshProUGUI textAmmo;

    static public int canvasAmmoInt = 0;

    Image faceBloodImage;

    CompositeDisposable _disposable = new();

    private void Start()
    {
        // �������� ��������� Image �� ��������� ������� FaceBlood
        faceBloodImage = FaceBlood.GetComponentInChildren<Image>();
        // ������������� ��������� ������������ � 0
        SetImageAlpha(0);

    }


    private void SetImageAlpha(float alpha)
    {
        Color color = faceBloodImage.color; // �������� ������� ����
        color.a = alpha; // ������������� ����� alpha
        faceBloodImage.color = color; // ����������� ���� �������
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

        // ���������� ��� Observable ������� � ��������
        ak47Observable.Merge(gunObservable, bennObservable, crowObservable)
            .Subscribe(value =>
            {
                canvasAmmoText.text = value;
            })
            .AddTo(_disposable);

        // �����
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

        // ����� �� ����
        PlayerSwitchingStates.lifeIntSubject
            .Skip(1)
            .Subscribe(value =>
            {
                // ������������� ������� ������������
                SetImageAlpha(180f / 255f);

                // ������� ���������
                Observable.Range(0, 100) // 100 ������ ��� ���������
                    .Select(i => (float)i / 100f) // ����������� �������� �� 0 �� 1
                    .Delay(TimeSpan.FromSeconds(0.01f)) // �������� ����� �������
                    .Subscribe(t =>
                    {
                        float newAlpha = Mathf.Lerp(180f / 255f, 0f, t); // �������� ������������
                        SetImageAlpha(newAlpha);
                    })
                    .AddTo(_disposable);
            })
            .AddTo(_disposable);

        PlayerSwitchingStates.allBulletsDictSubject.Subscribe(value =>
        {
            // ����� ���������� �������� �� ������
            string weaponKey = PlayerSwitchingStates.weapon.ToString();

            // ���������, ���� �� ���� � �������
            if (value.TryGetValue(weaponKey, out int bulletCount))
            {
                canvasPlayerShopWepon.text = "/" + bulletCount.ToString();
            }
            else
            {
                // ��������� ������, ���� ���� �� ������
                canvasPlayerShopWepon.text = "/0";
            }
        }).AddTo(_disposable);

        // ������������� ���������� ����� �� 0 ��� ������ Player
        PlayerSwitchingStates.isDead.Subscribe(value =>
        {
            print("isDead");
        }).AddTo(_disposable);







        // �������� �� ������� � __EffectAddBullets Value �������� enum � int �� BulletsTeam
        __EffectIconAddBullets.bulletsSubject.Subscribe(value =>
        {
            switch (value.Key)
            {
                //Gun
                case BulletsTeam.BulletWeapons.Gun:

                    //���� ������� �� ������ ����� � ��������� ��������
                    if (AmmoIcon[0].activeInHierarchy)
                    {
                        
                        Animator animatorIcon = AmmoIcon[0].GetComponent<Animator>();
                        TextMeshProUGUI textAmmo = AmmoIcon[0].GetComponentInChildren<TextMeshProUGUI>();
                        textAmmo.text = "+" + value.Value.ToString();
                        animatorIcon.SetTrigger("Add");
                    }
                    //���� ������ �������� �� �������� ���,������ ����� � �������� ��������
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
                    //���� ������� �� ������ ����� � ��������� ��������
                    if (AmmoIcon[1].activeInHierarchy)
                    {

                        Animator animatorIcon = AmmoIcon[1].GetComponent<Animator>();
                        TextMeshProUGUI textAmmo = AmmoIcon[1].GetComponentInChildren<TextMeshProUGUI>();
                        textAmmo.text = "+" + value.Value.ToString();
                        animatorIcon.SetTrigger("Add");
                    }
                    //���� ������ �������� �� �������� ���,������ ����� � �������� ��������
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
                    //���� ������� �� ������ ����� � ��������� ��������
                    if (AmmoIcon[2].activeInHierarchy)
                    {

                        Animator animatorIcon = AmmoIcon[2].GetComponent<Animator>();
                        TextMeshProUGUI textAmmo = AmmoIcon[2].GetComponentInChildren<TextMeshProUGUI>();
                        textAmmo.text = "+" + value.Value.ToString();
                        animatorIcon.SetTrigger("Add");
                    }
                    //���� ������ �������� �� �������� ���,������ ����� � �������� ��������
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

