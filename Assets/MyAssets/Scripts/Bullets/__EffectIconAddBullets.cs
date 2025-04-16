using UnityEngine;
using UniRx;
using System.Collections.Generic;


public class __EffectIconAddBullets : MonoBehaviour
{
    private BulletsTeam bulletsTeam;
    public static Subject<KeyValuePair<BulletsTeam.BulletWeapons, int>> bulletsSubject = new Subject<KeyValuePair<BulletsTeam.BulletWeapons, int>>();

    private void Start()
    {
        bulletsTeam = this.gameObject.GetComponent<BulletsTeam>();
    }

    private void OnDestroy()
    {
        if (bulletsTeam == null)
        {
            Debug.Log("Erorr_ BulletsTeam is null or canvasPlayer is null");
        }
        else
        {
            var bulletsData = new KeyValuePair<BulletsTeam.BulletWeapons, int>(bulletsTeam.weaponsBullets, bulletsTeam.quantity);
            bulletsSubject.OnNext(bulletsData);
        }
    }
}
