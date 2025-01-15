using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class Zombi : MonoBehaviour
{
    [Inject]
    PlayerSwitchingStates Player;


    public enum ZombiStatus { idle, run, attack, injury, dead }
    public ZombiStatus statusZombi;
    private ZombiStatus previusStatus;

    private static List<GameObject> activeZombies = new List<GameObject>();
    private bool isRunning = false;


    Animator animator;
    NavMeshAgent ZombyAgent;

    GameObject shootZombi;

    float LifeZombi = 100;

    float distanceToPlayer;

    float stoppingDistance = 1.8f;

    bool onAttack = false;

    public float bodyClearTime = 30;

    


    private void Awake()
    {

        animator = this.gameObject.GetComponentInChildren<Animator>();

        ZombyAgent = this.gameObject.GetComponent<NavMeshAgent>();

        ;
    }


    

    private void Start()
    {
        // Устанавливаем начальное состояние при запуске игры
        previusStatus = statusZombi;
        EnterState(statusZombi);


       


      

    }





//-------------------------------------------------------------------------------------События подписки--------------------------------------------------

    private void OnEnable()
    {
        PlayerSwitchingStates.PlayerFire += DemageZombi;

        StateZombiHit.ZombiInjury += ZombyInjurnyEndAnimation;

        StateZombiAttack.ZombiAttack += OnZombiAttack;
    }

    private void OnDisable()
    {
        PlayerSwitchingStates.PlayerFire -= DemageZombi;

        StateZombiHit.ZombiInjury -= ZombyInjurnyEndAnimation;

        StateZombiAttack.ZombiAttack -= OnZombiAttack;
    }


//------------------------------------------------------------------------------------





    private void Update()
    {
        OnStateZombi();
        UpdateState(); // Вызов метода обновления для текущего состояния



        if (Input.GetKeyDown(KeyCode.T))
        {

            statusZombi = ZombiStatus.run;


        }


       

    }











    private void OnStateZombi()
    {
        // Для разового исполнения
        if (statusZombi != previusStatus)
        {
            ExitState(previusStatus); // Выход из предыдущего состояния                     
            EnterState(statusZombi);   // Вход в новое состояние
            previusStatus = statusZombi;
        }
    }












    //---------------------------idle---------------Start and Update-----------------------------------------------------------------/////////////////////////////////////////
    private void StartIdle()
    {
        // Логика для состояния idle

        
        ZombyAgent.speed = 0f;

    }

    private void UpdateIdle()
    {
        // Логика обновления для состояния idle

    }







    //-----------------------------Run--------------------------------------------------------------------------///////////////////////////////////////////////////

    private void StartRun()
    {
        // Логика для состояния run
        foreach (GameObject zombie in activeZombies)
        {
            if (zombie != null)
            {
                if (zombie == this.gameObject)
                {
                    ZombyAgent.speed = 3;
                    animator.SetBool("isRun", true);
                    animator.SetBool("isAttack", false);


                    isRunning = animator.GetBool("isRun");
                }
            }
        }


        
        
        




    }

    
    private void UpdateRun()
    {

         distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);

        if (isRunning)
        {
                ZombyAgent.SetDestination(Player.transform.position);  
        }
        
        if(distanceToPlayer < stoppingDistance)
        {
            ZombyAgent.speed = 0;

            statusZombi = ZombiStatus.attack;
            print("Distance ------ "+stoppingDistance);

        }
        else
        {
            ZombyAgent.speed = 3;
        }

    }








    //-----------------------------------------Attack-------------------------------------------------------------------///////////////////////////////////////////////////

    private void StartAttack()
    {
        // Логика для состояния attack     

                    animator.SetBool("isAttack", true);
        
    }

    private void UpdateAttack()
    {
        // Логика обновления для состояния attack

        distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);
      

        if (distanceToPlayer >= stoppingDistance && onAttack)
        {
                      
              animator.SetBool("isAttack", false);
              statusZombi = ZombiStatus.run;
                    
        }
        
       


    }


   void OnZombiAttack(bool onAttackZombi)
    {
        onAttack = onAttackZombi;
    }





    //-----------------------------------------Injury-------------------------------------------------------------------///////////////////////////////////////////////////


    //Start
    private void StartInjury()
    {
        
        animator.SetBool("isHit", true);
          
        

        if(shootZombi == this.gameObject) 
        {
            ZombyAgent.speed = 0.1f;
        }
         

        


    }

    //Update
    private void UpdateInjury()
    {

    }



    //Событие с StateZombiHit , Срабатывает в конце анимации
    public void ZombyInjurnyEndAnimation()
    {
        
        animator.SetBool("isHit", false);

       
            

        ZombyAgent.speed = 3;
            
        


        if (LifeZombi > 0)
        {
            statusZombi = ZombiStatus.run;
            
        }
       else
        {
            ZombyAgent.speed = 0;     
        }

    }

    //----------------------------------------------------Dead-------------------------------------------------------------------------------------////////////////////////////////



    public void StartDead()
    {
        // Логика для состояния Dead


        ZombyAgent.speed = 0f;

        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;

        StartCoroutine(WaitClierBody());

    }

    public void UpdateDead()
    {
       
    }



   

