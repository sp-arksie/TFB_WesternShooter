using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AmmoBehaviour : MonoBehaviour
{
    [SerializeField] int magazineSize = 5;
    [SerializeField] float reloadTime = 1f;
    [SerializeField] float reloadStartDelay = 0f;
    [SerializeField] WeaponCalliber weaponCalliber;

    [SerializeField] ProjectileType currentProjectileSelected;
    public ProjectileType CurrentProjectileSelected { get => currentProjectileSelected; }
    
    List<ItemInventoryMediator.ProjectileInfo> availableProjectileTypes = new();

    int availableProjectileTypesIndex = 0;

    int ammoInMagazine;
    int reserveAmmo;

    bool shouldReload = false;
    bool reloadInProgress = false;
    Coroutine reloadCoroutine;

    ItemInventoryMediator inventoryMediator;

    public event Action ReloadComplete;

    #region DEBUG

    [SerializeField] TextMeshProUGUI debugAmmo;

    #endregion

    private void Awake()
    {
        inventoryMediator = ItemInventoryMediator.Instance;
    }

    private void Start()
    {
        ammoInMagazine = magazineSize;
        reserveAmmo = 100;
    }

    private void OnEnable()
    {
        //if(debugAmmo) debugAmmo.text = $"{ammoInMagazine}/{magazineSize}    Total: {inventoryMediator.GetCurrentAmmoTypeAmount(currentProjectileSelected)}";

        inventoryMediator.onAmmoTrackerUpdated += UpdateAvailableProjectileTypes;
    }

    private void OnDisable()
    {
        inventoryMediator.onAmmoTrackerUpdated -= UpdateAvailableProjectileTypes;
    }

    internal bool GetHasBulletInMagazine()
    {
        return ammoInMagazine > 0;
    }

    internal bool CanReload()
    {
        return (inventoryMediator.GetCurrentAmmoTypeAmount(CurrentProjectileSelected) > 0) && (ammoInMagazine < magazineSize);
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
            
            if (debugAmmo) debugAmmo.text = $"{ammoInMagazine}/{magazineSize}    Total: {inventoryMediator.GetCurrentAmmoTypeAmount(CurrentProjectileSelected)}";
            
            if ((inventoryMediator.GetCurrentAmmoTypeAmount(CurrentProjectileSelected) <= 0) ||
                (ammoInMagazine >= magazineSize))
                    shouldReload = false;
        }
        reloadInProgress = false;
        ReloadComplete.Invoke();
        yield return null;
    }

    private void AddAmmoToMagazine()
    {
        ammoInMagazine++;
        inventoryMediator.NotifyConsumeAmmo(CurrentProjectileSelected);
    }

    internal void ConsumeMagazineAmmo()
    {
        ammoInMagazine--;
        if(debugAmmo) debugAmmo.text = $"{ammoInMagazine}/{magazineSize}    Total: {inventoryMediator.GetCurrentAmmoTypeAmount(CurrentProjectileSelected)}";
    }

    internal void SwitchAmmoType(int increaseDecreaseAmount)
    {
        availableProjectileTypesIndex += increaseDecreaseAmount;

        if(availableProjectileTypesIndex < 0)
            availableProjectileTypesIndex = availableProjectileTypes.Count - 1;
        if(availableProjectileTypesIndex >= availableProjectileTypes.Count)
            availableProjectileTypesIndex = 0;

        currentProjectileSelected = availableProjectileTypes[availableProjectileTypesIndex].projectileType;
        ammoInMagazine = 0;
        StartReload();
    }

    private void UpdateAvailableProjectileTypes()
    {
        availableProjectileTypes = inventoryMediator.GetCompatibleProjectileTypes(weaponCalliber);

        bool updatedIndex = false;
        for(int i = 0; i < availableProjectileTypes.Count && updatedIndex == false; i++)
        {
            if (availableProjectileTypes[i].projectileType == CurrentProjectileSelected)
            {
                availableProjectileTypesIndex = i;
                updatedIndex = true;
            }
        }
    }

    internal float GetReloadStartDelay()
    {
        return reloadStartDelay;
    }
}
