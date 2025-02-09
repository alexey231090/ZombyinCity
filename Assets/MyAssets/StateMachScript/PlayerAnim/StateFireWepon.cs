using UnityEngine;
using Zenject;


public class StateFireWepon : StateMachineBehaviour
{
    [Inject]
    PlayerSwitchingStates playerSwitchingStates;

    [Inject]
    SoundManager soundManager;
   public enum Wepon
    {
        None,
        Crowbar,
        Gun,
        Bennelli,
        Ak74
    }

   public Wepon wepon;

    bool hasExecuted; 

    IStateFire stateFire;  
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
         hasExecuted = false;

        switch (wepon)
        {
            case Wepon.None:
                
                break;

            case Wepon.Crowbar:

                stateFire = new PlayerCrowbarBehaviur();
                break;

            case Wepon.Gun:

                stateFire = new PlayerGunBehaviur();
                stateFire.FireWepon(playerSwitchingStates,soundManager);

                break;
                case Wepon.Bennelli:

                stateFire = new PlayerBennelliBehaviour();
                stateFire.FireWepon(playerSwitchingStates,soundManager);
                break;

                 case Wepon.Ak74:

                stateFire = new PlayerAk74Behaviour();
                stateFire.FireWepon(playerSwitchingStates,soundManager);
                break;
        }
       
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (wepon)
        {
            case Wepon.Ak74:
              
                stateFire.FireWepon(playerSwitchingStates, soundManager);
                break;
            case Wepon.Crowbar:
               
                if (stateInfo.normalizedTime >= 0.451f)
                {
                   
                    // ��� ���, ������� ������ ����������� � ������ ����� ��������
                    stateFire.FireWepon(playerSwitchingStates , soundManager);
                    
                }
                break;

                case Wepon.Bennelli:

                if (stateInfo.normalizedTime >= 0.4f && hasExecuted == false)
                {

                    soundManager.ReloadAudio[3].Play();
      
                    hasExecuted = true;
                }

                break;

        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
   /* override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        

    }*/

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}


   
}
