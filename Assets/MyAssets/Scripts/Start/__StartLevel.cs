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
        if (PlayerDataManager.Instance != null)
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex; // Получаем индекс текущей сцены

            // Загружаем данные о патронах из массива с индексом текущей сцены
            PlayerSwitchingStates.weaponBullets["Gun"] = PlayerDataManager.Instance.GunAmmo[sceneIndex];
            PlayerSwitchingStates.weaponBullets["Bennelli_M4"] = PlayerDataManager.Instance.BennelliAmmo[sceneIndex];
            PlayerSwitchingStates.weaponBullets["AK74"] = PlayerDataManager.Instance.Ak74Ammo[sceneIndex];

            // Отправляем обновленные данные в Subject  
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);

            OnLoadData.OnNext(Unit.Default); // Вызываем событие Загрузки данных 
            Debug.Log("__StartLevel - Отправляем событие о загрузки OnLoadData");
        }
        else
        {
            Debug.Log("PlayerDataManager не найден в сцене!");
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

