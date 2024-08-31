using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : BaseState
{
    [SerializeField] Transform patrolPointsParent;
    [SerializeField] int startingPatrolPointIndex = 0;
    [SerializeField] float destinationReachedThreshold = 1f;

    Transform[] patrolPoints;
    int currentPatrolPointIndex;

    protected override void Awake()
    {
        base.Awake();
        patrolPoints = new Transform[patrolPointsParent.childCount];
        for (int i = 0; i < patrolPoints.Length; i++)
            patrolPoints[i] = patrolPointsParent.GetChild(i).transform;
    }

    private void Start()
    {
        currentPatrolPointIndex = startingPatrolPointIndex;
    }

    private void OnEnable()
    {
        
    }

    private void Update()
    {
        if (GetAgent() != null)
        {
            Vector3 destination = patrolPoints[currentPatrolPointIndex].position;
            GetAgent().SetDestination(destination);
            if (Vector3.Distance(transform.position, destination) < destinationReachedThreshold)
            {
                currentPatrolPointIndex++;
                if (currentPatrolPointIndex > patrolPoints.Length - 1) currentPatrolPointIndex = 0;
            }
        }
    }

    private void OnDisable()
    {
        
    }
}
