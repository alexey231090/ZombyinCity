using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using System;
namespace FpsZomby {
    public class Zombi : MonoBehaviour
    {
        [Inject]
        PlayerSwitchingStates Player;

        public event Action ZombiInjury; // Событие для ранения зомби

        public enum ZombiStatus { idle, run, attack, injury, dead }
        public ZombiStatus statusZombi;
        private ZombiStatus previousStatus;

        private static List<GameObject> activeZombies = new List<GameObject>();
        private bool isRunning = false;

        Animator animator;
        NavMeshAgent zombyAgent;

        GameObject shootZombi;

        [SerializeField] float lifeZombi = 100;

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
        }

        private void OnEnable()
        {
            PlayerSwitchingStates.PlayerFire += DemageZombi;
            StateZombiHit.ZombiInjury += ZombyInjurnyEndAnimation; // Подписка на событие ранения
            StateZombiAttack.ZombiAttack += OnZombiAttack;
        }

        private void OnDisable()
        {
            PlayerSwitchingStates.PlayerFire -= DemageZombi;
            StateZombiHit.ZombiInjury -= ZombyInjurnyEndAnimation; // Отписка от события ранения
            StateZombiAttack.ZombiAttack -= OnZombiAttack;
        }

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
            // Если состояние зомби изменилось
            if (statusZombi != previousStatus)
            {
                ExitState(previousStatus); // Выход из предыдущего состояния
                EnterState(statusZombi);   // Вход в новое состояние
                previousStatus = statusZombi;
            }
        }

        private void StartIdle()
        {
            // Логика для состояния idle
            zombyAgent.speed = 0f;
        }

        private void UpdateIdle()
        {
            // Логика обновления для состояния idle
        }

        private void StartRun()
        {
            // Убедитесь, что зомби начинает бежать, даже если он не в списке activeZombies
            zombyAgent.speed = 3;
            animator.SetBool("isRun", true);
            animator.SetBool("isAttack", false);

            isRunning = animator.GetBool("isRun");

            footDust.Play();
        }

        private void UpdateRun()
        {
            distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);
            
            
            if (statusZombi == ZombiStatus.run)
            {
                zombyAgent.SetDestination(Player.transform.position);
            }

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

        private void StartInjury()
        {
            // Проверка анимации ранения
            animator.SetBool("isHit", true);
            zombyAgent.speed = 0.1f; // Замедляем зомби           
        }

        
        private void UpdateInjury()
        {
            if (!IsInvoking(nameof(InjuryToRun)))
            {
                Invoke(nameof(InjuryToRun), 3f);
            }
        }

        private void InjuryToRun()
        {
            if (statusZombi == ZombiStatus.injury)
            {
                statusZombi = ZombiStatus.run;
                print("Start!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }
        }
        

        public void ZombyInjurnyEndAnimation()
        {
            animator.SetBool("isHit", false);            
            zombyAgent.speed = 3;

            if (activeZombies.Contains(this.gameObject)) // Проверка, находится ли зомби в списке activeZombies
            {
                if (lifeZombi > 0)
                {
                    statusZombi = ZombiStatus.run;
                }
                
                 
            }


        }



        public void StartDead()
        {
            // Логика для состояния Dead
            zombyAgent.speed = 0f;
            GetComponent<CapsuleCollider>().enabled = false;
            StartCoroutine(WaitClierBody());
            footDust.Stop();
            GetComponent<NavMeshAgent>().enabled = false;
        }

        public void UpdateDead()
        {
            // Логика обновления для состояния dead
        }

        public IEnumerator WaitClierBody()
        {
            // Ожидание 
            yield return new WaitForSeconds(bodyClearTime);
            // Удаление после ожидания
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

        // Урон зомби
        void DemageZombi(float demage, RaycastHit hit)
        {
            shootZombi = hit.collider.gameObject; // GameObject shootZombi = зомби в которого попали;

            if (shootZombi == gameObject) // Проверяем, что попадание произошло в текущего зомби
            {
                Debug.Log($"Зомби {gameObject.name} получил урон: {demage}");

                if (!activeZombies.Contains(gameObject))
                {
                    activeZombies.Add(gameObject);
                }

                lifeZombi -= demage;
                

                if (lifeZombi > 0)
                {
                    statusZombi = ZombiStatus.injury;
                }
                else
                {
                    animator.SetTrigger("isDead"); // Анимация смерти
                    statusZombi = ZombiStatus.dead;
                }
            }
            
        }

        // Обновление состояния
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

        private void EnterState(ZombiStatus state)
        {
            switch (state)
            {
                case ZombiStatus.idle:
                  //  Debug.Log("Вход в состояние: Idle");
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


    
}
