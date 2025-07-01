using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using System.Collections;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    private readonly CompositeDisposable _disposables = new();

    __StartLevel startLevel;

    // ������� ��� �������� ������ � �������� ��� ������� ������
    public int[] GunAmmo = new int[10]; // 0-9 (0 - ����, 1-9 - ������)
    public int[] BennelliAmmo = new int[10];
    public int[] Ak74Ammo = new int[10];

    private void Awake()
    {
        Debug.Log($"PlayerDataManager.Awake() ������. Instance = {Instance}");
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ������ �������������� ������� ����������� �������
            GunAmmo = new int[10];
            BennelliAmmo = new int[10];
            Ak74Ammo = new int[10];

            // ������������� �� ������� �������� ����� �����
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // ��������� ��� ����������� ������
            LoadAllSavedData();
            
            Debug.Log("PlayerDataManager ��������������� ��� �������� � ��������� �������� 10");
        }
        else
        {
            Debug.Log("PlayerDataManager ��� ����������, ���������� ��������");
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ��������� �������� �� __StartLevel ��� �������� ����� �����
        UpdateStartLevelSubscription();
        
        // ������������� ��������� ������ ��� ������� �����
        LoadDataForCurrentScene();
        
        // ��������� ��������� ������ � PlayerSwitchingStates
        UpdatePlayerWeaponState();
        
        // ��������� ���������� ����� __StartLevel (�� ������ ���� �� ��������� �����)
        StartCoroutine(DelayedStartLevelSearch());
    }

    private void UpdateStartLevelSubscription()
    {
        // ���� __StartLevel � ������� �����
         startLevel = FindObjectOfType<__StartLevel>();

        if (startLevel != null)
        {
            // ������� ���������� ��������
            _disposables.Clear();

            // ������������� �� ������� OnLoadData
            startLevel.OnLoadData.Subscribe(_ => LoadDataForCurrentScene()).AddTo(_disposables);
            Debug.Log("PlayerDataManager - �������� �� ������� OnLoadData ���������.");
        }
        else
        {
            Debug.LogWarning("__StartLevel �� ������ � ������� �����. ��� ���������, ���� ������ ��� �� ������ ��� �� ����� ��� ���� �����.");
        }
    }

    // ����� ��� ���������� ������ ������� ����� (��� ���������� ������)
    public void SaveDataForCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextLevelIndex = currentSceneIndex + 1; // ��������� ��� ���������� ������

        // ���������, ��� ��������� ������� � �������� 1-9 (�� ��������� ��� ���� � ����� 9 ������)
        if (nextLevelIndex >= 1 && nextLevelIndex <= 9)
        {
            // ��������� ������� ������� ������ ��� ���������� ������
            int currentGunAmmo = PlayerSwitchingStates.weaponBullets.ContainsKey("Gun") ? PlayerSwitchingStates.weaponBullets["Gun"] : 0;
            int currentBennelliAmmo = PlayerSwitchingStates.weaponBullets.ContainsKey("Bennelli_M4") ? PlayerSwitchingStates.weaponBullets["Bennelli_M4"] : 0;
            int currentAk74Ammo = PlayerSwitchingStates.weaponBullets.ContainsKey("AK74") ? PlayerSwitchingStates.weaponBullets["AK74"] : 0;

            // ��������� � PlayerPrefs
            PlayerPrefs.SetInt($"GunAmmo_Level_{nextLevelIndex}", currentGunAmmo);
            PlayerPrefs.SetInt($"BennelliAmmo_Level_{nextLevelIndex}", currentBennelliAmmo);
            PlayerPrefs.SetInt($"Ak74Ammo_Level_{nextLevelIndex}", currentAk74Ammo);
            PlayerPrefs.Save();

            // ����� ��������� �������
            GunAmmo[nextLevelIndex] = currentGunAmmo;
            BennelliAmmo[nextLevelIndex] = currentBennelliAmmo;
            Ak74Ammo[nextLevelIndex] = currentAk74Ammo;

            Debug.Log($"������ ��������� ��� ���������� ������ {nextLevelIndex}: GunAmmo={currentGunAmmo}, BennelliAmmo={currentBennelliAmmo}, Ak74Ammo={currentAk74Ammo}");
        }
        else
        {
            Debug.LogWarning($"��������� ������� {nextLevelIndex} ��� ��������� 1-9. ���������� ���������.");
        }
    }

    // ����� ��� �������� ������ ������� �����
    public void LoadDataForCurrentScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        Debug.Log($"�������� ������ ��� ������ {sceneIndex}");

        // ��������� ������ ��� ������� 1-9 (�� ��� ����)
        if (sceneIndex >= 1 && sceneIndex <= 9)
        {
            GunAmmo[sceneIndex] = PlayerPrefs.GetInt($"GunAmmo_Level_{sceneIndex}", 0);
            BennelliAmmo[sceneIndex] = PlayerPrefs.GetInt($"BennelliAmmo_Level_{sceneIndex}", 0);
            Ak74Ammo[sceneIndex] = PlayerPrefs.GetInt($"Ak74Ammo_Level_{sceneIndex}", 0);

            Debug.Log($"������ ��������� ��� ������ {sceneIndex}: GunAmmo={GunAmmo[sceneIndex]}, BennelliAmmo={BennelliAmmo[sceneIndex]}, Ak74Ammo={Ak74Ammo[sceneIndex]}");
        }
        else
        {
            Debug.Log($"����� {sceneIndex} - ��� ���� ��� �� �������. �������� ������ ���������.");
        }
    }

    private void OnEnable()
    {
        // ������������� �� ������� TriggerFinish
        TriggerFinish.OnSaveData.Subscribe(_ =>
        {
            // ��������� ������ ������� �����
            SaveDataForCurrentScene();
        }).AddTo(_disposables);
    }

    private void OnDisable()
    {


        _disposables.Dispose();
    }

    private void OnDestroy()
    {
        // ������������ �� ������� �������� �����
        SceneManager.sceneLoaded -= OnSceneLoaded;

        
    }

    // ����� ��� �������� ��������� PlayerDataManager
    public bool IsInitialized()
    {
        bool isInitialized = Instance != null && 
               GunAmmo != null && 
               BennelliAmmo != null && 
               Ak74Ammo != null &&
               GunAmmo.Length >= 10;
               
        Debug.Log($"PlayerDataManager.IsInitialized(): Instance={Instance != null}, GunAmmo={GunAmmo != null}({GunAmmo?.Length}), BennelliAmmo={BennelliAmmo != null}({BennelliAmmo?.Length}), Ak74Ammo={Ak74Ammo != null}({Ak74Ammo?.Length}), Result={isInitialized}");
        
        return isInitialized;
    }

    // ����� ��� �������� ���� ����������� ������ ��� �������������
    private void LoadAllSavedData()
    {
        Debug.Log("��������� ��� ����������� ������...");
        
        for (int levelIndex = 1; levelIndex <= 9; levelIndex++)
        {
            GunAmmo[levelIndex] = PlayerPrefs.GetInt($"GunAmmo_Level_{levelIndex}", 0);
            BennelliAmmo[levelIndex] = PlayerPrefs.GetInt($"BennelliAmmo_Level_{levelIndex}", 0);
            Ak74Ammo[levelIndex] = PlayerPrefs.GetInt($"Ak74Ammo_Level_{levelIndex}", 0);
            
            Debug.Log($"������� {levelIndex}: GunAmmo={GunAmmo[levelIndex]}, BennelliAmmo={BennelliAmmo[levelIndex]}, Ak74Ammo={Ak74Ammo[levelIndex]}");
        }
        
        Debug.Log("��� ����������� ������ ���������");
    }

    private void UpdatePlayerWeaponState()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // ��������� ������ ������ ��� ������� 1-9 (�� ��� ����)
        if (sceneIndex >= 1 && sceneIndex <= 9)
        {
            // ��������� ������ � �������� �� ������� � �������� ������� �����
            int gunAmmo = GunAmmo[sceneIndex];
            int bennelliAmmo = BennelliAmmo[sceneIndex];
            int ak74Ammo = Ak74Ammo[sceneIndex];
            
            // ��������� ��������� ������
            PlayerSwitchingStates.weaponBullets["Gun"] = gunAmmo;
            PlayerSwitchingStates.weaponBullets["Bennelli_M4"] = bennelliAmmo;
            PlayerSwitchingStates.weaponBullets["AK74"] = ak74Ammo;

            // ���������� ����������� ������ � Subject
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
            
            Debug.Log($"PlayerDataManager - ������������� ��������� ������� ��� ������ {sceneIndex}: Gun={gunAmmo}, Bennelli={bennelliAmmo}, AK74={ak74Ammo}");
        }
        else
        {
            Debug.Log($"����� {sceneIndex} - ��� ���� ��� �� �������. �������������� ���������� �������� ���������.");
        }
    }

    private IEnumerator DelayedStartLevelSearch()
    {
        yield return new WaitForSeconds(1f); // ���� 1 ������� ��� ������ __StartLevel

        // �������� ���� __StartLevel
        __StartLevel startLevel = FindObjectOfType<__StartLevel>();

        if (startLevel == null)
        {
            Debug.LogWarning("__StartLevel �� ������ ����� ��������. ��� ���������, ���� ������ ��� �� ������ ��� �� ����� ��� ���� �����.");
        }
        else
        {
            // ������� ���������� ��������
            _disposables.Clear();

            // ������������� �� ������� OnLoadData
            startLevel.OnLoadData.Subscribe(_ => LoadDataForCurrentScene()).AddTo(_disposables);
            Debug.Log("PlayerDataManager - �������� �� ������� OnLoadData ��������� ����� ��������.");
        }
    }

    // ��������� ����� ��� ������� ������ � �������� �� __StartLevel
    public void TrySubscribeToStartLevel()
    {
        __StartLevel startLevel = FindObjectOfType<__StartLevel>();
        
        if (startLevel != null)
        {
            // ������� ���������� ��������
            _disposables.Clear();

            // ������������� �� ������� OnLoadData
            startLevel.OnLoadData.Subscribe(_ => LoadDataForCurrentScene()).AddTo(_disposables);
            Debug.Log("PlayerDataManager - ������ �������� �� ������� OnLoadData ���������.");
        }
        else
        {
            Debug.LogWarning("__StartLevel �� ������ ��� ������ ������.");
        }
    }
}

