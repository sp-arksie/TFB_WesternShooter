using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoBehaviour : MonoBehaviour
{
    [SerializeField] int magazineSize = 5;
    [SerializeField] float reloadTime = 1f;
    [SerializeField] float reloadStartDelay = 0f;

    int ammoInMagazine;
    int reserveAmmo;

    bool shouldReload = false;
    bool reloadInProgress = false;
    Coroutine reloadCoroutine;

    public event Action ReloadComplete;

    #region DEBUG

    [SerializeField] TextMeshProUGUI debugAmmo;

    #endregion

    private void Start()
    {
        ammoInMagazine = 2;
        reserveAmmo = 100;
    }

    private void OnEnable()
    {
        if(debugAmmo) debugAmmo.text = $"{ammoInMagazine}/{magazineSize}    Total: {reserveAmmo}";
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
            if (debugAmmo) debugAmmo.text = $"{ammoInMagazine}/{magazineSize}    Total: {reserveAmmo}";
            if ((reserveAmmo <= 0) || (ammoInMagazine >= magazineSize)) shouldReload = false;
        }
        reloadInProgress = false;
        ReloadComplete.Invoke();
        yield return null;
    }

    private void AddAmmoToMagazine()
    {
        ammoInMagazine++;
        reserveAmmo--;
    }

    internal void ConsumeAmmo() { ammoInMagazine--; if(debugAmmo) debugAmmo.text = $"{ammoInMagazine}/{magazineSize}    Total: {reserveAmmo}"; }

    internal float GetReloadStartDelay()
    {
        return reloadStartDelay;
    }
}
