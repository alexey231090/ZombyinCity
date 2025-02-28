using UnityEngine;

public class UpWepon : MonoBehaviour
{
    
    public PlayerSwitchingStates.Weapons Upweapon;

    private void OnTriggerEnter(Collider other)
    {
        //Поднял оружие
        PlayerSwitchingStates.weapon = Upweapon;

        //Активация оружия
        switch (Upweapon)
        {
            case PlayerSwitchingStates.Weapons.Crowbar:
                PlayerSwitchingStates.CrowbarIsActive = true;
                break;
            case PlayerSwitchingStates.Weapons.Gun:
                PlayerSwitchingStates.GunIsActive = true;
                break;
            case PlayerSwitchingStates.Weapons.Bennelli_M4:
                PlayerSwitchingStates.BennelliIsActive = true;
                break;
            case PlayerSwitchingStates.Weapons.AK74:
                PlayerSwitchingStates.AK74IsActive = true;
                break;
        }
                
    }
}
