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

        [SerializeField] private TriggerType triggerType; // ��� ��������
        [SerializeField] private int triggerIndex; // ������ ��������
        [SerializeField] private List<GameObject> zombieObjects = new List<GameObject>(); // ������ �������� �����
        private bool isInitialized = false; // ���� �������������

        void Awake()
        {
            InitializeComponents(); // ������������� ����������� ��� �����������
        }

        void Start()
        {
            if (!isInitialized)
            {
                InitializeComponents(); // ��������� �������������, ���� �� ���� ���������
            }
        }

        // ������������� �����������
        private void InitializeComponents()
        {
            try
            {
                Collider triggerCollider = GetComponent<Collider>(); // �������� ��������� Collider
                if (triggerCollider == null)
                {
                    return; // �����, ���� Collider �����������
                }

                if (!triggerCollider.isTrigger)
                {
                    triggerCollider.isTrigger = true; // ������������� Collider ��� �������
                }

                Rigidbody rb = GetComponent<Rigidbody>(); // �������� ��������� Rigidbody
                if (rb == null)
                {
                    rb = gameObject.AddComponent<Rigidbody>(); // ��������� Rigidbody, ���� �����������
                    rb.isKinematic = true; // ������������� Rigidbody ��� ��������������
                    rb.useGravity = false; // ��������� ����������
                }

                if (zombieObjects == null)
                {
                    zombieObjects = new List<GameObject>(); // ������������� ������ �����, ���� �� null
                }

                if (triggerType == TriggerType.Zombi)
                {
                    // ��������� ��� ������� � ����� "Zombi" � ������, ���� ��� ��������� � ��������
                    Collider[] colliders = Physics.OverlapBox(triggerCollider.bounds.center, triggerCollider.bounds.extents, triggerCollider.transform.rotation);
                    foreach (Collider col in colliders)
                    {
                        if (col.gameObject.layer == LayerMask.NameToLayer("Zombi") && !zombieObjects.Contains(col.gameObject))
                        {
                            zombieObjects.Add(col.gameObject);
                        }
                    }
                }

                isInitialized = true; // ������������� ���� �������������
            }
            catch (System.Exception e)
            {
                Debug.Log("������ AttackTrigger: " + e.Message);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null) return; // �����, ���� ������ ������ null

            try
            {
                if (triggerType == TriggerType.Player && other.CompareTag("Player"))
                {
                    ActivateZombies(); // ��������� ����� ��� ����� ������ � �������
                    Destroy(gameObject); // �������� �������, �� ������� ��������� ������
                }
                else if (triggerType == TriggerType.Zombi && other.CompareTag("Player"))
                {
                    Destroy(gameObject); // �������� �������, �� ������� ��������� ������
                }
                else if (triggerType == TriggerType.OldTrigger && other.CompareTag("Player"))
                {
                    ActivateOldZombies(); // ��������� ����� �� ������� ������
                    Destroy(gameObject); // �������� �������, �� ������� ��������� ������
                }
            }
            catch (System.Exception e)
            {
                // ��������� ����������
            }
        }

        private void ActivateZombies()
        {
            try
            {
                // ����� ��� ������� � ����������� AttackTrigger
                AttackTrigger[] allTriggers = FindObjectsOfType<AttackTrigger>();
                foreach (AttackTrigger trigger in allTriggers)
                {
                    // ����� ������� � ����� Zombi � ����� �� ��������
                    if (trigger.triggerType == TriggerType.Zombi && trigger.triggerIndex == triggerIndex)
                    {
                        // ������������ ����� �� ������ ����� ��������
                        foreach (GameObject zombieObject in trigger.zombieObjects)
                        {
                            if (zombieObject == null) continue; // �������, ���� ������ ����� null

                            Zombi zombieComponent = zombieObject.GetComponent<Zombi>(); // �������� ��������� Zombi
                            if (zombieComponent != null)
                            {
                                UnityEngine.AI.NavMeshAgent navMeshAgent = zombieObject.GetComponent<UnityEngine.AI.NavMeshAgent>(); // �������� ��������� NavMeshAgent
                                if (navMeshAgent != null && navMeshAgent.enabled)
                                {
                                    zombieComponent.statusZombi = Zombi.ZombiStatus.run; // ������������� ������ ����� �� "���"
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                // ��������� ����������
            }
        }

        private void ActivateOldZombies()
        {
            try
            {
                if (zombieObjects == null || zombieObjects.Count == 0)
                {
                    return; // �����, ���� ������ ����� ����
                }

                foreach (GameObject zombieObject in zombieObjects)
                {
                    if (zombieObject == null) continue; // �������, ���� ������ ����� null

                    Zombi zombieComponent = zombieObject.GetComponent<Zombi>(); // �������� ��������� Zombi
                    if (zombieComponent != null)
                    {
                        UnityEngine.AI.NavMeshAgent navMeshAgent = zombieObject.GetComponent<UnityEngine.AI.NavMeshAgent>(); // �������� ��������� NavMeshAgent
                        if (navMeshAgent != null && navMeshAgent.enabled)
                        {
                            zombieComponent.statusZombi = Zombi.ZombiStatus.run; // ������������� ������ ����� �� "���"
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                // ��������� ����������
            }
        }

        public void AddZombie(GameObject zombie)
        {
            if (zombie == null) return; // �����, ���� ����� null

            if (zombieObjects == null)
            {
                zombieObjects = new List<GameObject>(); // ������������� ������ �����, ���� �� null
            }

            if (!zombieObjects.Contains(zombie))
            {
                zombieObjects.Add(zombie); // ���������� ����� � ������, ���� ��� ��� ���
            }
        }
    }
}
