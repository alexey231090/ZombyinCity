using System;
using UnityEngine;


public class StateZombiAttack : StateMachineBehaviour
{

    public static event Action<bool> ZombiAttack;
    public static event Action ZombiEndAttack;
    

    private bool hasTriggeredAt0_6;

    float timeAttack = 0.45f;  //Синхронная атака зомби с событием.

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
       
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 0.8f)
        {
            ZombiAttack?.Invoke(true);
        }
        else
        {
            ZombiAttack?.Invoke(false);
        }

        // Проверяем, достигли ли мы 0.6 и событие еще не было вызвано
        if (stateInfo.normalizedTime >= timeAttack && !hasTriggeredAt0_6)
        {
            ZombiEndAttack?.Invoke();
            hasTriggeredAt0_6 = true; // Устанавливаем триггер, чтобы событие не вызывалось повторно
        }

        // Проверяем, завершилась ли анимация
        if (stateInfo.normalizedTime >= 1f || !animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Zombie Attack"))
        {
            // Сбрасываем анимацию
            animator.Play(stateInfo.shortNameHash, layerIndex, 0.1f);
            hasTriggeredAt0_6 = false; // Сбрасываем триггер, чтобы событие могло сработать снова
            Debug.Log("ExitAtt");
        }
    }



    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        
        ZombiAttack?.Invoke(true);

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
