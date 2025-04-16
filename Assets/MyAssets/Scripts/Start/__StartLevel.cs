using UniRx;
using UnityEngine;
using Zenject;

public class __StartLevel : MonoBehaviour
{

    public static readonly Subject<Unit> OnLoadData = new Subject<Unit>(); // Вызываем событие Загрузки данных
    public static readonly Subject<int> IconActivateWepons = new Subject<int>();

    [SerializeField] bool crowbarActiv, gunActiv, benelliActiv, ak74Activ; // Переменные для проверки активации предметов
    [SerializeField] int crowbarIndex, gunIndex, benelliIndex, ak74Index; // Индексы для иконок оружия

    [Inject]
    PlayerSwitchingStates playerSwitchingStates; // Переменная для доступа к классу PlayerSwitchingStates


    private void OnTriggerEnter(Collider other)
    {
        

        // Проверяем, существует ли PlayerDataManager в сцене  
        if (PlayerDataManager.Instance != null)
        {
            OnLoadData.OnNext(Unit.Default); // Вызываем событие Загрузки данных 

            // Обновляем словарь weaponBullets  
            PlayerSwitchingStates.weaponBullets["Gun"] = PlayerDataManager.Instance.GunAmmo;
            PlayerSwitchingStates.weaponBullets["Bennelli_M4"] = PlayerDataManager.Instance.BennelliAmmo;
            PlayerSwitchingStates.weaponBullets["AK74"] = PlayerDataManager.Instance.Ak74Ammo;

            // Отправляем обновленные данные в Subject  
            PlayerSwitchingStates.allBulletsDictSubject.OnNext(PlayerSwitchingStates.weaponBullets);
        }
        else
        {
            Debug.Log("PlayerDataManager не найден в сцене!");
            
        }


        PlayerSwitchingStates.CrowbarIsActive = crowbarActiv; // Устанавливаем состояние для предмета "Кувалда"  
        PlayerSwitchingStates.GunIsActive = gunActiv; // Устанавливаем состояние для предмета "Пистолет"  
        PlayerSwitchingStates.BennelliIsActive = benelliActiv; // Устанавливаем состояние для предмета "Бенелли"  
        PlayerSwitchingStates.AK74IsActive = ak74Activ; // Устанавливаем состояние для предмета "АК74"  

        // Активируем иконки оружия  
        if (crowbarActiv)
        {
            Observable.Timer(System.TimeSpan.FromSeconds(0.05))
                .Subscribe(_ =>
                {
                    IconActivateWepons.OnNext(crowbarIndex);
                })
                .AddTo(this);
        }

        if (gunActiv)
        {
            Observable.Timer(System.TimeSpan.FromSeconds(0.1))
                .Subscribe(_ =>
                {
                    IconActivateWepons.OnNext(gunIndex);

                    print("gunIndex = !!!!!!!!!!!!!!!!!!!!!!!!!!!" + gunIndex);
                })
                .AddTo(this);
        }

        if (benelliActiv)
        {
            Observable.Timer(System.TimeSpan.FromSeconds(0.15))
                .Subscribe(_ =>
                {
                    IconActivateWepons.OnNext(benelliIndex);
                })
                .AddTo(this);
        }

        if (ak74Activ)
        {
            Observable.Timer(System.TimeSpan.FromSeconds(0.2))
                .Subscribe(_ =>
                {
                    IconActivateWepons.OnNext(ak74Index);
                })
                .AddTo(this);
        }

        // Удаляем объект через 2 секунды  
        Observable.Timer(System.TimeSpan.FromSeconds(2))
            .Subscribe(_ =>
            {
                Destroy(this.gameObject);
            })
            .AddTo(this);
    }
}

