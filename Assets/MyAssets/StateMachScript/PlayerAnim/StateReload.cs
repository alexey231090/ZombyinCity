using UnityEngine;
using UniRx;





public class StateReload : StateMachineBehaviour
{

    public static readonly Subject<Unit> reloadGunSubject = new();
    public static readonly Subject<Unit> reloadBennelliSubject = new();

    PlayerSwitchingStates playerSwitchingStates;
    public enum Wepon
    {
        None,
        Crowbar,
        Gun,
        Bennelli,
        Ak74
    }
    int reloadInt = 0;

    public Wepon wepon;

    IStateReload stateReload;

    bool hasExecuted = true; // Флаг для отслеживания выполнения



    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

         hasExecuted = false; // Флаг для отслеживания выполнения

        playerSwitchingStates = FindObjectOfType<PlayerSwitchingStates>();

        switch (wepon)
        {
            case Wepon.Bennelli:


                stateReload = new PlayerBennelliBehaviour();
               

                break;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        switch (wepon)
        {
            case Wepon.Bennelli:
              



                    if (stateInfo.normalizedTime >= 0.2f && !hasExecuted )
                {



                    Debug.Log("Reload State");
                    // Ваш код, который должен выполняться в нужном кадре анимации                       

                        stateReload.Reload(playerSwitchingStates, animator); //Метод интерфейса

                    reloadBennelliSubject.OnNext(Unit.Default);

                        hasExecuted = true;

                }
                    break;

            case Wepon.Gun:


                if (stateInfo.normalizedTime >= 0.8f )
                {
                    reloadGunSubject.OnNext(Unit.Default);
                }


                    break;

        }


    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
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
