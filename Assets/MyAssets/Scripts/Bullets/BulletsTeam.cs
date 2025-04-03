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

    // �����, ������� ����������� ����� ���������
    private void OnDestroy()
    {
        // ��� ��� �����
        Debug.Log("������ BulletsTeam ����� ������");
    }
}
