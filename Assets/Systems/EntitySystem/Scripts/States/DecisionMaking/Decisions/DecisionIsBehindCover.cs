using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionIsBehindCover : DecisionNode
{
    protected override bool CheckCondition()
    {
        return Vector3.Distance(
            entityController.transform.position,
            entityController.CurrentCoverDestination) 
            < entityController.CoverDestinationReachedThreshold;
    }
}
