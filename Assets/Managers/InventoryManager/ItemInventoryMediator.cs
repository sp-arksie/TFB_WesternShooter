using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static ItemInventoryMediator;

public class ItemInventoryMediator : MonoBehaviour
{
    public static ItemInventoryMediator Instance { get; private set; }

    public event Action<EquippedItem> onItemEquipped;
    public event Action<int> onItemUnequipped;
    public event Action onAmmoTrackerUpdated;

    public List<EquippedItem> equippedItems = new();

    public Dictionary<ProjectileType, ProjectileInfo> projectileTracker { get; private set; } = new();

    [SerializeField] GameObject compactAmmoPrefab;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }

        //=================

        //ProjectileInfo pi = new(compactAmmoPrefab, ProjectileType.Compact, WeaponCalliber.SmallCalliber);
        //pi.amount = 69;
        //projectileTracker.Add(ProjectileType.Compact, pi);
    }

    private void Start()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("ProjectilePrefabs");
        foreach(GameObject prefab in prefabs)
        {
            Projectile p = prefab.GetComponent<Projectile>();
            ProjectileInfo pi = new(prefab, p.projectileType, p.MatchingCalliber);
            pi.amount = 0;

            projectileTracker.Add(p.projectileType, pi);
        }
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
            this.cellID = cellID;
        }
    }

    public void NotifyItemEquipped(InventoryGridCell itemInfo)
    {
        string cellID = itemInfo.cellID;
        bool alreadyEquipped = false;

        foreach(EquippedItem equippedItem in equippedItems)
        {
            if(equippedItem.cellID == cellID)
            {
                alreadyEquipped = true;

                string itemName = itemInfo.InventoryItem.name;
                ToolTip.ShowToolTip($"{itemName} already equipped!");
                DOVirtual.DelayedCall(3f, () => ToolTip.HideToolTip());
            }
        }

        if(!alreadyEquipped)
        {
            EquippedItem newItem = new(itemInfo.InventoryItem.ItemPhysical, itemInfo.cellID);
            newItem.amount = itemInfo.CurrentItemAmount;

            equippedItems.Add(newItem);
            onItemEquipped.Invoke(newItem);
        }
    }

    public void NotifyItemUnequipped(InventoryGridCell itemInfo)
    {
        bool unequipped = false;
        for(int i = 0; i < equippedItems.Count && unequipped == false; i++)
        {
            if(itemInfo.cellID == equippedItems[i].cellID)
            {
                equippedItems.RemoveAt(i);
                onItemUnequipped.Invoke(i);
                unequipped = true;
            }
        }

        if (!unequipped)
        {
            string itemName = itemInfo.InventoryItem.name;
            ToolTip.ShowToolTip($"{itemName} is not currently equipped!");
            DOVirtual.DelayedCall(3f, () => ToolTip.HideToolTip());
        }
    }

    // TODO: call this funciton when an item is moved or added in inventory
    public void NotifyIdUpdate(string oldId, string newId)
    {
        for(int i = 0; i < equippedItems.Count; i++)
        {
            if (equippedItems[i].cellID == oldId)
            {
                EquippedItem updatedItem = new(equippedItems[i].prefabToInstantiate, newId);
                updatedItem.amount = equippedItems[i].amount;

                equippedItems.RemoveAt(i);
                equippedItems.Insert(i, updatedItem);
            }
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
        public List<Vector2Int> locationsInInventory = new();

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
        foreach(KeyValuePair<ProjectileType, ProjectileInfo> projectileType in projectileTracker)
        {
            projectileType.Value.amount = 0;
            projectileType.Value.locationsInInventory.Clear();
        }

        foreach (KeyValuePair<string, List<Vector2Int>> item in inventory.itemLocationTracker)
        {
            InventoryGridCell igc = inventory.inventoryContainer[item.Value[0]];
            Vector2Int location = item.Value[0];

            Projectile projectile = igc.InventoryItem.ItemPhysical.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectileTracker[projectile.projectileType].amount += igc.CurrentItemAmount;

                if(projectileTracker[projectile.projectileType].locationsInInventory.Count > 0)
                {
                    if(!projectileTracker[projectile.projectileType].locationsInInventory.Contains(location))
                        projectileTracker[projectile.projectileType].locationsInInventory.Add(location);
                }
                else
                {
                    projectileTracker[projectile.projectileType].locationsInInventory.Add(location);
                }
            }
        }

        onAmmoTrackerUpdated?.Invoke();
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
