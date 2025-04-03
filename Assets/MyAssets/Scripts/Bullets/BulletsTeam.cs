using UnityEngine;
using Zenject;

public class BulletsTeam : MonoBehaviour
{

    [Inject]
    CanvasFirstPerson playerSwitch;

    public enum BulletWeapons
    {
        Gun,
        Bennelli_M4,
        AK74,
        None
           
    }

    [SerializeField] public int quantity;

    [SerializeField] public BulletWeapons weaponsBullets;

    // Метод, который выполняется перед удалением
    private void OnDestroy()
    {
        // Ваш код здесь
        Debug.Log("Объект BulletsTeam будет удален");
    }
}
