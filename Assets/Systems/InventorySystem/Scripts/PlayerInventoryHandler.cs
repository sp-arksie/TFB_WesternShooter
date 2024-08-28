using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryHandler : MonoBehaviour
{
    Inventory playerInventory;
    InventoryManager inventoryManager;

    private void Awake()
    {
        playerInventory = GetComponent<Inventory>();
        inventoryManager = InventoryManager.Instance;
        inventoryManager.AddOverlappingInventory(playerInventory);
    }
}
