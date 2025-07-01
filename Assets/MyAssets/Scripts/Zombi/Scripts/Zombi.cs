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

        public event Action ZombiInjury; // Событие для ранения зомби

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
        // Пробуем найти ближайшую точку на NavMesh и телепортировать туда
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            Debug.Log($"{gameObject.name}: Телепортирован на NavMesh!");
        }
        else
        {
            Debug.LogError($"{gameObject.name}: Не удалось найти NavMesh рядом!");
        }
    }
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
            if (zombyAgent != null)
            {
                if (!zombyAgent.enabled)
                    Debug.LogWarning($"{gameObject.name}: NavMeshAgent отключён!");
                else if (!zombyAgent.isOnNavMesh)
                    Debug.LogError($"{gameObject.name}: NavMeshAgent НЕ НА СЕТКЕ!");
            }
            zombiSpeedDebug = zombyAgent.speed;
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
            if (zombyAgent == null)
            {
                Debug.LogError($"{gameObject.name}: NavMeshAgent не найден!");
                return;
            }

            if (!zombyAgent.enabled)
            {
                Debug.LogWarning($"{gameObject.name}: NavMeshAgent отключён (enabled = false)!");
                return;
            }

            if (!zombyAgent.isOnNavMesh)
            {
                Debug.LogError($"{gameObject.name}: NavMeshAgent НЕ НА СЕТКЕ! (isOnNavMesh = false)");
                return;
            }

            // Если всё ок — запускаем движение
            Debug.Log($"{gameObject.name}: StartRun OK — агент на сетке, запускаем движение!");
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
            // ЗАЩИТА: Прежде чем давать команду, проверяем, что агент жив и на сетке.
            // Это устранит ошибку "SetDestination can only be called on an active agent".
            if (zombyAgent == null || !zombyAgent.enabled || !zombyAgent.isOnNavMesh)
            {
                return;
            }

            distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);
            
            
            if (statusZombi == ZombiStatus.run)
            {
                zombyAgent.SetDestination(Player.transform.position);
            }
        //Запуск атаки зомби
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
            // Анимация ранения теперь будет вызываться триггером,
            // но мы по-прежнему должны остановить зомби.
            if(zombyAgent.enabled)
            {
                zombyAgent.isStopped = true; // Используем isStopped для четкой остановки
            }
        }

        
        public void ZombyInjurnyEndAnimation()
        {
            // Кастыль: только если этот аниматор сейчас в состоянии ранения
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Reaction Hit"))
                return;

            if (lifeZombi > 0)
            {
                statusZombi = ZombiStatus.run;
                if (zombyAgent.enabled && zombyAgent.isOnNavMesh)
                {
                    zombyAgent.speed = 3; // Возвращаем скорость после ранения
                    zombyAgent.isStopped = false; // Снимаем стоп
                    if (Player != null)
                        zombyAgent.SetDestination(Player.transform.position); // Сразу задаём цель
                }
            }
        }



        public void StartDead()
        {
            // Логика для состояния Dead
            if (zombyAgent.enabled)
            {
                zombyAgent.isStopped = true; // Останавливаем перед отключением
            }
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
                // Исправлена проверка: не наносим урон, если зомби уже в процессе умирания.
                if (statusZombi == ZombiStatus.dead) return;

                lifeZombi -= demage;

                if (lifeZombi > 0)
                {
                    statusZombi = ZombiStatus.injury;
                    // Используем триггер 'isHit' для запуска или перезапуска анимации
                    animator.SetTrigger("isHit"); 
                }
                else
                {
                    statusZombi = ZombiStatus.dead;
                    animator.SetTrigger("isDead"); // Анимация смерти
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
                    Debug.Log("Вход в состояние: Run");
                    UpdateRun();
                    break;

                case ZombiStatus.attack:
                    UpdateAttack();
                    break;

                case ZombiStatus.injury:
                    // UpdateInjury() был удален, чтобы избежать конфликтов
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