public IEnumerator WaitClierBody()
{
    // Задержка 
    yield return new WaitForSeconds(bodyClearTime);

        // Действие после задержки
        Destroy(this.gameObject);
}





//------------------------------------------------------------------------------///////////////











//EXIT------------------------------------------------------------------------------------------------------------------------------------------
//Вписывать метод Выхода из состояния
private void ExitState(ZombiStatus state)
    {
        switch (state)
        {
            case ZombiStatus.idle:
                Debug.Log("Выход из состояния: Idle");
                break;

            case ZombiStatus.run:
                Debug.Log("Выход из состояния: Run");

                animator.SetBool("isRun", false);


                break;

            case ZombiStatus.attack:
                Debug.Log("Выход из состояния: Attack");
                break;

            case ZombiStatus.injury:
                Debug.Log("Выход из состояния: Injury");
                break;

            case ZombiStatus.dead:
                Debug.Log("Выход из состояния: Dead");
                break;
        }
    }

















    














    //поподание в зомби
    void DemageZombi(float demage, RaycastHit hit)
    {

        shootZombi = hit.collider.gameObject;                   // GameObject shootZombi = Зомби в которого попали;


        if (shootZombi == this.gameObject)                      //Добовляем зомби в массив лист
        {
            if (!activeZombies.Contains(this.gameObject))   
            {
                activeZombies.Add(this.gameObject);             
            }
        }
      

        if (shootZombi == this.gameObject)
        {
            

            LifeZombi = LifeZombi - demage;

            print("Hit " + LifeZombi);

            if (LifeZombi > 0)
            {
                statusZombi = ZombiStatus.injury;
            }
            else
            {
                animator.SetTrigger("isDead");            // Убийство

                statusZombi = ZombiStatus.dead;
            }


        }

    }















    //Вписывать метод Update состояния
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

            case ZombiStatus.dead:
                UpdateDead();
                break;
        }
    }



    
    //Вписывать метод старта состояния
    private void EnterState(ZombiStatus state)
    {
        switch (state)
        {
            case ZombiStatus.idle:
                Debug.Log("Вход в состояние: Idle");
                StartIdle();
                break;

            case ZombiStatus.run:
                Debug.Log("Вход в состояние: Run");
                StartRun();
                break;

            case ZombiStatus.attack:
                Debug.Log("Вход в состояние: Attack");
                StartAttack();
                break;

            case ZombiStatus.injury:
                Debug.Log("Вход в состояние: Injury");
                StartInjury();
                break;

            case ZombiStatus.dead:
                Debug.Log("Вход в состояние: Dead");
                StartDead();
                break;
        }
    }


    
}
