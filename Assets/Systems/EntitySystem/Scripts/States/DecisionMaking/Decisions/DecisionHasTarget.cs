using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionHasTarget : DecisionNode
{
    protected override bool CheckCondition()
    {
        return entityController.GetTarget() != null;
    }
}
