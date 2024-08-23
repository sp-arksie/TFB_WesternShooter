using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBehaviour : MonoBehaviour
{
    [SerializeField] int magazineSize = 5;
    [SerializeField] float reloadTime = 1f;

    int ammoInMagazine;
    int reserveAmmo;

    public bool GetHasBulletInMagazine()
    {
        return ammoInMagazine > 0;
    }

    public bool CanReload()
    {
        return (reserveAmmo > 0) && (ammoInMagazine < magazineSize);
    }
}
