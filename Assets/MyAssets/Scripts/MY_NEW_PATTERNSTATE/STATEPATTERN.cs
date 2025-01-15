using UnityEngine;

public class STATEPATTERN : MonoBehaviour
{
    public enum ZombiStatus { idle, run, attack, injury }
    public ZombiStatus statusZombi;
    private ZombiStatus previusStatus;

    private void Update()
    {
        OnStateZombi();
        UpdateState(); // ����� ������ ���������� ��� �������� ���������
    }

    private void OnStateZombi()
    {
        // ��� �������� ����������
        if (statusZombi != previusStatus)
        {
            ExitState(previusStatus); // ����� �� ����������� ���������
            ExecuteState();            // ���������� ������ ���������
            EnterState(statusZombi);   // ���� � ����� ���������
            previusStatus = statusZombi;
        }
    }

    private void ExecuteState()
    {
        switch (statusZombi)
        {
            case ZombiStatus.idle:
                SetBehaviorIdle();
                break;

            case ZombiStatus.run:
                SetBehaviorRun();
                break;

            case ZombiStatus.attack:
                SetBehaviorAttack();
                break;

            case ZombiStatus.injury:
                SetZombieInjury();
                break;
        }
    }

    private void EnterState(ZombiStatus state)
    {
        switch (state)
        {
            case ZombiStatus.idle:
                Debug.Log("���� � ���������: Idle");
                break;

            case ZombiStatus.run:
                Debug.Log("���� � ���������: Run");
                break;

            case ZombiStatus.attack:
                Debug.Log("���� � ���������: Attack");
                break;

            case ZombiStatus.injury:
                Debug.Log("���� � ���������: Injury");
                break;
        }
    }

    private void ExitState(ZombiStatus state)
    {
        switch (state)
        {
            case ZombiStatus.idle:
                Debug.Log("����� �� ���������: Idle");
                break;

            case ZombiStatus.run:
                Debug.Log("����� �� ���������: Run");
                break;

            case ZombiStatus.attack:
                Debug.Log("����� �� ���������: Attack");
                break;

            case ZombiStatus.injury:
                Debug.Log("����� �� ���������: Injury");
                break;
        }
    }

    private void UpdateState()
    {
        switch (statusZombi)
        {
            case ZombiStatus.idle:
                UpdateIdle();
                break;

            case ZombiStatus.run:
                UpdateRun();
                break;

            case ZombiStatus.attack:
                UpdateAttack();
                break;

            case ZombiStatus.injury:
                UpdateInjury();
                break;
        }
    }

    private void UpdateIdle()
    {
        // ������ ���������� ��� ��������� idle
        
    }

    private void UpdateRun()
    {
        // ������ ���������� ��� ��������� run
       
    }

    private void UpdateAttack()
    {
        // ������ ���������� ��� ��������� attack
       
    }

    private void UpdateInjury()
    {
        // ������ ���������� ��� ��������� injury
       
    }

    private void SetBehaviorIdle()
    {
        // ������ ��� ��������� idle
       
    }

    private void SetBehaviorRun()
    {
        // ������ ��� ��������� run
       
    }

    private void SetBehaviorAttack()
    {
        // ������ ��� ��������� attack
        
    }

    private void SetZombieInjury()
    {
        // ������ ��� ��������� injury
       
    }
}
