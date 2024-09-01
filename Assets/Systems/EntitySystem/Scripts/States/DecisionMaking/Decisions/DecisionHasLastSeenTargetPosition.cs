using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionHasLastSeenTargetPosition : DecisionNode
{
    protected override bool CheckCondition()
    {
        return entityController.GetHasLastSeenTargetPosition();
    }
}
