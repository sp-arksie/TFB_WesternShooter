using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionIsInMeleeRange : DecisionNode
{
    protected override bool CheckCondition()
    {
        float distance = Vector3.Distance(
            entityController.transform.position, 
            entityController.sight.GetCurrentlyVisiblesToEntity()[0].transform.position);
        float minMeleeRange = entityController.GetMinimumMeleeRange();

        return distance < minMeleeRange;
    }
}
