using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionIsInEffectiveRange : DecisionNode
{
    protected override bool CheckCondition()
    {
        float distance = Vector3.Distance(
            entityController.transform.position,
            entityController.sight.GetCurrentlyVisiblesToEntity()[0].entityVisible.transform.position);

        ProjectileWeaponForEntity weapon = entityController.HotBarManager.currentSelectedItem as ProjectileWeaponForEntity;

        return distance < weapon.effectiveRange;
    }
}
