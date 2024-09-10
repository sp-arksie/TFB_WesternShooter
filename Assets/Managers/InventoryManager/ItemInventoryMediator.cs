using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemInventoryMediator;

public class ItemInventoryMediator : MonoBehaviour
{
    public static ItemInventoryMediator instance { get; private set; }

    public static event Action<EquippedItem> onItemEquipped;

    public static Dictionary<string, EquippedItem> equippedItemsTracker { get; private set; } = new();
    public static Dictionary<Projectile.ProjectileType, List<AmmoInfo>> ammoTracker { get; private set; } = new();

    [SerializeField] GameObject compactAmmoPrefab;

    private void Awake()
    {
        instance = this;

        ammoTracker.Add(Projectile.ProjectileType.Compact, new List<AmmoInfo>() { new AmmoInfo("123456", compactAmmoPrefab, Projectile.ProjectileType.Compact, WeaponCalliber.SmallCalliber) });
    }

    #region Equippable Items
    public class EquippedItem
    {
        public GameObject prefabToInstantiate { get; set; }
        public string instanceID { get; private set; }
        public int amount;

        public EquippedItem(string id, GameObject prefabToInstantiate)
        {
            instanceID = id;
            this.prefabToInstantiate = prefabToInstantiate;
        }
    }

    public static void NotifyItemEquipped(InventoryGridCell itemInfo)
    {
        EquippedItem equippedItem = new(itemInfo.cellID, itemInfo.InventoryItem.ItemPhysical);
        equippedItem.amount = itemInfo.CurrentItemAmount;

        equippedItemsTracker.Add(equippedItem.instanceID, equippedItem);

        onItemEquipped.Invoke(equippedItem);
    }

    public void DebugNotifyItemEquipped(string id, GameObject prefab, int amount)
    {
        EquippedItem ei = new(id, prefab);
        ei.amount = amount;
        equippedItemsTracker.Add(id, ei);
        onItemEquipped.Invoke(ei);
    }

    public static void NotifyItemUsed(string instanceID)
    {
        equippedItemsTracker[instanceID].amount -= 1;
    }

    public static int QueryItemAmount(string instanceID)
    {
        return equippedItemsTracker[instanceID].amount;
    }
    #endregion

    #region Ammo
    public class AmmoInfo
    {
        public GameObject prefabToInstatiate { get; private set; }
        public Projectile.ProjectileType projectileType { get; private set; }
        public WeaponCalliber matchingCalliber { get; private set; }
        public string instanceID { get; private set; }
        public int amount;

        public AmmoInfo(string id, GameObject prefabToInstantiate, Projectile.ProjectileType projectileType, WeaponCalliber matchingCalliber)
        {
            this.instanceID = id;
            this.prefabToInstatiate = prefabToInstantiate;
            this.projectileType = projectileType;
            this.matchingCalliber = matchingCalliber;
        }
    }

    public static void NotifyAmmoChange(Projectile.ProjectileType bulletType, int amount)
    {
        bool processFinished = false;
        int amountToSubtract = amount;

        for (int i = 0; i < ammoTracker[bulletType].Count && processFinished == false; i++)
        {
            if(ammoTracker[bulletType][i].amount - amountToSubtract > 0)
            {
                ammoTracker[bulletType][i].amount -= amountToSubtract;
                processFinished = true;
            }
            else
            {
                amountToSubtract -= ammoTracker[bulletType][i].amount;
                ammoTracker[bulletType][i].amount -= ammoTracker[bulletType][i].amount;
            }
        }
    }

    public AmmoInfo GetAmmoInfo(Projectile.ProjectileType bulletType)
    {
        return ammoTracker[bulletType][0];
    }

    public static int QueryCurrentAmmoTypeAmount(Projectile.ProjectileType bulletType)
    {
        int amount = 0;

        for (int i = 0; i < ammoTracker[bulletType].Count; i++)
        {
            amount += ammoTracker[bulletType][i].amount;
        }

        return amount;
    }

    public static List<AmmoInfo> GetCompatibleAmmo(WeaponCalliber calliber)
    {
        List<AmmoInfo> results = new();

        foreach (KeyValuePair<Projectile.ProjectileType, List<AmmoInfo>> ammo in ammoTracker)
        {
            if (ammo.Value[0].matchingCalliber == calliber)
            {
                results.AddRange(ammo.Value);
            }
        }
        return results;
    }

    public static void UpdateAmmoTracker(Dictionary<Vector2Int, InventoryGridCell> container, Dictionary<string, List<Vector2Int>> itemTracker)
    {
        foreach (KeyValuePair<string, List<Vector2Int>> item in itemTracker)
        {
            InventoryGridCell igc = container[item.Value[0]];
            Projectile bullet = igc.InventoryItem.ItemPhysical.GetComponent<Projectile>();
            if (bullet != null)
            {
                // Completely new AmmoType has been added in inventory
                if (!ammoTracker.ContainsKey(bullet.GetBulletType()))
                {
                    AmmoInfo ammoInfo = new(item.Key, igc.InventoryItem.ItemPhysical, bullet.projectileType, bullet.matchingCalliber);
                    ammoInfo.amount = igc.CurrentItemAmount;
                    List<AmmoInfo> infoList = new() { ammoInfo };
                    ammoTracker[bullet.GetBulletType()] = infoList;
                }
                else
                {
                    foreach (AmmoInfo ai in ammoTracker[bullet.GetBulletType()])
                    {
                        // Update existing AmmoType that matches with the one in Inventory
                        if (ai.instanceID == item.Key)
                        {
                            ai.amount = igc.CurrentItemAmount;
                        }
                        // New set of existing AmmoType has been added in Inventory
                        else
                        {
                            AmmoInfo ammoInfo = new(item.Key, igc.InventoryItem.ItemPhysical, bullet.projectileType, bullet.matchingCalliber);
                            ammoInfo.amount = igc.CurrentItemAmount;
                            ammoTracker[bullet.GetBulletType()].Add(ammoInfo);
                        }

                        // Clean entries that have no ammo
                        if(ai.amount == 0)
                        {
                            ammoTracker[bullet.GetBulletType()].Remove(ai);
                        }
                    }
                }
            }
        }
    }
    #endregion
}
