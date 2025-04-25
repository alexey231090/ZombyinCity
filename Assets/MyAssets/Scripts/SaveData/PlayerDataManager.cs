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

    // Массивы для хранения данных о патронах для каждого уровня
    public int[] GunAmmo = new int[9];
    public int[] BennelliAmmo = new int[9];
    public int[] Ak74Ammo = new int[9];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Подписываемся на событие загрузки новой сцены
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Обновляем подписку на __StartLevel при загрузке новой сцены
        UpdateStartLevelSubscription();
    }

    private void UpdateStartLevelSubscription()
    {
        

        if (__StartLevel != null)
        {
            // Очищаем предыдущие подписки
            _disposables.Clear();

            // Подписываемся на событие OnLoadData
            __StartLevel.OnLoadData.Subscribe(_ => LoadDataForCurrentScene()).AddTo(_disposables);
            Debug.Log("PlayerDataManager - Подписка на событие OnLoadData выполнена.");
        }
        else
        {
            Debug.LogError("Внимание!: __StartLevel не найден в текущей сцене!");
        }



        


    }

    // Метод для сохранения данных текущей сцены (для следующего уровня)
    public void SaveDataForCurrentScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1; // Сохраняем для следующего уровня

        if (sceneIndex >= 0 && sceneIndex < 9) // Проверяем, что индекс в пределах массива
        {
            PlayerPrefs.SetInt($"GunAmmo_Level_{sceneIndex}", GunAmmo[sceneIndex]);
            PlayerPrefs.SetInt($"BennelliAmmo_Level_{sceneIndex}", BennelliAmmo[sceneIndex]);
            PlayerPrefs.SetInt($"Ak74Ammo_Level_{sceneIndex}", Ak74Ammo[sceneIndex]);
            PlayerPrefs.Save();

            Debug.Log($"Данные сохранены для следующего уровня {sceneIndex}: GunAmmo={GunAmmo[sceneIndex]}, BennelliAmmo={BennelliAmmo[sceneIndex]}, Ak74Ammo={Ak74Ammo[sceneIndex]}");
        }
        else
        {
            Debug.LogWarning($"Индекс следующей сцены {sceneIndex} выходит за пределы массива.");
        }
    }

    // Метод для загрузки данных текущей сцены
    public void LoadDataForCurrentScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        Debug.Log($"Загрузка данных до if для уровня {sceneIndex}");

        if (sceneIndex >= 0 && sceneIndex < 9)
        {
            GunAmmo[sceneIndex] = PlayerPrefs.GetInt($"GunAmmo_Level_{sceneIndex}", 0);
            BennelliAmmo[sceneIndex] = PlayerPrefs.GetInt($"BennelliAmmo_Level_{sceneIndex}", 0);
            Ak74Ammo[sceneIndex] = PlayerPrefs.GetInt($"Ak74Ammo_Level_{sceneIndex}", 0);

            Debug.Log($"Данные загружены для уровня {sceneIndex}: GunAmmo={GunAmmo[sceneIndex]}, BennelliAmmo={BennelliAmmo[sceneIndex]}, Ak74Ammo={Ak74Ammo[sceneIndex]}");
        }
        else
        {
            Debug.LogWarning($"Индекс сцены {sceneIndex} выходит за пределы массива.");
        }
    }

    private void OnEnable()
    {

        //// Подписываемся на событие TriggerFinish
        //TriggerFinish.OnSaveData.Subscribe(_ =>
        //{
        //    // Сохраняем данные текущей сцены
        //    SaveDataForCurrentScene();
        //}).AddTo(_disposables);
    }

    private void OnDisable()
    {


        _disposables.Dispose();
    }

    private void OnDestroy()
    {
        // Отписываемся от события загрузки сцены
        SceneManager.sceneLoaded -= OnSceneLoaded;

        
    }
}

