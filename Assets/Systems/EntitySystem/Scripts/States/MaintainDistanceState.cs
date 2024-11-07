using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainDistanceState : BaseState
{
    private void OnEnable()
    {
        StartRun();
    }

    private void Update()
    {
        Vector3 direction = (transform.position - GetSight().GetCurrentlyVisiblesToEntity()[0].entityVisible.transform.position).normalized;

        float currentDistance = Vector3.Distance(GetSight().GetCurrentlyVisiblesToEntity()[0].entityVisible.transform.position, transform.position);
        float distanceToMove = GetMinimumDistanceFromTarget() - currentDistance;

        GetAgent().SetDestination(transform.position + direction * distanceToMove);

        
        NotifyOrientEntityToTarget();
    }
}
