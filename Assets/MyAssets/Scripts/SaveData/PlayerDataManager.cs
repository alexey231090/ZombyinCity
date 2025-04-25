using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.SceneManagement;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    private readonly CompositeDisposable _disposables = new();

    [Inject]
     __StartLevel __StartLevel;

    // ������� ��� �������� ������ � �������� ��� ������� ������
    public int[] GunAmmo = new int[9];
    public int[] BennelliAmmo = new int[9];
    public int[] Ak74Ammo = new int[9];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ������������� �� ������� �������� ����� �����
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ��������� �������� �� __StartLevel ��� �������� ����� �����
        UpdateStartLevelSubscription();
    }

    private void UpdateStartLevelSubscription()
    {
        

        if (__StartLevel != null)
        {
            // ������� ���������� ��������
            _disposables.Clear();

            // ������������� �� ������� OnLoadData
            __StartLevel.OnLoadData.Subscribe(_ => LoadDataForCurrentScene()).AddTo(_disposables);
            Debug.Log("PlayerDataManager - �������� �� ������� OnLoadData ���������.");
        }
        else
        {
            Debug.LogError("��������!: __StartLevel �� ������ � ������� �����!");
        }



        


    }

    // ����� ��� ���������� ������ ������� ����� (��� ���������� ������)
    public void SaveDataForCurrentScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1; // ��������� ��� ���������� ������

        if (sceneIndex >= 0 && sceneIndex < 9) // ���������, ��� ������ � �������� �������
        {
            PlayerPrefs.SetInt($"GunAmmo_Level_{sceneIndex}", GunAmmo[sceneIndex]);
            PlayerPrefs.SetInt($"BennelliAmmo_Level_{sceneIndex}", BennelliAmmo[sceneIndex]);
            PlayerPrefs.SetInt($"Ak74Ammo_Level_{sceneIndex}", Ak74Ammo[sceneIndex]);
            PlayerPrefs.Save();

            Debug.Log($"������ ��������� ��� ���������� ������ {sceneIndex}: GunAmmo={GunAmmo[sceneIndex]}, BennelliAmmo={BennelliAmmo[sceneIndex]}, Ak74Ammo={Ak74Ammo[sceneIndex]}");
        }
        else
        {
            Debug.LogWarning($"������ ��������� ����� {sceneIndex} ������� �� ������� �������.");
        }
    }

    // ����� ��� �������� ������ ������� �����
    public void LoadDataForCurrentScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        Debug.Log($"�������� ������ �� if ��� ������ {sceneIndex}");

        if (sceneIndex >= 0 && sceneIndex < 9)
        {
            GunAmmo[sceneIndex] = PlayerPrefs.GetInt($"GunAmmo_Level_{sceneIndex}", 0);
            BennelliAmmo[sceneIndex] = PlayerPrefs.GetInt($"BennelliAmmo_Level_{sceneIndex}", 0);
            Ak74Ammo[sceneIndex] = PlayerPrefs.GetInt($"Ak74Ammo_Level_{sceneIndex}", 0);

            Debug.Log($"������ ��������� ��� ������ {sceneIndex}: GunAmmo={GunAmmo[sceneIndex]}, BennelliAmmo={BennelliAmmo[sceneIndex]}, Ak74Ammo={Ak74Ammo[sceneIndex]}");
        }
        else
        {
            Debug.LogWarning($"������ ����� {sceneIndex} ������� �� ������� �������.");
        }
    }

    private void OnEnable()
    {

        //// ������������� �� ������� TriggerFinish
        //TriggerFinish.OnSaveData.Subscribe(_ =>
        //{
        //    // ��������� ������ ������� �����
        //    SaveDataForCurrentScene();
        //}).AddTo(_disposables);
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
}

