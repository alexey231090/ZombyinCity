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
            Debug.LogError($"[{gameObject.name}] NavMeshAgent НЕ найден!");
        else
            Debug.Log($"[{gameObject.name}] NavMeshAgent найден. BaseOffset: {agent.baseOffset}, enabled: {agent.enabled}");

        if (animator == null)
            Debug.LogWarning($"[{gameObject.name}] Animator не найден!");
        else
            Debug.Log($"[{gameObject.name}] Animator найден. Текущий стейт: {animator.GetCurrentAnimatorStateInfo(0).IsName(animator.GetCurrentAnimatorStateInfo(0).shortNameHash.ToString())}");
    }

    void Update()
    {
        if (agent != null)
        {
            Debug.Log($"[{gameObject.name}] isOnNavMesh: {agent.isOnNavMesh}, enabled: {agent.enabled}, isStopped: {agent.isStopped}, speed: {agent.speed}, pos: {transform.position}");
            if (!agent.isOnNavMesh)
                Debug.LogError($"[{gameObject.name}] ВНИМАНИЕ: NavMeshAgent НЕ НА СЕТКЕ! Текущая позиция: {transform.position}");
        }
    }
} 