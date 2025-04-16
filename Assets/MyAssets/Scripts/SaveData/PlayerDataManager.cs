using UnityEngine;
using Zenject;
using UniRx;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    [Inject]
    PlayerSwitchingStates playerSwitchingStates;

    // ������ ������
    public int GunAmmo { get; private set; }
    public int BennelliAmmo { get; private set; }
    public int Ak74Ammo { get; private set; }
    

    private void Awake()
    {
        // ���������� Singleton
        if (Instance == null)
        {
           
            Instance = this;
            DontDestroyOnLoad(gameObject); // ��������� ������ ����� ��������
            LoadData(); // ��������� ������ ��� ������
        }
        else
        {
            Destroy(gameObject); // ���������� ����������� ������
        }
    }

    // ����� ��� ���������� ������ � PlayerPrefs
    public void SaveData()
    {
        PlayerPrefs.SetInt("GunAmmo", GunAmmo);
        PlayerPrefs.SetInt("BennelliAmmo", BennelliAmmo);
        PlayerPrefs.SetInt("Ak74Ammo", Ak74Ammo);
        PlayerPrefs.Save(); // ��������� ���������
        Debug.Log("������ ���������:");
    }

    // ����� ��� �������� ������ �� PlayerPrefs
    public void LoadData()
    {
        
        GunAmmo = PlayerPrefs.GetInt("GunAmmo", 0); // ��������� ���������� �������� (0 � �������� �� ���������)
        BennelliAmmo = PlayerPrefs.GetInt("BennelliAmmo", 0); // ��������� ���������� �������� (0 � �������� �� ���������)
        Ak74Ammo = PlayerPrefs.GetInt("Ak74Ammo", 0); // ��������� ���������� �������� (0 � �������� �� ���������)        
        Debug.Log("������ ���������:");
    }

    // ����� ��� ���������� ������ ������
    public void UpdateData(int gun, int bennelli,int ak74)
    {
        GunAmmo = gun;
        BennelliAmmo = bennelli;
        Ak74Ammo = ak74;

        Debug.Log("������ ���������: ");
    }

    // ����� ��� ���������� ������� ������ � ��������
    private void SaveCurrentAmmoData()
    {
        // �������� ������ �� PlayerSwitchingStates
        GunAmmo = PlayerSwitchingStates.weaponBullets["Gun"];
        BennelliAmmo = PlayerSwitchingStates.weaponBullets["Bennelli_M4"];
        Ak74Ammo = PlayerSwitchingStates.weaponBullets["AK74"];

        // ��������� ������ � PlayerPrefs
        SaveData();
    }

    private void OnEnable()
    {
        // ������������� �� ������� 
        TriggerFinish.OnSaveData.Subscribe(_ => SaveCurrentAmmoData()).AddTo(this);

        __StartLevel.OnLoadData.Subscribe(_ => LoadData()).AddTo(this);
    }

    private void OnDisable()
    {
        // ������������ �� ������� 
        TriggerFinish.OnSaveData.Dispose();

        __StartLevel.OnLoadData.Dispose();
    }



}

