using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionHasLineOfSight : DecisionNode
{
    protected override bool CheckCondition()
    {
        return entityController.sight.NotifyCheckLineOfSight(entityController.GetTarget().entityVisible);
    }
}
