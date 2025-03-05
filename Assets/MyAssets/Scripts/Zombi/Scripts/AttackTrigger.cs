using System.Collections.Generic;
using UnityEngine;

namespace FpsZomby
{
    public class AttackTrigger : MonoBehaviour
    {
        [SerializeField] private List<GameObject> zombieObjects = new List<GameObject>(); // Список объектов зомби
        private bool isInitialized = false; // Флаг инициализации

        void Awake()
        {
            InitializeComponents(); // Инициализация компонентов при пробуждении
        }

        void Start()
        {
            if (!isInitialized)
            {
                InitializeComponents(); // Повторная инициализация, если не была выполнена
            }
        }

        private void InitializeComponents()
        {
            try
            {
                Collider triggerCollider = GetComponent<Collider>(); // Получаем компонент Collider
                if (triggerCollider == null)
                {
                    return; // Выход, если Collider отсутствует
                }

                if (!triggerCollider.isTrigger)
                {
                    triggerCollider.isTrigger = true; // Устанавливаем Collider как триггер
                }

                Rigidbody rb = GetComponent<Rigidbody>(); // Получаем компонент Rigidbody
                if (rb == null)
                {
                    rb = gameObject.AddComponent<Rigidbody>(); // Добавляем Rigidbody, если отсутствует
                    rb.isKinematic = true; // Устанавливаем Rigidbody как кинематический
                    rb.useGravity = false; // Отключаем гравитацию
                }

                if (zombieObjects == null)
                {
                    zombieObjects = new List<GameObject>(); // Инициализация списка зомби, если он null
                }

                isInitialized = true; // Устанавливаем флаг инициализации
            }
            catch (System.Exception e)
            {
                Debug.Log("Ошибка AttackTrigger");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null) return; // Выход, если другой объект null

            try
            {
                if (other.CompareTag("Player"))
                {
                    ActivateZombies(); // Активация зомби при входе игрока в триггер
                    Destroy(gameObject); // Удаление объекта, на котором находится скрипт
                }
            }
            catch (System.Exception e)
            {
                // Обработка исключений
            }
        }

        private void ActivateZombies()
        {
            try
            {
                if (zombieObjects == null || zombieObjects.Count == 0)
                {
                    return; // Выход, если список зомби пуст
                }

                foreach (GameObject zombieObject in zombieObjects)
                {
                    if (zombieObject == null) continue; // Пропуск, если объект зомби null

                    Zombi zombieComponent = zombieObject.GetComponent<Zombi>(); // Получаем компонент Zombi
                    if (zombieComponent != null)
                    {
                        UnityEngine.AI.NavMeshAgent navMeshAgent = zombieObject.GetComponent<UnityEngine.AI.NavMeshAgent>(); // Получаем компонент NavMeshAgent
                        if (navMeshAgent != null && navMeshAgent.enabled)
                        {
                            zombieComponent.statusZombi = Zombi.ZombiStatus.run; // Устанавливаем статус зомби на "бег"
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                // Обработка исключений
            }
        }

        public void AddZombie(GameObject zombie)
        {
            if (zombie == null) return; // Выход, если зомби null

            if (zombieObjects == null)
            {
                zombieObjects = new List<GameObject>(); // Инициализация списка зомби, если он null
            }

            if (!zombieObjects.Contains(zombie))
            {
                zombieObjects.Add(zombie); // Добавление зомби в список, если его там нет
            }
        }
    }
}
