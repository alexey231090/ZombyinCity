using System.Collections.Generic;
using UnityEngine;

namespace FpsZomby
{
    public class AttackTrigger : MonoBehaviour
    {
        public enum TriggerType
        {
            Zombi,
            Player,
            OldTrigger
        }

        [SerializeField] private TriggerType triggerType; // Тип триггера
        [SerializeField] private int triggerIndex; // Индекс триггера
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

        // Инициализация компонентов
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

                if (triggerType == TriggerType.Zombi)
                {
                    // Добавляем все объекты с тегом "Zombi" в список, если они находятся в триггере
                    Collider[] colliders = Physics.OverlapBox(triggerCollider.bounds.center, triggerCollider.bounds.extents, triggerCollider.transform.rotation);
                    foreach (Collider col in colliders)
                    {
                        if (col.gameObject.layer == LayerMask.NameToLayer("Zombi") && !zombieObjects.Contains(col.gameObject))
                        {
                            zombieObjects.Add(col.gameObject);
                        }
                    }
                }

                isInitialized = true; // Устанавливаем флаг инициализации
            }
            catch (System.Exception e)
            {
                Debug.Log("Ошибка AttackTrigger: " + e.Message);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null) return; // Выход, если другой объект null

            try
            {
                if (triggerType == TriggerType.Player && other.CompareTag("Player"))
                {
                    ActivateZombies(); // Активация зомби при входе игрока в триггер
                    Destroy(gameObject); // Удаление объекта, на котором находится скрипт
                }
                else if (triggerType == TriggerType.Zombi && other.CompareTag("Player"))
                {
                    Destroy(gameObject); // Удаление объекта, на котором находится скрипт
                }
                else if (triggerType == TriggerType.OldTrigger && other.CompareTag("Player"))
                {
                    ActivateOldZombies(); // Активация зомби по старому методу
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
                // Найти все объекты с компонентом AttackTrigger
                AttackTrigger[] allTriggers = FindObjectsOfType<AttackTrigger>();
                foreach (AttackTrigger trigger in allTriggers)
                {
                    // Найти триггер с типом Zombi и таким же индексом
                    if (trigger.triggerType == TriggerType.Zombi && trigger.triggerIndex == triggerIndex)
                    {
                        // Активировать зомби из списка этого триггера
                        foreach (GameObject zombieObject in trigger.zombieObjects)
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
                }
            }
            catch (System.Exception e)
            {
                // Обработка исключений
            }
        }

        private void ActivateOldZombies()
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
