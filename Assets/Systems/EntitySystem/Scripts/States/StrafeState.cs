using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeState : BaseState
{
    private void Update()
    {
        NotifyOrientEntityToTarget();

        if (GetStrafeValue() != Vector3.zero)
        {
            float distance = Vector3.Distance(transform.position, GetStrafeDestination());

            if (distance < GetStrafeDestinationReachedThreshold() ||
                (Time.time - GetTimeOfLastStrafeSet()) > GetMaxTimeToReachStrafeDestination())
            {
                SetStrafeValue();
                GetAgent().SetDestination( GetStrafeDestination() );
            }
        }
        else
        {
            SetStrafeValue();
            GetAgent().SetDestination(GetStrafeDestination());
        }
    }

    private Vector3 GetStrafeDestination()
    {
        Vector3 currentPosition = GetAgent().transform.position;
        Vector3 globalStrafeValue = GetAgent().transform.TransformDirection(GetStrafeValue());

        return new(
            currentPosition.x + globalStrafeValue.x,
            currentPosition.y + globalStrafeValue.y,
            currentPosition.z + globalStrafeValue.z);
    }
}
