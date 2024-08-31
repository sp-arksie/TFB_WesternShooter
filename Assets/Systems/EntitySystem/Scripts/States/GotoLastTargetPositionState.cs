using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoLastTargetPositionState : BaseState
{
    [SerializeField] float destinationReachedThreshold = 1f;

    private void Update()
    {
        GetAgent().SetDestination(GetLastSeenTargetPosition());
        if(Vector3.Distance(transform.position, GetLastSeenTargetPosition()) < destinationReachedThreshold)
        {
            ForgetLastSeenTargetPosition();
        }
    }
}
