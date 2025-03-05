using System.Collections.Generic;
using UnityEngine;

namespace FpsZomby
{
    public class AttackTrigger : MonoBehaviour
    {
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

                isInitialized = true; // ������������� ���� �������������
            }
            catch (System.Exception e)
            {
                Debug.Log("������ AttackTrigger");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null) return; // �����, ���� ������ ������ null

            try
            {
                if (other.CompareTag("Player"))
                {
                    ActivateZombies(); // ��������� ����� ��� ����� ������ � �������
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
