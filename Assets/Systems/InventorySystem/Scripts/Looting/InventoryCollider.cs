using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class InventoryCollider : MonoBehaviour
{
    Inventory inventory;
    InventoryManager inventoryManager;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        inventoryManager = InventoryManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inventoryManager.AddOverlappingInventory(inventory);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inventoryManager.RemoveOverlappingInventory(inventory);
        }
    }
}
