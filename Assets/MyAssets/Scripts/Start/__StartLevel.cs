using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class __StartLevel : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    public readonly Subject<Unit> OnLoadData = new Subject<Unit>(); // �������� ������� �������� ������
    public readonly Subject<int> IconActivateWepons = new Subject<int>();

    [SerializeField] bool crowbarActiv, gunActiv, benelliActiv, ak74Activ; // ���������� ��� �������� ��������� ���������
    [SerializeField] int crowbarIndex, gunIndex, benelliIndex, ak74Index; // ������� ��� ������ ������

    private void OnTriggerEnter(Collider other)
    {
        // ���������, ���������� �� PlayerDataManager � �����  
        if (PlayerDataManager.Instance != null)
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex; // �������� ������ ������� �����

            // ��������� ������ � �������� �� ������� � �������� ������� �����
            PlayerSwitchingStates.weaponBullets["Gun"] = PlayerDataManager.Instance.GunAmmo[sceneIndex];
            PlayerSwitchingStates.weaponBullets["Bennelli_M4"] = PlayerDataManager.Instance.BennelliAmmo[sceneIndex];
            PlayerSwitchingStates.weaponBullets["AK74"] = PlayerDataManager.Instance.Ak74Ammo[sceneIndex];

            // ���������� ����������� ������ � Subject  
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);

            OnLoadData.OnNext(Unit.Default); // �������� ������� �������� ������ 
            Debug.Log("__StartLevel - ���������� ������� � �������� OnLoadData");
        }
        else
        {
            Debug.Log("PlayerDataManager �� ������ � �����!");
        }

        // ������������� ��������� ��� ���������
        PlayerSwitchingStates.CrowbarIsActive = crowbarActiv;
        PlayerSwitchingStates.GunIsActive = gunActiv;
        PlayerSwitchingStates.BennelliIsActive = benelliActiv;
        PlayerSwitchingStates.AK74IsActive = ak74Activ;

        // ���������� ������ ������
        if (crowbarActiv)
        {
           
                    IconActivateWepons.OnNext(crowbarIndex);
                
        }

        if (gunActiv)
        {
            
                    IconActivateWepons.OnNext(gunIndex);
                    
                
        }

        if (benelliActiv)
        {
            
                    IconActivateWepons.OnNext(benelliIndex);
                
        }

        if (ak74Activ)
        {
            
                    IconActivateWepons.OnNext(ak74Index);
                
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _disposables.Dispose();
        Destroy(this.gameObject);
    }

    private void OnDisable()
    {
        _disposables.Dispose();
        IconActivateWepons.OnCompleted();
    }
}

