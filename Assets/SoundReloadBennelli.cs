
using UnityEngine;
using Zenject;

public class SoundReloadBennelli : StateMachineBehaviour
{
    [Inject]
    SoundManager soundManager;
    bool hasExecuted = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       //soundManager = GameObject.FindObjectOfType<SoundManager>();

       hasExecuted = false;


        // soundManager.ReloadAudio[3].Play();


    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 0.4 && hasExecuted == false)
        {
            soundManager.ReloadAudio[3].Play();

            Debug.Log("PlaY!");

            hasExecuted = true;
        }

        //Debug.Log("other " + stateInfo.normalizedTime);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
