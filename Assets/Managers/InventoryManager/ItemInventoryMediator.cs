using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static ItemInventoryMediator;

public class ItemInventoryMediator : MonoBehaviour
{
    public static ItemInventoryMediator instance { get; private set; }

    public event Action<EquippedItem> onItemEquipped;
    public event Action<int> onItemUnequipped;
    public event Action onAmmoTrackerUpdated;

    public EquippedItem[] equippedItems { get; private set; } = new EquippedItem[8];

    public Dictionary<ProjectileType, ProjectileInfo> projectileTracker { get; private set; } = new();

    [SerializeField] GameObject compactAmmoPrefab;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }

        //=================

        ProjectileInfo pi = new(compactAmmoPrefab, ProjectileType.Compact, WeaponCalliber.SmallCalliber);
        pi.amount = 100;
        projectileTracker.Add(ProjectileType.Compact, pi);
    }

    #region Equippable Items
    public class EquippedItem
    {
        public GameObject prefabToInstantiate { get; private set; }
        public string cellID { get; private set; }
        public int amount;

        public EquippedItem(GameObject prefabToInstantiate, string cellID)
        {
            this.prefabToInstantiate = prefabToInstantiate;
        }
    }

    public void NotifyItemEquipped(InventoryGridCell itemInfo)
    {
        EquippedItem equippedItem = new(itemInfo.InventoryItem.ItemPhysical, itemInfo.cellID);
        equippedItem.amount = itemInfo.CurrentItemAmount;

        int firstEmpty = Array.FindIndex(equippedItems, i => i == null);
        equippedItems[firstEmpty] = equippedItem;
    }

    public void NotifyItemUnequipped(InventoryGridCell itemInfo)
    {
        bool unequipped = false;
        for(int i = 0; i < equippedItems.Length && unequipped == false; i++)
        {
            if(itemInfo.cellID == equippedItems[i].cellID)
            {
                equippedItems[i] = null;
                onItemUnequipped.Invoke(i);
                unequipped = true;
            }
        }
    }

    public void NotifyIdUpdate(string oldId, string newId)
    {
        int i = 0;
        foreach(EquippedItem ei in equippedItems)
        {
            if(ei.cellID == oldId)
            {
                EquippedItem updatedItem = new(ei.prefabToInstantiate, newId);
                updatedItem.amount = ei.amount;
                equippedItems[i] = updatedItem;
            }
            i++;
        }
    }

    public void NotifyItemUsed(int index)
    {
        equippedItems[index].amount--;
    }

    public int CheckItemAmountAtIndex(int index)
    {
        return equippedItems[index].amount;
    }
    #endregion

    #region Ammo
    public class ProjectileInfo
    {
        public GameObject prefabToInstatiate { get; private set; }
        public ProjectileType projectileType { get; private set; }
        public WeaponCalliber matchingCalliber { get; private set; }
        public int amount;

        public ProjectileInfo(GameObject prefabToInstantiate, ProjectileType projectileType, WeaponCalliber matchingCalliber)
        {
            this.prefabToInstatiate = prefabToInstantiate;
            this.projectileType = projectileType;
            this.matchingCalliber = matchingCalliber;
        }
    }

    public void NotifyConsumeAmmo(ProjectileType projectileType)
    {
        projectileTracker[projectileType].amount--;
    }

    public int GetCurrentAmmoTypeAmount(ProjectileType projectileType)
    {
        return projectileTracker[projectileType].amount;
    }

    public void UpdateAmmoTracker(Inventory inventory)
    {
        foreach(KeyValuePair<ProjectileType, ProjectileInfo> ammoType in projectileTracker)
        {
            ammoType.Value.amount = 0;
        }

        foreach (KeyValuePair<string, List<Vector2Int>> item in inventory.itemLocationTracker)
        {
            InventoryGridCell igc = inventory.inventoryContainer[item.Value[0]];

            Projectile projectile = igc.InventoryItem.ItemPhysical.GetComponent<Projectile>();
            if (projectile != null)
            {
                if (projectileTracker.ContainsKey(projectile.projectileType))
                {
                    projectileTracker[projectile.projectileType].amount += igc.CurrentItemAmount;
                }
                else
                {
                    ProjectileInfo newAmmoType = new(igc.InventoryItem.ItemPhysical, projectile.projectileType, projectile.MatchingCalliber);
                    newAmmoType.amount = igc.CurrentItemAmount;

                    projectileTracker.Add(projectile.projectileType, newAmmoType);
                }
            }
        }

        DictionaryExtensions.RemoveAll(projectileTracker, (k, v) => v.amount == 0);

        onAmmoTrackerUpdated.Invoke();
    }

    public List<ProjectileInfo> GetCompatibleProjectileTypes(WeaponCalliber calliber)
    {
        List<ProjectileInfo> results = new();

        foreach(KeyValuePair<ProjectileType, ProjectileInfo> projectileType in projectileTracker)
        {
            if(projectileType.Value.matchingCalliber == calliber)
            {
                results.Add(projectileType.Value);
            }
        }

        return results;
    }

    public ProjectileInfo GetProjectileInfo(ProjectileType projectileType)
    {
        return projectileTracker[projectileType];
    }
    #endregion
}
