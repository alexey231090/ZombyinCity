using UnityEngine;
using Zenject;
using UniRx;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    [Inject]
    PlayerSwitchingStates playerSwitchingStates;

    // Данные игрока
    public int GunAmmo { get; private set; }
    public int BennelliAmmo { get; private set; }
    public int Ak74Ammo { get; private set; }
    

    private void Awake()
    {
        // Реализация Singleton
        if (Instance == null)
        {
           
            Instance = this;
            DontDestroyOnLoad(gameObject); // Сохраняем объект между уровнями
            LoadData(); // Загружаем данные при старте
        }
        else
        {
            Destroy(gameObject); // Уничтожаем дублирующий объект
        }
    }

    // Метод для сохранения данных в PlayerPrefs
    public void SaveData()
    {
        PlayerPrefs.SetInt("GunAmmo", GunAmmo);
        PlayerPrefs.SetInt("BennelliAmmo", BennelliAmmo);
        PlayerPrefs.SetInt("Ak74Ammo", Ak74Ammo);
        PlayerPrefs.Save(); // Сохраняем изменения
        Debug.Log("Данные сохранены:");
    }

    // Метод для загрузки данных из PlayerPrefs
    public void LoadData()
    {
        
        GunAmmo = PlayerPrefs.GetInt("GunAmmo", 0); // Загружаем количество патронов (0 — значение по умолчанию)
        BennelliAmmo = PlayerPrefs.GetInt("BennelliAmmo", 0); // Загружаем количество патронов (0 — значение по умолчанию)
        Ak74Ammo = PlayerPrefs.GetInt("Ak74Ammo", 0); // Загружаем количество патронов (0 — значение по умолчанию)        
        Debug.Log("Данные загружены:");
    }

    // Метод для обновления данных игрока
    public void UpdateData(int gun, int bennelli,int ak74)
    {
        GunAmmo = gun;
        BennelliAmmo = bennelli;
        Ak74Ammo = ak74;

        Debug.Log("Данные обновлены: ");
    }

    // Метод для сохранения текущих данных о патронах
    private void SaveCurrentAmmoData()
    {
        // Получаем данные из PlayerSwitchingStates
        GunAmmo = PlayerSwitchingStates.weaponBullets["Gun"];
        BennelliAmmo = PlayerSwitchingStates.weaponBullets["Bennelli_M4"];
        Ak74Ammo = PlayerSwitchingStates.weaponBullets["AK74"];

        // Сохраняем данные в PlayerPrefs
        SaveData();
    }

    private void OnEnable()
    {
        // Подписываемся на событие 
        TriggerFinish.OnSaveData.Subscribe(_ => SaveCurrentAmmoData()).AddTo(this);

        __StartLevel.OnLoadData.Subscribe(_ => LoadData()).AddTo(this);
    }

    private void OnDisable()
    {
        // Отписываемся от события 
        TriggerFinish.OnSaveData.Dispose();

        __StartLevel.OnLoadData.Dispose();
    }



}

