using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootState : BaseState
{
    ProjectileWeaponForEntity weapon;
    Vector3 targetPosition = Vector3.zero;

    private void OnEnable()
    {
        GetAgent().SetDestination(transform.position);
        weapon = GetHotBarManager().currentSelectedItem as ProjectileWeaponForEntity;
    }

    private void Update()
    {
        targetPosition = GetSight().GetCurrentlyVisiblesToEntity()[0].entityVisible.transform.position;

        NotifyOrientEntityToTarget();
        
        OrientGunToTarget();

        GetHotBarManager().DoQuickAction();
    }

    private void OrientGunToTarget()
    {
        Transform weaponTransform = weapon.transform;
        Vector3 dir = targetPosition - weaponTransform.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        weaponTransform.rotation = rot;
    }
}
