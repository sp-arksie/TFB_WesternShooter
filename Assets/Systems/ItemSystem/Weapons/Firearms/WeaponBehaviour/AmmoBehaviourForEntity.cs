using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBehaviourForEntity : AmmoBehaviour
{
    [SerializeField] GameObject projectilePrefabToShoot;

    int reserveAmmo = 1000;

    protected override bool CanReload()
    {
        return (reserveAmmo > 0) && (ammoInMagazine < magazineSize);
    }

    internal GameObject GetProjectilePrefabToShoot() { return projectilePrefabToShoot; }
}
