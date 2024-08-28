using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Size")]
    [SerializeField] int width = 2;
    [SerializeField] int height = 2;

    private Dictionary<Vector2Int, InventoryGridCell> inventoryContainer = new();
    private Dictionary<string, List<Vector2Int>> itemLocationTracker = new();

    BufferItemData bufferItemData = new();

    #region DEBUG

    [Header("DEBUG")]
    [SerializeField] InventoryItemSO ammoCrate;
    [SerializeField] InventoryItemSO medicalItem;
    [SerializeField] InventoryItemSO SKSCarbine;

    [Space(10)]
    [SerializeField] bool addAmmoCrate = false;
    [SerializeField] bool addMedicalItem = false;
    [SerializeField] bool addSKSCarbine = false;
    [SerializeField] Vector2Int whereToAdd = new(0, 0);
    [SerializeField] Vector2Int localSelectedItemCell = new(1, 1);
    [SerializeField] int amountToAdd = 1;

    [Space(10)]
    [SerializeField] bool showDictionaryLog = false;


    private void OnValidate()
    {
        if (addAmmoCrate) DebugAddAmmoCrate(amountToAdd, whereToAdd, localSelectedItemCell);
        if (addMedicalItem) DebugAddMedicalItem(amountToAdd, whereToAdd, localSelectedItemCell);
        if (addSKSCarbine) DebugAddSKSCarbine(amountToAdd, whereToAdd, localSelectedItemCell);
        if (showDictionaryLog) DebugLogInventory();
    }

    private void DebugAddAmmoCrate(int amount, Vector2Int whereToAdd, Vector2Int localPosition)
    {
        addAmmoCrate = false;
        int returnVal = AddItemManual(ammoCrate, amount, ammoCrate.ItemSizeInInventory, localPosition, whereToAdd);
        Debug.Log($"Amount not added:  {returnVal}");
    }

    private void DebugAddMedicalItem(int amount, Vector2Int whereToAdd, Vector2Int localPosition)
    {
        addMedicalItem = false;
        int returnVal = AddItemManual(medicalItem, amount, medicalItem.ItemSizeInInventory, localPosition, whereToAdd);
        Debug.Log($"Amount not added:  {returnVal}");
    }

    private void DebugAddSKSCarbine(int amount, Vector2Int whereToAdd, Vector2Int localPosition)
    {
        addSKSCarbine = false;
        int returnVal = AddItemManual(SKSCarbine, amount, SKSCarbine.ItemSizeInInventory, localPosition, whereToAdd);
        Debug.Log($"Amount not added:  {returnVal}");
    }

    private void DebugLogInventory()
    {
        showDictionaryLog = false;
        foreach (KeyValuePair<Vector2Int, InventoryGridCell> cell in inventoryContainer)
        {
            if (cell.Value == null)
            {
                Debug.Log(cell.Key + "   empty");
            }
            else
            {
                Debug.Log($"{cell.Key}     {cell.Value.InventoryItem.name}{Environment.NewLine}" +
                    $"CellID: {cell.Value.cellID}{Environment.NewLine}" +
                    $"CurrentAmount:  {cell.Value.CurrentItemAmount}");
            }
        }
    }

    #endregion

    private void Start()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                inventoryContainer.Add(new Vector2Int(i, j), null);
            }
        }
    }

    private int AddItemManual(InventoryItemSO itemToAdd, int amountToAdd, Vector2Int itemDimensions, Vector2Int selectedItemLocalCell, Vector2Int selectedInventoryCell)
    {
        int itemsNotAdded = TryAddItem(itemToAdd, amountToAdd, itemDimensions, selectedItemLocalCell, selectedInventoryCell);

        return itemsNotAdded;
    }

    private int TryAddItem(InventoryItemSO itemToAdd, int amountToAdd, Vector2Int itemDimensions, Vector2Int selectedItemLocalCell, Vector2Int selectedInventoryCell)
    {
        int itemsNotAdded = 0;

        if (inventoryContainer[selectedInventoryCell] == null)
        {
            itemsNotAdded = AddNewItem(itemToAdd, amountToAdd, itemDimensions, selectedItemLocalCell, selectedInventoryCell);
            Debug.Log("cell == null  ->  create new cell");
        }
        else if (itemToAdd.ItemName != inventoryContainer[selectedInventoryCell].InventoryItem.ItemName)
        {
            itemsNotAdded = amountToAdd;
            Debug.Log("items dont match");
        }
        else if (inventoryContainer[selectedInventoryCell].CurrentItemAmount >= itemToAdd.MaximumStacks)
        {
            itemsNotAdded = amountToAdd;
            Debug.Log("currentAmount == maxStacks");
        }
        else if (inventoryContainer[selectedInventoryCell].CurrentItemAmount + amountToAdd <= itemToAdd.MaximumStacks)
        {
            AddStackableItem(amountToAdd, itemDimensions, selectedItemLocalCell, selectedInventoryCell);
            Debug.Log("new total < maxStacks");
        }
        else
        {
            Debug.Log("new total > maxStacks");
            int difference = itemToAdd.MaximumStacks - inventoryContainer[selectedInventoryCell].CurrentItemAmount;
            itemsNotAdded = amountToAdd - difference;

            AddStackableItem(difference, itemDimensions, selectedItemLocalCell, selectedInventoryCell);
        }

        return itemsNotAdded;
    }

    private int AddNewItem(InventoryItemSO itemToAdd, int amountToAdd, Vector2Int itemDimensions, Vector2Int selectedItemLocalCell, Vector2Int selectedInventoryCell)
    {
        int itemsNotAdded = 0;

        if (itemDimensions.x > 1 || itemDimensions.y > 1)
        {
            List<Vector2Int> localOffsets = GetItemLocalCellsWithOffset(itemDimensions, selectedItemLocalCell);
            foreach (Vector2Int offset in localOffsets)
            {
                if (!inventoryContainer.ContainsKey(selectedInventoryCell + offset) || inventoryContainer[selectedInventoryCell + offset] != null)
                    itemsNotAdded = amountToAdd;
            }
            if (itemsNotAdded <= 0)
            {
                string guid = Guid.NewGuid().ToString();
                foreach (Vector2Int offset in localOffsets) { AddToEmptyCell(itemToAdd, amountToAdd, selectedInventoryCell + offset, guid, itemDimensions); }
            }
        }
        else
        {
            if (!inventoryContainer.ContainsKey(selectedInventoryCell))
                itemsNotAdded = amountToAdd;
            else
                AddToEmptyCell(itemToAdd, amountToAdd, selectedInventoryCell, Guid.NewGuid().ToString(), itemDimensions);
        }

        return itemsNotAdded;
    }

    private void AddStackableItem(int amountToAdd, Vector2Int itemDimensions, Vector2Int selectedItemLocalCell, Vector2Int selectedInventoryCell)
    {
        if (itemDimensions.x > 1 || itemDimensions.y > 1)
        {
            foreach (Vector2Int itemLocation in itemLocationTracker[inventoryContainer[selectedInventoryCell].cellID])
            {
                inventoryContainer[itemLocation].AddItemAmount(amountToAdd);
            }
        }
        else
        {
            inventoryContainer[selectedInventoryCell].AddItemAmount(amountToAdd);
        }
    }

    private void AddToEmptyCell(InventoryItemSO itemToAdd, int amountToAdd, Vector2Int locationToAdd, string guid, Vector2Int currentItemOrientation)
    {
        inventoryContainer[locationToAdd] = new InventoryGridCell(itemToAdd, amountToAdd, guid, currentItemOrientation);

        if (itemLocationTracker.ContainsKey(guid))
        {
            itemLocationTracker[guid].Add(locationToAdd);
        }
        else
        {
            List<Vector2Int> list = new() { locationToAdd };
            itemLocationTracker.Add(guid, list);
        }
    }

    private List<Vector2Int> GetItemLocalCellsWithOffset(Vector2Int itemDimensions, Vector2Int selectedItemLocalCell)
    {
        List<Vector2Int> localCellsWithOffset = new List<Vector2Int>();
        for (int i = 0; i < itemDimensions.x; i++)
        {
            for (int j = 0; j < itemDimensions.y; j++)
            {
                localCellsWithOffset.Add((new Vector2Int(i, j)) - selectedItemLocalCell);
            }
        }
        return localCellsWithOffset;
    }

    public void NotifyMovingItem(Vector2Int selectedCell, int id)
    {
        FillBuffersForPotentialChangeInInventory(selectedCell, id);
        RemoveItem(selectedCell);
    }

    private void RemoveItem(Vector2Int selectedCell)
    {
        string guid = inventoryContainer[selectedCell].cellID.ToString();
        foreach (Vector2Int occupiedCell in itemLocationTracker[guid])
        {
            inventoryContainer[occupiedCell] = null;
        }
        itemLocationTracker.Remove(guid);
    }

    private void FillBuffersForPotentialChangeInInventory(Vector2Int itemLocation, int id)
    {
        bufferItemData.inventoryID = id;
        bufferItemData.gridCell = inventoryContainer[itemLocation];
        bufferItemData.selectedLocalCell = GetSelectedLocalCell(itemLocation);
        bufferItemData.wasRotated = false;
        if (inventoryContainer[itemLocation].CurrentItemOrientation != inventoryContainer[itemLocation].InventoryItem.ItemSizeInInventory)
            bufferItemData.shouldRotateOnDraw = true;
        else
            bufferItemData.shouldRotateOnDraw = false;
        bufferItemData.cellsOccupied.Clear();
        bufferItemData.cellsOccupied = itemLocationTracker[bufferItemData.gridCell.cellID];
    }

    private Vector2Int GetSelectedLocalCell(Vector2Int selectedCell)
    {
        Vector2Int smallestXY = new(1000, 1000);
        foreach (Vector2Int cell in itemLocationTracker[inventoryContainer[selectedCell].cellID])
        {
            if (cell.x < smallestXY.x)
                smallestXY.x = cell.x;
            if (cell.y < smallestXY.y)
                smallestXY.y = cell.y;
        }

        Vector2Int selectedLocalCell = selectedCell - smallestXY;

        return selectedLocalCell;
    }

    public void NotifyPlaceItem(Vector2Int selectedCell, BufferItemData bufferData)
    {
        bufferItemData = bufferData;
        Vector2Int selectedLocalCell = bufferItemData.selectedLocalCell;

        if (bufferItemData.wasRotated)
        {
            selectedLocalCell = RecalculateNewLocalCell(selectedLocalCell);
            bufferItemData.gridCell.ChangeItemOrientation();
        }
        int amountNotAdded = AddItemManual(bufferItemData.gridCell.InventoryItem, bufferItemData.gridCell.CurrentItemAmount, bufferItemData.gridCell.CurrentItemOrientation, selectedLocalCell, selectedCell);
        if (amountNotAdded > 0)
        {
            if (bufferItemData.wasRotated) bufferItemData.gridCell.ChangeItemOrientation();
            PlaceInOriginalLocation(amountNotAdded);
        }
    }

    private Vector2Int RecalculateNewLocalCell(Vector2Int selectedLocalCell)
    {
        if (bufferItemData.gridCell.CurrentItemOrientation != bufferItemData.gridCell.InventoryItem.ItemSizeInInventory)
        {
            int x = selectedLocalCell.y;
            int y = (bufferItemData.gridCell.CurrentItemOrientation.x - 1) - selectedLocalCell.x;
            return new Vector2Int(x, y);
        }
        else
        {
            int x = selectedLocalCell.x;
            int y = (bufferItemData.gridCell.CurrentItemOrientation.y - 1) - selectedLocalCell.y;
            return new Vector2Int(y, x);
        }
    }

    public void NotifyRotatedItem()
    {
        if (bufferItemData.wasRotated == false)
            bufferItemData.wasRotated = true;
        else
            bufferItemData.wasRotated = false;
    }

    public void NotifyFailedItemPlacement(int amount)
    {
        PlaceInOriginalLocation(amount);
    }

    private void PlaceInOriginalLocation(int amount)
    {
        foreach (Vector2Int cellOccupied in bufferItemData.cellsOccupied)
        {
            AddToEmptyCell(bufferItemData.gridCell.InventoryItem, amount, cellOccupied, bufferItemData.gridCell.cellID, bufferItemData.gridCell.CurrentItemOrientation);
            if (inventoryContainer[cellOccupied].CurrentItemOrientation != bufferItemData.gridCell.CurrentItemOrientation)
                inventoryContainer[cellOccupied].ChangeItemOrientation();
        }
    }

    #region WIP

    // =============================================================
    // For QOL eg: shift-clicking item to transfer to your inventory
    public void AddItemAutomatic(InventoryItemSO itemToAdd, int amountToAdd, Vector2Int itemDimensions)
    {

    }

    //private void CheckForEligibleNonEmptySlot(InventoryItemSO itemToAdd, int amountToAdd, Vector2Int itemDimensions)
    //{
    //    // only if max stacks is > 1
    //    List<Vector2Int> matchingItems = itemTracker.Where(x => x.Value == itemToAdd.ItemID).Select(y => y.Key).ToList();

    //    if (matchingItems != null)
    //    {
    //        AddToExistingCell(itemToAdd, amountToAdd, itemDimensions, matchingItems);
    //    }
    //    else
    //    {
    //        // TODO: add to new cell
    //    }
    //}

    private void AddToExistingCell(InventoryItemSO itemToAdd, int amountToAdd, Vector2Int itemDimensions, List<Vector2Int> matchingItems)
    {
        // TO DO: continue QOL inventory stuff
    }
    // =============================================================

    #endregion

    #region Helper Functions

    public Dictionary<Vector2Int, InventoryGridCell> GetInventoryContainer() { return inventoryContainer; }
    public Dictionary<string, List<Vector2Int>> GetItemLocationTracker() { return itemLocationTracker; }
    public BufferItemData GetBufferItemData() { return bufferItemData; }
    public Vector2Int GetInventoryDimensions() { return new Vector2Int(width, height); }

    #endregion

}

public class BufferItemData
{
    public int inventoryID = -1;
    public InventoryGridCell gridCell;
    public List<Vector2Int> cellsOccupied = new();
    public Vector2Int selectedLocalCell;
    public bool wasRotated = false;
    public bool shouldRotateOnDraw = false;
}