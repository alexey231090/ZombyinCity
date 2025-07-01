using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using System;
namespace FpsZomby {
    public class Zombi : MonoBehaviour
    {

        float zombiSpeedDebug = 0;
        [Inject]
        PlayerSwitchingStates Player;

        public event Action ZombiInjury; // ������� ��� ������� �����

        public enum ZombiStatus { idle, run, attack, injury, dead }
        public ZombiStatus statusZombi;
        private ZombiStatus previousStatus;

        private static List<GameObject> activeZombies = new List<GameObject>();
        private bool isRunning = false;

        Animator animator;
        NavMeshAgent zombyAgent;

        GameObject shootZombi;

        [SerializeField] public float lifeZombi = 100;

        float distanceToPlayer;

        float stoppingDistance = 1.8f;

        bool onAttack = false;

        public float bodyClearTime = 30;

        public ParticleSystem footDust;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            zombyAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            previousStatus = statusZombi;
            EnterState(statusZombi);

            if (zombyAgent != null && !zombyAgent.isOnNavMesh)
    {
        // ������� ����� ��������� ����� �� NavMesh � ��������������� ����
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            Debug.Log($"{gameObject.name}: �������������� �� NavMesh!");
        }
        else
        {
            Debug.LogError($"{gameObject.name}: �� ������� ����� NavMesh �����!");
        }
    }
        }

        private void OnEnable()
        {
            PlayerSwitchingStates.PlayerFire += DemageZombi;
            StateZombiHit.ZombiInjury += ZombyInjurnyEndAnimation; // �������� �� ������� �������
            StateZombiAttack.ZombiAttack += OnZombiAttack;
        }

        private void OnDisable()
        {
            PlayerSwitchingStates.PlayerFire -= DemageZombi;
            StateZombiHit.ZombiInjury -= ZombyInjurnyEndAnimation; // ������� �� ������� �������
            StateZombiAttack.ZombiAttack -= OnZombiAttack;
        }

        private void Update()
        {
            if (zombyAgent != null)
            {
                if (!zombyAgent.enabled)
                    Debug.LogWarning($"{gameObject.name}: NavMeshAgent ��������!");
                else if (!zombyAgent.isOnNavMesh)
                    Debug.LogError($"{gameObject.name}: NavMeshAgent �� �� �����!");
            }
            zombiSpeedDebug = zombyAgent.speed;
            OnStateZombi();
            UpdateState(); // ����� ������ ���������� ��� �������� ���������

            if (Input.GetKeyDown(KeyCode.T))
            {
                statusZombi = ZombiStatus.run;
            }
        }

        private void OnStateZombi()
        {
            // ���� ��������� ����� ����������
            if (statusZombi != previousStatus)
            {
                ExitState(previousStatus); // ����� �� ����������� ���������
                EnterState(statusZombi);   // ���� � ����� ���������
                previousStatus = statusZombi;
            }
        }

        private void StartIdle()
        {
            // ������ ��� ��������� idle
            zombyAgent.speed = 0f;
        }

        private void UpdateIdle()
        {
            // ������ ���������� ��� ��������� idle
        }

        private void StartRun()
        {
            if (zombyAgent == null)
            {
                Debug.LogError($"{gameObject.name}: NavMeshAgent �� ������!");
                return;
            }

            if (!zombyAgent.enabled)
            {
                Debug.LogWarning($"{gameObject.name}: NavMeshAgent �������� (enabled = false)!");
                return;
            }

            if (!zombyAgent.isOnNavMesh)
            {
                Debug.LogError($"{gameObject.name}: NavMeshAgent �� �� �����! (isOnNavMesh = false)");
                return;
            }

            // ���� �� �� � ��������� ��������
            Debug.Log($"{gameObject.name}: StartRun OK � ����� �� �����, ��������� ��������!");
            zombyAgent.isStopped = false;
            zombyAgent.speed = 3;
            zombyAgent.SetDestination(Player.transform.position);

            animator.SetBool("isRun", true);
            animator.SetBool("isAttack", false);
            isRunning = animator.GetBool("isRun");
            footDust.Play();
        }

        private void UpdateRun()
        {
            // ������: ������ ��� ������ �������, ���������, ��� ����� ��� � �� �����.
            // ��� �������� ������ "SetDestination can only be called on an active agent".
            if (zombyAgent == null || !zombyAgent.enabled || !zombyAgent.isOnNavMesh)
            {
                return;
            }

            distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);
            
            
            if (statusZombi == ZombiStatus.run)
            {
                zombyAgent.SetDestination(Player.transform.position);
            }
        //������ ����� �����
            if (distanceToPlayer < stoppingDistance)
            {
                zombyAgent.speed = 0;
                statusZombi = ZombiStatus.attack;
                Debug.Log("Distance ------ " + stoppingDistance);
            }
            else
            {
                zombyAgent.speed = 3;
            }
        }

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

        private void StartInjury()
        {
            // �������� ������� ������ ����� ���������� ���������,
            // �� �� ��-�������� ������ ���������� �����.
            if(zombyAgent.enabled)
            {
                zombyAgent.isStopped = true; // ���������� isStopped ��� ������ ���������
            }
        }

        
        public void ZombyInjurnyEndAnimation()
        {
            // �������: ������ ���� ���� �������� ������ � ��������� �������
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Reaction Hit"))
                return;

            if (lifeZombi > 0)
            {
                statusZombi = ZombiStatus.run;
                if (zombyAgent.enabled && zombyAgent.isOnNavMesh)
                {
                    zombyAgent.speed = 3; // ���������� �������� ����� �������
                    zombyAgent.isStopped = false; // ������� ����
                    if (Player != null)
                        zombyAgent.SetDestination(Player.transform.position); // ����� ����� ����
                }
            }
        }



        public void StartDead()
        {
            // ������ ��� ��������� Dead
            if (zombyAgent.enabled)
            {
                zombyAgent.isStopped = true; // ������������� ����� �����������
            }
            zombyAgent.speed = 0f;
            GetComponent<CapsuleCollider>().enabled = false;
            StartCoroutine(WaitClierBody());
            footDust.Stop();
            GetComponent<NavMeshAgent>().enabled = false;
        }

        public void UpdateDead()
        {
            // ������ ���������� ��� ��������� dead
        }

        public IEnumerator WaitClierBody()
        {
            // �������� 
            yield return new WaitForSeconds(bodyClearTime);
            // �������� ����� ��������
            Destroy(gameObject);
        }

        private void ExitState(ZombiStatus state)
        {
            switch (state)
            {
                case ZombiStatus.idle:
                    break;

                case ZombiStatus.run:
                    animator.SetBool("isRun", false);
                    break;

                case ZombiStatus.attack:
                    break;

                case ZombiStatus.injury:
                    break;

                case ZombiStatus.dead:
                    break;
            }
        }

        // ���� �����
        void DemageZombi(float demage, RaycastHit hit)
        {
            shootZombi = hit.collider.gameObject; // GameObject shootZombi = ����� � �������� ������;

            if (shootZombi == gameObject) // ���������, ��� ��������� ��������� � �������� �����
            {
                // ���������� ��������: �� ������� ����, ���� ����� ��� � �������� ��������.
                if (statusZombi == ZombiStatus.dead) return;

                lifeZombi -= demage;

                if (lifeZombi > 0)
                {
                    statusZombi = ZombiStatus.injury;
                    // ���������� ������� 'isHit' ��� ������� ��� ����������� ��������
                    animator.SetTrigger("isHit"); 
                }
                else
                {
                    statusZombi = ZombiStatus.dead;
                    animator.SetTrigger("isDead"); // �������� ������
                }
            }
        }

        // ���������� ���������
        private void UpdateState()
        {
            switch (statusZombi)
            {
                case ZombiStatus.idle:
                    UpdateIdle();
                    break;

                case ZombiStatus.run:
                    Debug.Log("���� � ���������: Run");
                    UpdateRun();
                    break;

                case ZombiStatus.attack:
                    UpdateAttack();
                    break;

                case ZombiStatus.injury:
                    // UpdateInjury() ��� ������, ����� �������� ����������
                    break;

                case ZombiStatus.dead:
                    UpdateDead();
                    break;
            }
        }

        private void EnterState(ZombiStatus state)
        {
            switch (state)
            {
                case ZombiStatus.idle:
                  //  Debug.Log("���� � ���������: Idle");
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


    
}
