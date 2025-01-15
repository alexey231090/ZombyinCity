using UnityEngine;

public class STATEPATTERN : MonoBehaviour
{
    public enum ZombiStatus { idle, run, attack, injury }
    public ZombiStatus statusZombi;
    private ZombiStatus previusStatus;

    private void Update()
    {
        OnStateZombi();
        UpdateState(); // Вызов метода обновления для текущего состояния
    }

    private void OnStateZombi()
    {
        // Для разового исполнения
        if (statusZombi != previusStatus)
        {
            ExitState(previusStatus); // Выход из предыдущего состояния
            ExecuteState();            // Выполнение нового состояния
            EnterState(statusZombi);   // Вход в новое состояние
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
                Debug.Log("Вход в состояние: Idle");
                break;

            case ZombiStatus.run:
                Debug.Log("Вход в состояние: Run");
                break;

            case ZombiStatus.attack:
                Debug.Log("Вход в состояние: Attack");
                break;

            case ZombiStatus.injury:
                Debug.Log("Вход в состояние: Injury");
                break;
        }
    }

    private void ExitState(ZombiStatus state)
    {
        switch (state)
        {
            case ZombiStatus.idle:
                Debug.Log("Выход из состояния: Idle");
                break;

            case ZombiStatus.run:
                Debug.Log("Выход из состояния: Run");
                break;

            case ZombiStatus.attack:
                Debug.Log("Выход из состояния: Attack");
                break;

            case ZombiStatus.injury:
                Debug.Log("Выход из состояния: Injury");
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
        // Логика обновления для состояния idle
        
    }

    private void UpdateRun()
    {
        // Логика обновления для состояния run
       
    }

    private void UpdateAttack()
    {
        // Логика обновления для состояния attack
       
    }

    private void UpdateInjury()
    {
        // Логика обновления для состояния injury
       
    }

    private void SetBehaviorIdle()
    {
        // Логика для состояния idle
       
    }

    private void SetBehaviorRun()
    {
        // Логика для состояния run
       
    }

    private void SetBehaviorAttack()
    {
        // Логика для состояния attack
        
    }

    private void SetZombieInjury()
    {
        // Логика для состояния injury
       
    }
}
