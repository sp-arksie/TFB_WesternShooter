using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBehaviourForPlayer : AmmoBehaviour
{
    [SerializeField] ProjectileType currentProjectileSelected;
    public ProjectileType CurrentProjectileSelected { get => currentProjectileSelected; }

    List<ItemInventoryMediator.ProjectileInfo> availableProjectileTypes = new();
    int availableProjectileTypesIndex = 0;

    ItemInventoryMediator inventoryMediator;

    private void Awake()
    {
        inventoryMediator = ItemInventoryMediator.Instance;
    }

    private void OnEnable()
    {
        inventoryMediator.onAmmoTrackerUpdated += UpdateAvailableProjectileTypes;
    }

    private void OnDisable()
    {
        inventoryMediator.onAmmoTrackerUpdated -= UpdateAvailableProjectileTypes;
    }

    protected override bool CanReload()
    {
        return (inventoryMediator.GetCurrentAmmoTypeAmount(CurrentProjectileSelected) > 0) && (ammoInMagazine < magazineSize);
    }

    protected override void AddAmmoToMagazine()
    {
        base.AddAmmoToMagazine();
        inventoryMediator.NotifyConsumeProjectile(CurrentProjectileSelected);
    }

    internal void SwitchAmmoType(int increaseDecreaseAmount)
    {
        availableProjectileTypesIndex += increaseDecreaseAmount;

        if (availableProjectileTypesIndex < 0)
            availableProjectileTypesIndex = availableProjectileTypes.Count - 1;
        if (availableProjectileTypesIndex >= availableProjectileTypes.Count)
            availableProjectileTypesIndex = 0;

        currentProjectileSelected = availableProjectileTypes[availableProjectileTypesIndex].projectileType;
        ammoInMagazine = 0;
        StartReload();
    }

    private void UpdateAvailableProjectileTypes()
    {
        availableProjectileTypes = inventoryMediator.GetCompatibleProjectileTypes(weaponCalliber);

        bool updatedIndex = false;
        for (int i = 0; i < availableProjectileTypes.Count && updatedIndex == false; i++)
        {
            if (availableProjectileTypes[i].projectileType == CurrentProjectileSelected)
            {
                availableProjectileTypesIndex = i;
                updatedIndex = true;
            }
        }
    }
}
