using UnityEngine;
using FpsZomby;
using UniRx;

public class PlayerCrowbarBehaviur : IPlayerBehaviour,IStateFire
{

    const string CrowbarHit = "CrowbarHit";
    const string CrowbarMiss = "CrowbarMiss";
    int myIndex = 0;
    PlayerSwitchingStates switchState;
    Player player;
    Animator animator;
    Ray ray;
    private RaycastHit hit;
    float spread = 5;  //Разброс
    private bool hasFired = true;   //один удар
    float crowBarDistance = 1.5f;   //Дистанция до стены
    int myDemage = 15;

    public static readonly Subject<int> crowIntSubject = new();

    SoundManager soundManager;

    public void Enter()
    {

        Debug.Log("Вход _ Crowbar");
        switchState = GameObject.FindObjectOfType<PlayerSwitchingStates>();
        animator = GameObject.FindGameObjectWithTag("Hand").GetComponent<Animator>();
        player = GameObject.FindObjectOfType(typeof(Player)) as Player;
        soundManager = GameObject.FindObjectOfType( typeof(SoundManager)) as SoundManager;

        crowIntSubject.OnNext(0);

        switchState.weponArray[myIndex].SetActive(true);

        animator.SetInteger("WeponNum", myIndex);

        CanvasFirstPerson.canvasAmmoInt = 0;

        switchState.Demage = myDemage;

        soundManager.SelectOther.Play();



        PlayerSwitchingStates.isDead.Subscribe(x =>
        {
            animator.SetInteger("WeponNum", -1);
        });

    }

    public void Exit()
    {
        Debug.Log("Выход _ Crowbar");

    }

    

    public void Update()
    {

       

        if (Input.GetMouseButtonDown(0))
        {
             ray = switchState.RayCastCameraCenter();

            if (Physics.Raycast(ray, out hit))
            {
                // Получаем расстояние от начала луча до объекта
                float distance = hit.distance;

               if(hit.distance < crowBarDistance)
                {

                    animator.SetTrigger(CrowbarHit);

                }

               else
                {
                    animator.SetTrigger(CrowbarMiss);
                    soundManager.FireAudio[5].Play();
                }


            }

           
        }  
        


    }

    public void FireWepon(PlayerSwitchingStates playerSwitching, SoundManager soundManager)
    {
        if (hasFired)
        {
            ray = playerSwitching.RayCastCameraCenter();

            if (Physics.Raycast(ray, out hit))
            {
                // Получаем расстояние от начала луча до объекта
                float distance = hit.distance;

                if (hit.distance < crowBarDistance)
                {
                    GameObject bullet = playerSwitching.bullets;
                    playerSwitching.ShootInWall(playerSwitching.TraceShot(spread), bullet);

                    soundManager.FireAudio[6].Play();

                    hasFired = !hasFired;
                }
            }
        }
    }

    
}
