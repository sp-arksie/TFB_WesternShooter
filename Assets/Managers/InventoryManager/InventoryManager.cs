using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager instance;

    public static InventoryManager Instance { get { return instance; } }

    public List<Inventory> inventories = new();

    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
        }
    }

    public void AddOverlappingInventory(Inventory inventory)
    {
        inventories.Add(inventory);
    }

    public void RemoveOverlappingInventory(Inventory inventory)
    {
        inventories.RemoveAt(1);
    }
}
