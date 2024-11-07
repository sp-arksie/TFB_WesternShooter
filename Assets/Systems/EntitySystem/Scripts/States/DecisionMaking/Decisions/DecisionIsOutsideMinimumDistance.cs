using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionIsOutsideMinimumDistance : DecisionNode
{
    protected override bool CheckCondition()
    {
        float distance = Vector3.Distance(
            entityController.transform.position,
            entityController.sight.GetCurrentlyVisiblesToEntity()[0].entityVisible.transform.position);

        return distance > entityController.MinimumDistanceFromTarget;
    }
}
