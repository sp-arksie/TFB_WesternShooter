using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotBarManagerForEntity : HotBarManager
{
    List<ItemTypeForEntity> itemData  = new();
    Dictionary<ItemTypeForEntity, List<int>> itemTracker = new();

    float shortRange = 10f;
    float mediumRange = 50f;
    float longRange = 75f;

    protected override void Awake()
    {
        base.Awake();
        for(int i = 0; i < hotBarItems.childCount;  i++)
        {
            ItemBase item = hotBarItems.GetChild(i).GetComponent<ItemBase>();
            EvaluateType(item, i);
        }
    }
    private void EvaluateType(ItemBase item, int index)
    {
        Type itemType = item.GetType();

        if (itemType == typeof(ProjectileWeapon))
        {
            ProjectileWeapon weapon = item as ProjectileWeapon;
            float effectiveRange = weapon.EffectiveRange;

            if (effectiveRange < shortRange)
            {
                AddToItemTracker(ItemTypeForEntity.ShortRangedWeapon, index);
            }
            else if (effectiveRange < mediumRange)
            {
                AddToItemTracker(ItemTypeForEntity.MediumRangedWeapon, index);
            }
            else
            {
                AddToItemTracker(ItemTypeForEntity.LongRangedWeapon, index);
            }

        }
        else if (itemType == typeof(MeleeWeapon))
        {
            AddToItemTracker(ItemTypeForEntity.MeleeWeapon, index);
        }
        else if (itemType == typeof(MedicalItem))
        {
            AddToItemTracker(ItemTypeForEntity.Medical, index);
        }
        else
        {
            Debug.LogWarning($"{item} not accounted for when checking which items {this} is currently holding.");
        }
    }

    private void AddToItemTracker(ItemTypeForEntity itemType, int index)
    {
        if (!itemTracker.ContainsKey(itemType))
        {
            itemTracker.Add(itemType, new List<int> { index });
        }
        else
        {
            itemTracker[itemType].Add(index);
        }
    }

    internal void DoQuickAction() { QuickAction(); }

    internal void DoChargeStart() { }

    internal void DoChargeRelease() { }

    internal void DoSelectItem() { }

    #region Helper Functions
    internal bool GetCurrentItemBusy() { return currentItemBusy; }
    #endregion
}
