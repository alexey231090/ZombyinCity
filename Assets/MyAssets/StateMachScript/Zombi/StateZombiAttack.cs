using System;
using UnityEngine;


public class StateZombiAttack : StateMachineBehaviour
{

    public static event Action<bool> ZombiAttack;
    public static event Action ZombiEndAttack;
    

    private bool hasTriggeredAt0_6;

    float timeAttack = 0.45f;  //���������� ����� ����� � ��������.

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

        // ���������, �������� �� �� 0.6 � ������� ��� �� ���� �������
        if (stateInfo.normalizedTime >= timeAttack && !hasTriggeredAt0_6)
        {
            ZombiEndAttack?.Invoke();
            hasTriggeredAt0_6 = true; // ������������� �������, ����� ������� �� ���������� ��������
        }

        // ���������, ����������� �� ��������
        if (stateInfo.normalizedTime >= 1f || !animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Zombie Attack"))
        {
            // ���������� ��������
            animator.Play(stateInfo.shortNameHash, layerIndex, 0.1f);
            hasTriggeredAt0_6 = false; // ���������� �������, ����� ������� ����� ��������� �����
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
