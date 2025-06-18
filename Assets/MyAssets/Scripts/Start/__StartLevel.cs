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
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogWarning("PlayerDataManager �� ������! ������� ����� ���������...");
            CreatePlayerDataManager();
        }

        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.IsInitialized())
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex; // �������� ������ ������� �����

            // ��������� ������ ������ ��� ������� 1-9 (�� ��� ����)
            if (sceneIndex >= 1 && sceneIndex <= 9)
            {
                // ��������� ������ � �������� �� ������� � �������� ������� �����
                int gunAmmo = PlayerDataManager.Instance.GunAmmo[sceneIndex];
                int bennelliAmmo = PlayerDataManager.Instance.BennelliAmmo[sceneIndex];
                int ak74Ammo = PlayerDataManager.Instance.Ak74Ammo[sceneIndex];
                
                PlayerSwitchingStates.weaponBullets["Gun"] = gunAmmo;
                PlayerSwitchingStates.weaponBullets["Bennelli_M4"] = bennelliAmmo;
                PlayerSwitchingStates.weaponBullets["AK74"] = ak74Ammo;

                // ���������� ����������� ������ � Subject  
                PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);

                OnLoadData.OnNext(Unit.Default); // �������� ������� �������� ������ 
                Debug.Log($"__StartLevel - ��������� ������� ��� ������ {sceneIndex}: Gun={gunAmmo}, Bennelli={bennelliAmmo}, AK74={ak74Ammo}");
            }
            else
            {
                Debug.Log($"����� {sceneIndex} - ��� ���� ��� �� �������. �������� �������� ���������.");
            }
        }
        else
        {
            Debug.LogWarning("PlayerDataManager �� ������ ��� �� ��������������� � �����!");
            // ������������� �������� �� ���������
            PlayerSwitchingStates.weaponBullets["Gun"] = 0;
            PlayerSwitchingStates.weaponBullets["Bennelli_M4"] = 0;
            PlayerSwitchingStates.weaponBullets["AK74"] = 0;
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
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

    private void CreatePlayerDataManager()
    {
        // ������� ����� GameObject � ����������� PlayerDataManager
        GameObject playerDataManagerGO = new GameObject("PlayerDataManager");
        PlayerDataManager playerDataManager = playerDataManagerGO.AddComponent<PlayerDataManager>();
        
        // ������������� �������������� �������
        playerDataManager.GunAmmo = new int[10];
        playerDataManager.BennelliAmmo = new int[10];
        playerDataManager.Ak74Ammo = new int[10];
        
        Debug.Log("PlayerDataManager ������ ���������� � ������������������� ���������");
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

