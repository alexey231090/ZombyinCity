using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using System.Collections;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    private readonly CompositeDisposable _disposables = new();

    __StartLevel startLevel;

    // Массивы для хранения данных о патронах для каждого уровня
    public int[] GunAmmo = new int[10]; // 0-9 (0 - меню, 1-9 - уровни)
    public int[] BennelliAmmo = new int[10];
    public int[] Ak74Ammo = new int[10];

    private void Awake()
    {
        Debug.Log($"PlayerDataManager.Awake() вызван. Instance = {Instance}");
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Всегда инициализируем массивы правильного размера
            GunAmmo = new int[10];
            BennelliAmmo = new int[10];
            Ak74Ammo = new int[10];

            // Подписываемся на событие загрузки новой сцены
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // Загружаем все сохраненные данные
            LoadAllSavedData();
            
            Debug.Log("PlayerDataManager инициализирован как синглтон с массивами размером 10");
        }
        else
        {
            Debug.Log("PlayerDataManager уже существует, уничтожаем дубликат");
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Обновляем подписку на __StartLevel при загрузке новой сцены
        UpdateStartLevelSubscription();
        
        // Автоматически загружаем данные для текущей сцены
        LoadDataForCurrentScene();
        
        // Обновляем состояние оружия в PlayerSwitchingStates
        UpdatePlayerWeaponState();
        
        // Запускаем отложенный поиск __StartLevel (на случай если он создается позже)
        StartCoroutine(DelayedStartLevelSearch());
    }

    private void UpdateStartLevelSubscription()
    {
        // Ищем __StartLevel в текущей сцене
         startLevel = FindObjectOfType<__StartLevel>();

        if (startLevel != null)
        {
            // Очищаем предыдущие подписки
            _disposables.Clear();

            // Подписываемся на событие OnLoadData
            startLevel.OnLoadData.Subscribe(_ => LoadDataForCurrentScene()).AddTo(_disposables);
            Debug.Log("PlayerDataManager - Подписка на событие OnLoadData выполнена.");
        }
        else
        {
            Debug.LogWarning("__StartLevel не найден в текущей сцене. Это нормально, если объект еще не создан или не нужен для этой сцены.");
        }
    }

    // Метод для сохранения данных текущей сцены (для следующего уровня)
    public void SaveDataForCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextLevelIndex = currentSceneIndex + 1; // Сохраняем для следующего уровня

        // Проверяем, что следующий уровень в пределах 1-9 (не сохраняем для меню и после 9 уровня)
        if (nextLevelIndex >= 1 && nextLevelIndex <= 9)
        {
            // Сохраняем текущие патроны игрока для следующего уровня
            int currentGunAmmo = PlayerSwitchingStates.weaponBullets.ContainsKey("Gun") ? PlayerSwitchingStates.weaponBullets["Gun"] : 0;
            int currentBennelliAmmo = PlayerSwitchingStates.weaponBullets.ContainsKey("Bennelli_M4") ? PlayerSwitchingStates.weaponBullets["Bennelli_M4"] : 0;
            int currentAk74Ammo = PlayerSwitchingStates.weaponBullets.ContainsKey("AK74") ? PlayerSwitchingStates.weaponBullets["AK74"] : 0;

            // Сохраняем в PlayerPrefs
            PlayerPrefs.SetInt($"GunAmmo_Level_{nextLevelIndex}", currentGunAmmo);
            PlayerPrefs.SetInt($"BennelliAmmo_Level_{nextLevelIndex}", currentBennelliAmmo);
            PlayerPrefs.SetInt($"Ak74Ammo_Level_{nextLevelIndex}", currentAk74Ammo);
            PlayerPrefs.Save();

            // Также обновляем массивы
            GunAmmo[nextLevelIndex] = currentGunAmmo;
            BennelliAmmo[nextLevelIndex] = currentBennelliAmmo;
            Ak74Ammo[nextLevelIndex] = currentAk74Ammo;

            Debug.Log($"Данные сохранены для следующего уровня {nextLevelIndex}: GunAmmo={currentGunAmmo}, BennelliAmmo={currentBennelliAmmo}, Ak74Ammo={currentAk74Ammo}");
        }
        else
        {
            Debug.LogWarning($"Следующий уровень {nextLevelIndex} вне диапазона 1-9. Сохранение пропущено.");
        }
    }

    // Метод для загрузки данных текущей сцены
    public void LoadDataForCurrentScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        Debug.Log($"Загрузка данных для уровня {sceneIndex}");

        // Загружаем только для уровней 1-9 (не для меню)
        if (sceneIndex >= 1 && sceneIndex <= 9)
        {
            GunAmmo[sceneIndex] = PlayerPrefs.GetInt($"GunAmmo_Level_{sceneIndex}", 0);
            BennelliAmmo[sceneIndex] = PlayerPrefs.GetInt($"BennelliAmmo_Level_{sceneIndex}", 0);
            Ak74Ammo[sceneIndex] = PlayerPrefs.GetInt($"Ak74Ammo_Level_{sceneIndex}", 0);

            Debug.Log($"Данные загружены для уровня {sceneIndex}: GunAmmo={GunAmmo[sceneIndex]}, BennelliAmmo={BennelliAmmo[sceneIndex]}, Ak74Ammo={Ak74Ammo[sceneIndex]}");
        }
        else
        {
            Debug.Log($"Сцена {sceneIndex} - это меню или не уровень. Загрузка данных пропущена.");
        }
    }

    private void OnEnable()
    {
        // Подписываемся на событие TriggerFinish
        TriggerFinish.OnSaveData.Subscribe(_ =>
        {
            // Сохраняем данные текущей сцены
            SaveDataForCurrentScene();
        }).AddTo(_disposables);
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

    // Метод для проверки состояния PlayerDataManager
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

    // Метод для загрузки всех сохраненных данных при инициализации
    private void LoadAllSavedData()
    {
        Debug.Log("Загружаем все сохраненные данные...");
        
        for (int levelIndex = 1; levelIndex <= 9; levelIndex++)
        {
            GunAmmo[levelIndex] = PlayerPrefs.GetInt($"GunAmmo_Level_{levelIndex}", 0);
            BennelliAmmo[levelIndex] = PlayerPrefs.GetInt($"BennelliAmmo_Level_{levelIndex}", 0);
            Ak74Ammo[levelIndex] = PlayerPrefs.GetInt($"Ak74Ammo_Level_{levelIndex}", 0);
            
            Debug.Log($"Уровень {levelIndex}: GunAmmo={GunAmmo[levelIndex]}, BennelliAmmo={BennelliAmmo[levelIndex]}, Ak74Ammo={Ak74Ammo[levelIndex]}");
        }
        
        Debug.Log("Все сохраненные данные загружены");
    }

    private void UpdatePlayerWeaponState()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // Обновляем данные только для уровней 1-9 (не для меню)
        if (sceneIndex >= 1 && sceneIndex <= 9)
        {
            // Загружаем данные о патронах из массива с индексом текущей сцены
            int gunAmmo = GunAmmo[sceneIndex];
            int bennelliAmmo = BennelliAmmo[sceneIndex];
            int ak74Ammo = Ak74Ammo[sceneIndex];
            
            // Обновляем состояние оружия
            PlayerSwitchingStates.weaponBullets["Gun"] = gunAmmo;
            PlayerSwitchingStates.weaponBullets["Bennelli_M4"] = bennelliAmmo;
            PlayerSwitchingStates.weaponBullets["AK74"] = ak74Ammo;

            // Отправляем обновленные данные в Subject
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
            
            Debug.Log($"PlayerDataManager - Автоматически обновлены патроны для уровня {sceneIndex}: Gun={gunAmmo}, Bennelli={bennelliAmmo}, AK74={ak74Ammo}");
        }
        else
        {
            Debug.Log($"Сцена {sceneIndex} - это меню или не уровень. Автоматическое обновление патронов пропущено.");
        }
    }

    private IEnumerator DelayedStartLevelSearch()
    {
        yield return new WaitForSeconds(1f); // Ждем 1 секунду для поиска __StartLevel

        // Повторно ищем __StartLevel
        __StartLevel startLevel = FindObjectOfType<__StartLevel>();

        if (startLevel == null)
        {
            Debug.LogWarning("__StartLevel не найден после задержки. Это нормально, если объект еще не создан или не нужен для этой сцены.");
        }
        else
        {
            // Очищаем предыдущие подписки
            _disposables.Clear();

            // Подписываемся на событие OnLoadData
            startLevel.OnLoadData.Subscribe(_ => LoadDataForCurrentScene()).AddTo(_disposables);
            Debug.Log("PlayerDataManager - Подписка на событие OnLoadData выполнена после задержки.");
        }
    }

    // Публичный метод для ручного поиска и подписки на __StartLevel
    public void TrySubscribeToStartLevel()
    {
        __StartLevel startLevel = FindObjectOfType<__StartLevel>();
        
        if (startLevel != null)
        {
            // Очищаем предыдущие подписки
            _disposables.Clear();

            // Подписываемся на событие OnLoadData
            startLevel.OnLoadData.Subscribe(_ => LoadDataForCurrentScene()).AddTo(_disposables);
            Debug.Log("PlayerDataManager - Ручная подписка на событие OnLoadData выполнена.");
        }
        else
        {
            Debug.LogWarning("__StartLevel не найден при ручном поиске.");
        }
    }
}

