using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public class TriggerFinish : MonoBehaviour
{
    public static readonly Subject<Unit> OnSaveData = new Subject<Unit>(); // Событие для сохранения данных
    

    public string levelName; // Имя уровня, на который нужно перейти
    
    private void Start()
    {
        // Подписываемся на событие сохранения данных
        OnSaveData.Subscribe(_ => SavePlayerData()).AddTo(this);
    }

    private void SavePlayerData()
    {
        // Проверяем, существует ли PlayerDataManager
        if (PlayerDataManager.Instance != null)
        {
            // Показываем текущие патроны перед сохранением
            int currentGunAmmo = PlayerSwitchingStates.weaponBullets.ContainsKey("Gun") ? PlayerSwitchingStates.weaponBullets["Gun"] : 0;
            int currentBennelliAmmo = PlayerSwitchingStates.weaponBullets.ContainsKey("Bennelli_M4") ? PlayerSwitchingStates.weaponBullets["Bennelli_M4"] : 0;
            int currentAk74Ammo = PlayerSwitchingStates.weaponBullets.ContainsKey("AK74") ? PlayerSwitchingStates.weaponBullets["AK74"] : 0;
            
            Debug.Log($"TriggerFinish - Текущие патроны: Gun={currentGunAmmo}, Bennelli={currentBennelliAmmo}, AK74={currentAk74Ammo}");
            
            // Сохраняем данные для следующего уровня
            PlayerDataManager.Instance.SaveDataForCurrentScene();
            Debug.Log("TriggerFinish - Данные игрока сохранены для следующего уровня");
        }
        else
        {
            Debug.LogWarning("TriggerFinish - PlayerDataManager не найден!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, является ли объект, с которым произошло столкновение, игроком
        if (other.CompareTag("Player"))
        {
            OnSaveData.OnNext(Unit.Default); // Вызываем событие сохранения данных
            LoadLevel(levelName);      
        }
    }

    public void LoadLevel(string nameLevel)
    {
        StartCoroutine(LoadSceneAsync(nameLevel));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
