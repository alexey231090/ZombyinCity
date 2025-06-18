using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public class TriggerFinish : MonoBehaviour
{
    public static readonly Subject<Unit> OnSaveData = new Subject<Unit>(); // ������� ��� ���������� ������
    

    public string levelName; // ��� ������, �� ������� ����� �������
    
    private void Start()
    {
        // ������������� �� ������� ���������� ������
        OnSaveData.Subscribe(_ => SavePlayerData()).AddTo(this);
    }

    private void SavePlayerData()
    {
        // ���������, ���������� �� PlayerDataManager
        if (PlayerDataManager.Instance != null)
        {
            // ���������� ������� ������� ����� �����������
            int currentGunAmmo = PlayerSwitchingStates.weaponBullets.ContainsKey("Gun") ? PlayerSwitchingStates.weaponBullets["Gun"] : 0;
            int currentBennelliAmmo = PlayerSwitchingStates.weaponBullets.ContainsKey("Bennelli_M4") ? PlayerSwitchingStates.weaponBullets["Bennelli_M4"] : 0;
            int currentAk74Ammo = PlayerSwitchingStates.weaponBullets.ContainsKey("AK74") ? PlayerSwitchingStates.weaponBullets["AK74"] : 0;
            
            Debug.Log($"TriggerFinish - ������� �������: Gun={currentGunAmmo}, Bennelli={currentBennelliAmmo}, AK74={currentAk74Ammo}");
            
            // ��������� ������ ��� ���������� ������
            PlayerDataManager.Instance.SaveDataForCurrentScene();
            Debug.Log("TriggerFinish - ������ ������ ��������� ��� ���������� ������");
        }
        else
        {
            Debug.LogWarning("TriggerFinish - PlayerDataManager �� ������!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ���������, �������� �� ������, � ������� ��������� ������������, �������
        if (other.CompareTag("Player"))
        {
            OnSaveData.OnNext(Unit.Default); // �������� ������� ���������� ������
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
