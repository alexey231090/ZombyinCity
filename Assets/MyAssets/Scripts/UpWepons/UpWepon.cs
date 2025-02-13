using UnityEngine;

public class UpWepon : MonoBehaviour
{
    
    public PlayerSwitchingStates.Weapons Upweapon;

    private void OnTriggerEnter(Collider other)
    {
        PlayerSwitchingStates.weapon = Upweapon;
    }
}
