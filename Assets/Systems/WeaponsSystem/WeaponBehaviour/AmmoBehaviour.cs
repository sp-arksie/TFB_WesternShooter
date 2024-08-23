using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBehaviour : MonoBehaviour
{
    [SerializeField] int magazineSize = 5;
    [SerializeField] float reloadTime = 1f;

    int ammoInMagazine;
    int reserveAmmo;

    bool shouldReload = false;
    bool reloadInProgress = false;
    Coroutine reloadCoroutine;

    private void Start()
    {
        ammoInMagazine = 100;
        reserveAmmo = 100;
    }

    internal bool GetHasBulletInMagazine()
    {
        return ammoInMagazine > 0;
    }

    internal bool CanReload()
    {
        return (reserveAmmo > 0) && (ammoInMagazine < magazineSize);
    }

    internal void StartReload()
    {
        if (!reloadInProgress)
        {
            shouldReload = true;
            reloadCoroutine = StartCoroutine(PerformReloads());
        }
    }

    internal void CancelReload()
    {
        shouldReload = false;
    }

    private IEnumerator PerformReloads()
    {
        reloadInProgress = true;
        while(shouldReload)
        {
            yield return new WaitForSeconds(reloadTime);
            AddAmmoToMagazine();
        }
        reloadInProgress = false;
        yield return null;
    }

    private void AddAmmoToMagazine()
    {
        ammoInMagazine++;
        reserveAmmo--;
    }
}
