using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AmmoBehaviour : MonoBehaviour
{
    [SerializeField] protected int magazineSize = 5;
    [SerializeField] protected float reloadTime = 1f;
    [SerializeField] protected float reloadStartOrFinishDelay = 0f;
    [SerializeField] protected WeaponCalliber weaponCalliber;

    protected int ammoInMagazine;

    protected bool shouldReload = false;
    protected bool reloadInProgress = false;
    Coroutine reloadCoroutine;

    public event Action ReloadComplete;

    #region DEBUG

    [SerializeField] TextMeshProUGUI debugAmmo;

    #endregion

    private void Start()
    {
        ammoInMagazine = magazineSize;
    }

    internal bool GetHasBulletInMagazine()
    {
        return ammoInMagazine > 0;
    }

    internal bool GetCanReload()
    {
        return CanReload();
    }

    protected virtual bool CanReload() { throw new NotImplementedException(); }

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
        while (shouldReload)
        {
            yield return new WaitForSeconds(reloadTime);
            AddAmmoToMagazine();

            if (CanReload() == false)
                shouldReload = false;
        }
        reloadInProgress = false;
        ReloadComplete?.Invoke();
        yield return null;
    }

    protected virtual void AddAmmoToMagazine()
    {
        ammoInMagazine++;
    }

    internal void NotifyProjectileConsumed()
    {
        ConsumeAmmo();
    }

    private void ConsumeAmmo()
    {
        ammoInMagazine--;
    }

    internal float GetReloadStartOrFinishDelay()
    {
        return reloadStartOrFinishDelay;
    }
}
