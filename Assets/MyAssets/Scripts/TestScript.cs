using UnityEngine;
using UnityEngine.AI;

public class TestScript : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        Debug.Log(gameObject.name+" ---------------------------------------------");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (agent == null)
            Debug.LogError($"[{gameObject.name}] NavMeshAgent �� ������!");
        else
            Debug.Log($"[{gameObject.name}] NavMeshAgent ������. BaseOffset: {agent.baseOffset}, enabled: {agent.enabled}");

        if (animator == null)
            Debug.LogWarning($"[{gameObject.name}] Animator �� ������!");
        else
            Debug.Log($"[{gameObject.name}] Animator ������. ������� �����: {animator.GetCurrentAnimatorStateInfo(0).IsName(animator.GetCurrentAnimatorStateInfo(0).shortNameHash.ToString())}");
    }

    void Update()
    {
        if (agent != null)
        {
            Debug.Log($"[{gameObject.name}] isOnNavMesh: {agent.isOnNavMesh}, enabled: {agent.enabled}, isStopped: {agent.isStopped}, speed: {agent.speed}, pos: {transform.position}");
            if (!agent.isOnNavMesh)
                Debug.LogError($"[{gameObject.name}] ��������: NavMeshAgent �� �� �����! ������� �������: {transform.position}");
        }
    }
} 