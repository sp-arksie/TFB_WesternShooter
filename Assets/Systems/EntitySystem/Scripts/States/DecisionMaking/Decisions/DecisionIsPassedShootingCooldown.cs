using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionIsPassedShootingCooldown : DecisionNode
{
    protected override bool CheckCondition()
    {
        float randomCooldown = Random.Range(entityController.ShootingCooldownMin, entityController.ShootingCooldownMax);

        return (Time.time - entityController.LastQuickActionTime) > randomCooldown;
    }
}
