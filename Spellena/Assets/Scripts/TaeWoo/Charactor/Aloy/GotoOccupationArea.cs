using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;

public class GotoOccupationArea : Node
{
    private Transform playerTransform;
    private NavMeshAgent agent;

    private Transform occupationPoint;
    private Animator animator;

    public GotoOccupationArea() { }

    public GotoOccupationArea(Transform _playerTransform, Transform _occupationPoint)
    {
        playerTransform = _playerTransform;

        agent = playerTransform.GetComponent<NavMeshAgent>();
        if (agent == null) Debug.LogError("NavMeshAgent�� �Ҵ���� �ʾҽ��ϴ�");
        animator = playerTransform.GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator�� �Ҵ���� �ʾҽ��ϴ�");

        occupationPoint = _occupationPoint;
    }

    public override NodeState Evaluate()
    {
        agent.isStopped = false;
        agent.destination = occupationPoint.position;
        animator.SetBool("Move", true);

        Debug.Log("GotoOccupationArea..");

        state = NodeState.Running;
        return state;
    }
}
