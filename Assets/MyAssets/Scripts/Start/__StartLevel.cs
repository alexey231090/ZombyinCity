using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class __StartLevel : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    public readonly Subject<Unit> OnLoadData = new Subject<Unit>(); // Вызываем событие Загрузки данных
    public readonly Subject<int> IconActivateWepons = new Subject<int>();

    [SerializeField] bool crowbarActiv, gunActiv, benelliActiv, ak74Activ; // Переменные для проверки активации предметов
    [SerializeField] int crowbarIndex, gunIndex, benelliIndex, ak74Index; // Индексы для иконок оружия

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, существует ли PlayerDataManager в сцене  
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogWarning("PlayerDataManager не найден! Создаем новый экземпляр...");
            CreatePlayerDataManager();
        }

        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.IsInitialized())
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex; // Получаем индекс текущей сцены

            // Загружаем данные только для уровней 1-9 (не для меню)
            if (sceneIndex >= 1 && sceneIndex <= 9)
            {
                // Загружаем данные о патронах из массива с индексом текущей сцены
                int gunAmmo = PlayerDataManager.Instance.GunAmmo[sceneIndex];
                int bennelliAmmo = PlayerDataManager.Instance.BennelliAmmo[sceneIndex];
                int ak74Ammo = PlayerDataManager.Instance.Ak74Ammo[sceneIndex];
                
                PlayerSwitchingStates.weaponBullets["Gun"] = gunAmmo;
                PlayerSwitchingStates.weaponBullets["Bennelli_M4"] = bennelliAmmo;
                PlayerSwitchingStates.weaponBullets["AK74"] = ak74Ammo;

                // Отправляем обновленные данные в Subject  
                PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);

                OnLoadData.OnNext(Unit.Default); // Вызываем событие Загрузки данных 
                Debug.Log($"__StartLevel - Загружены патроны для уровня {sceneIndex}: Gun={gunAmmo}, Bennelli={bennelliAmmo}, AK74={ak74Ammo}");
            }
            else
            {
                Debug.Log($"Сцена {sceneIndex} - это меню или не уровень. Загрузка патронов пропущена.");
            }
        }
        else
        {
            Debug.LogWarning("PlayerDataManager не найден или не инициализирован в сцене!");
            // Устанавливаем значения по умолчанию
            PlayerSwitchingStates.weaponBullets["Gun"] = 0;
            PlayerSwitchingStates.weaponBullets["Bennelli_M4"] = 0;
            PlayerSwitchingStates.weaponBullets["AK74"] = 0;
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
        }

        // Устанавливаем состояние для предметов
        PlayerSwitchingStates.CrowbarIsActive = crowbarActiv;
        PlayerSwitchingStates.GunIsActive = gunActiv;
        PlayerSwitchingStates.BennelliIsActive = benelliActiv;
        PlayerSwitchingStates.AK74IsActive = ak74Activ;

        // Активируем иконки оружия
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
        // Создаем новый GameObject с компонентом PlayerDataManager
        GameObject playerDataManagerGO = new GameObject("PlayerDataManager");
        PlayerDataManager playerDataManager = playerDataManagerGO.AddComponent<PlayerDataManager>();
        
        // Принудительно инициализируем массивы
        playerDataManager.GunAmmo = new int[10];
        playerDataManager.BennelliAmmo = new int[10];
        playerDataManager.Ak74Ammo = new int[10];
        
        Debug.Log("PlayerDataManager создан программно с инициализированными массивами");
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

