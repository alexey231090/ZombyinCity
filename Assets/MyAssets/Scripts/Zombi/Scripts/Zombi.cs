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
        // ������������� ��������� ��������� ��� ������� ����
        previusStatus = statusZombi;
        EnterState(statusZombi);


       


      

    }





//-------------------------------------------------------------------------------------������� ��������--------------------------------------------------

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
        UpdateState(); // ����� ������ ���������� ��� �������� ���������



        if (Input.GetKeyDown(KeyCode.T))
        {

            statusZombi = ZombiStatus.run;


        }


       

    }











    private void OnStateZombi()
    {
        // ��� �������� ����������
        if (statusZombi != previusStatus)
        {
            ExitState(previusStatus); // ����� �� ����������� ���������                     
            EnterState(statusZombi);   // ���� � ����� ���������
            previusStatus = statusZombi;
        }
    }












    //---------------------------idle---------------Start and Update-----------------------------------------------------------------/////////////////////////////////////////
    private void StartIdle()
    {
        // ������ ��� ��������� idle

        
        ZombyAgent.speed = 0f;

    }

    private void UpdateIdle()
    {
        // ������ ���������� ��� ��������� idle

    }







    //-----------------------------Run--------------------------------------------------------------------------///////////////////////////////////////////////////

    private void StartRun()
    {
        // ������ ��� ��������� run
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
        // ������ ��� ��������� attack     

                    animator.SetBool("isAttack", true);
        
    }

    private void UpdateAttack()
    {
        // ������ ���������� ��� ��������� attack

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



    //������� � StateZombiHit , ����������� � ����� ��������
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
        // ������ ��� ��������� Dead


        ZombyAgent.speed = 0f;

        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;

        StartCoroutine(WaitClierBody());

    }

    public void UpdateDead()
    {
       
    }



   

public IEnumerator WaitClierBody()
{
    // �������� 
    yield return new WaitForSeconds(bodyClearTime);

        // �������� ����� ��������
        Destroy(this.gameObject);
}





//------------------------------------------------------------------------------///////////////











//EXIT------------------------------------------------------------------------------------------------------------------------------------------
//��������� ����� ������ �� ���������
private void ExitState(ZombiStatus state)
    {
        switch (state)
        {
            case ZombiStatus.idle:
                Debug.Log("����� �� ���������: Idle");
                break;

            case ZombiStatus.run:
                Debug.Log("����� �� ���������: Run");

                animator.SetBool("isRun", false);


                break;

            case ZombiStatus.attack:
                Debug.Log("����� �� ���������: Attack");
                break;

            case ZombiStatus.injury:
                Debug.Log("����� �� ���������: Injury");
                break;

            case ZombiStatus.dead:
                Debug.Log("����� �� ���������: Dead");
                break;
        }
    }

















    














    //��������� � �����
    void DemageZombi(float demage, RaycastHit hit)
    {

        shootZombi = hit.collider.gameObject;                   // GameObject shootZombi = ����� � �������� ������;


        if (shootZombi == this.gameObject)                      //��������� ����� � ������ ����
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
                animator.SetTrigger("isDead");            // ��������

                statusZombi = ZombiStatus.dead;
            }


        }

    }















    //��������� ����� Update ���������
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



    
    //��������� ����� ������ ���������
    private void EnterState(ZombiStatus state)
    {
        switch (state)
        {
            case ZombiStatus.idle:
                Debug.Log("���� � ���������: Idle");
                StartIdle();
                break;

            case ZombiStatus.run:
                Debug.Log("���� � ���������: Run");
                StartRun();
                break;

            case ZombiStatus.attack:
                Debug.Log("���� � ���������: Attack");
                StartAttack();
                break;

            case ZombiStatus.injury:
                Debug.Log("���� � ���������: Injury");
                StartInjury();
                break;

            case ZombiStatus.dead:
                Debug.Log("���� � ���������: Dead");
                StartDead();
                break;
        }
    }


    
}
