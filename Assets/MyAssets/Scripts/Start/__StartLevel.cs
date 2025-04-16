using UniRx;
using UnityEngine;
using Zenject;

public class __StartLevel : MonoBehaviour
{

    public static readonly Subject<Unit> OnLoadData = new Subject<Unit>(); // �������� ������� �������� ������
    public static readonly Subject<int> IconActivateWepons = new Subject<int>();

    [SerializeField] bool crowbarActiv, gunActiv, benelliActiv, ak74Activ; // ���������� ��� �������� ��������� ���������
    [SerializeField] int crowbarIndex, gunIndex, benelliIndex, ak74Index; // ������� ��� ������ ������

    [Inject]
    PlayerSwitchingStates playerSwitchingStates; // ���������� ��� ������� � ������ PlayerSwitchingStates


    private void OnTriggerEnter(Collider other)
    {
        

        // ���������, ���������� �� PlayerDataManager � �����  
        if (PlayerDataManager.Instance != null)
        {
            OnLoadData.OnNext(Unit.Default); // �������� ������� �������� ������ 

            // ��������� ������� weaponBullets  
            PlayerSwitchingStates.weaponBullets["Gun"] = PlayerDataManager.Instance.GunAmmo;
            PlayerSwitchingStates.weaponBullets["Bennelli_M4"] = PlayerDataManager.Instance.BennelliAmmo;
            PlayerSwitchingStates.weaponBullets["AK74"] = PlayerDataManager.Instance.Ak74Ammo;

            // ���������� ����������� ������ � Subject  
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
        }
        else
        {
            Debug.Log("PlayerDataManager �� ������ � �����!");
            
        }


        PlayerSwitchingStates.CrowbarIsActive = crowbarActiv; // ������������� ��������� ��� �������� "�������"  
        PlayerSwitchingStates.GunIsActive = gunActiv; // ������������� ��������� ��� �������� "��������"  
        PlayerSwitchingStates.BennelliIsActive = benelliActiv; // ������������� ��������� ��� �������� "�������"  
        PlayerSwitchingStates.AK74IsActive = ak74Activ; // ������������� ��������� ��� �������� "��74"  

        // ���������� ������ ������  
        if (crowbarActiv)
        {
            Observable.Timer(System.TimeSpan.FromSeconds(0.05))
                .Subscribe(_ =>
                {
                    IconActivateWepons.OnNext(crowbarIndex);
                })
                .AddTo(this);
        }

        if (gunActiv)
        {
            Observable.Timer(System.TimeSpan.FromSeconds(0.1))
                .Subscribe(_ =>
                {
                    IconActivateWepons.OnNext(gunIndex);

                    print("gunIndex = !!!!!!!!!!!!!!!!!!!!!!!!!!!" + gunIndex);
                })
                .AddTo(this);
        }

        if (benelliActiv)
        {
            Observable.Timer(System.TimeSpan.FromSeconds(0.15))
                .Subscribe(_ =>
                {
                    IconActivateWepons.OnNext(benelliIndex);
                })
                .AddTo(this);
        }

        if (ak74Activ)
        {
            Observable.Timer(System.TimeSpan.FromSeconds(0.2))
                .Subscribe(_ =>
                {
                    IconActivateWepons.OnNext(ak74Index);
                })
                .AddTo(this);
        }

        // ������� ������ ����� 2 �������  
        Observable.Timer(System.TimeSpan.FromSeconds(2))
            .Subscribe(_ =>
            {
                Destroy(this.gameObject);
            })
            .AddTo(this);
    }
}

