using UnityEngine;

public class InventoryGridCell
{
    public InventoryItemSO InventoryItem { get; private set; }

    public string cellID { get; private set; }

    public int CurrentItemAmount { get; private set; }

    private Vector2Int currentItemOrientation;


    public Vector2Int CurrentItemOrientation { get { return currentItemOrientation; } private set { currentItemOrientation = value; } }

    //public InventoryGridCell(InventoryItemSO inventoryItem, int amount, string guid)
    //{
    //    this.inventoryItem = inventoryItem;
    //    this.currentItemAmount = amount;
    //    cellID = guid;
    //    currentItemOrientation = inventoryItem.ItemSizeInInventory;
    //}

    public InventoryGridCell(InventoryItemSO inventoryItem, int amount, string guid, Vector2Int currentItemOrientation)
    {
        this.InventoryItem = inventoryItem;
        this.CurrentItemAmount = amount;
        cellID = guid;
        this.CurrentItemOrientation = currentItemOrientation;
    }

    public void AddItemAmount(int amount)
    {
        CurrentItemAmount += amount;
    }

    public void ChangeItemOrientation()
    {
        int buffer = CurrentItemOrientation.x;
        currentItemOrientation.x = CurrentItemOrientation.y;
        currentItemOrientation.y = buffer;
    }
}
