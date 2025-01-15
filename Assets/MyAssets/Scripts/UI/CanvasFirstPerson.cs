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
    

    static public int canvasAmmoInt = 0;

    CompositeDisposable _disposable = new();

   public GameObject FaceBlood;
   Image faceBloodImage; 




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
        ak47Observable.Merge(gunObservable,bennObservable,crowObservable)
            .Subscribe(value =>
            {
                canvasAmmoText.text = value;             
            })
            .AddTo(_disposable); 


        //�����
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


        //����� �� ����
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





        //������������� ���������� ����� �� 0 ��� ������ Player
        PlayerSwitchingStates.isDead.Subscribe(value =>
        {

           
            print("isDead");

        }).AddTo(_disposable);




    }



   

    

    private void OnDisable()
    {

        

        _disposable.Dispose();
    }



}

