using UnityEngine;

public class BulletsTeam : MonoBehaviour
{

    public enum BulletWeapons
    {
        Gun,
        Bennelli_M4,
        AK74
    }

   [SerializeField] public int quantity;


   [SerializeField] public BulletWeapons weaponsBullets;


   


}
