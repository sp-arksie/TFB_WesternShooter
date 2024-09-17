using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedReward : MonoBehaviour
{
    [SerializeField] List<InventoryItemSO> rewards;
    [SerializeField] List<int> amounts;

    Inventory inventory;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    private void Start()
    {
        Vector2Int inventoryDimensions = inventory.GetInventoryDimensions();
        int k = 0;
        bool allItemsAdded = false;

        for(int i = 0; i < inventoryDimensions.x && allItemsAdded == false; i++)
        {
            for(int j = 0; j < inventoryDimensions.y && allItemsAdded == false; j++)
            {
                int amountNotAdded = inventory.NotifyPlaceItem(new Vector2Int(i, j), rewards[k], amounts[k], rewards[k].ItemSizeInInventory, new Vector2Int(0, 0));
                if (amountNotAdded == 0)
                {
                    k++;
                    if(k > rewards.Count - 1) allItemsAdded = true;
                }
            }
        }
    }
}
