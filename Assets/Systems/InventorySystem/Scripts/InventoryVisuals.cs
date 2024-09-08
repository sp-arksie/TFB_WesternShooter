using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;

public class InventoryVisuals : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Transform InventoryCellsParent;
    [SerializeField] Transform InventoryVisualsParent;
    [SerializeField] GameObject cellVisualPrefab;

    [Header("Rotation")]
    [SerializeField] float rotationDuration = 0.5f;
    [SerializeField] float rotationCooldown = 0.5f;
    float timeSinceLastRotation;
    Coroutine rotationCoroutine;

    GridLayoutGroup[] inventoryGridLayoutGroups;
    List<RectTransform> firstCellRectTransform = new() { null, null };

    BufferItemData bufferItemData;
    GameObject itemBeingDragged;
    Vector2Int currentHoveringCell = new(-1, -1);
    int currentHoveringInventoryId = -1;

    InputManager input;
    InventoryManager inventoryManager;


    #region DEBUG

    [Header("Debug")]
    [SerializeField] bool debugDrawInventory = false;

    private void OnValidate()
    {
        if (debugDrawInventory)
        {
            debugDrawInventory = false;
            DrawInventory();
        }
    }

    #endregion

    private void Awake()
    {
        inventoryGridLayoutGroups = new GridLayoutGroup[InventoryCellsParent.childCount];
        for (int i = 0; i < InventoryCellsParent.childCount; i++)
        {
            inventoryGridLayoutGroups[i] = InventoryCellsParent.GetChild(i).GetComponent<GridLayoutGroup>();
        }
        input = InputManager.Instance;
        inventoryManager = InventoryManager.Instance;

        PrepareGridCells(true);
    }

    private void OnEnable()
    {
        if (inventoryManager.inventories.Count > 1)
        {
            InventoryCellsParent.GetChild(1).gameObject.SetActive(true);
            InventoryVisualsParent.GetChild(1).gameObject.SetActive(true);
            PrepareGridCells(false);
        }

        StartCoroutine(DrawInventoryEndOfFrame());
    }

    private void OnDisable()
    {
        if (inventoryManager.inventories.Count > 1)
        {
            InventoryCellsParent.GetChild(1).gameObject.SetActive(false);
            InventoryVisualsParent.GetChild(1).gameObject.SetActive(false);
        }
    }

    private IEnumerator DrawInventoryEndOfFrame()
    {
        yield return new WaitForEndOfFrame();

        DrawInventory();
    }

    private void Update()
    {
        timeSinceLastRotation += Time.deltaTime;

        if (timeSinceLastRotation > rotationCooldown && Input.GetKey(KeyCode.Space) && itemBeingDragged != null && bufferItemData.gridCell.InventoryItem.CanRotate)
        {
            if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
            timeSinceLastRotation = 0f;
            RotateDraggedItem();
            inventoryManager.inventories[bufferItemData.inventoryID].NotifyRotatedItem();
        }
    }

    private void PrepareGridCells(bool isPlayerInventory)
    {
        int i = isPlayerInventory == true ? 0 : 1;

        Vector2Int dimensions = inventoryManager.inventories[i].GetInventoryDimensions();

        inventoryGridLayoutGroups[i].constraint = GridLayoutGroup.Constraint.FixedRowCount;
        inventoryGridLayoutGroups[i].constraintCount = dimensions.y;

        for (int y = 0; y < dimensions.y; y++)
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                GameObject go = Instantiate(cellVisualPrefab, Vector3.zero, Quaternion.identity, InventoryCellsParent.GetChild(i).transform);
                RectTransform rectCell = go.GetComponent<RectTransform>();
                rectCell.anchorMin = new(0, 1);
                rectCell.anchorMax = new(0, 1);
                if (x == 0 && y == 0) firstCellRectTransform[i] = rectCell;
                go.GetComponentInChildren<TextMeshProUGUI>().text = $"{x}, {y}";
                GridCellUI gcui = go.GetComponent<GridCellUI>();
                gcui.SetLocationInGrid(x, y);
                gcui.SetId(i);

                AddEvent(go, EventTriggerType.PointerEnter, delegate { OnPointerEnter(go); });
                AddEvent(go, EventTriggerType.PointerExit, delegate { OnPointerExit(go); });
                AddEvent(go, EventTriggerType.BeginDrag, delegate { OnBeginDrag(go); });
                AddEvent(go, EventTriggerType.Drag, delegate { OnDrag(go); });
                AddEvent(go, EventTriggerType.EndDrag, delegate { OnEndDrag(go); });
            }
        }
    }

    private void DrawInventory()
    {
        for (int i = 0; i < inventoryGridLayoutGroups.Length && inventoryGridLayoutGroups[i].gameObject.activeSelf; i++)
        {
            for (int k = InventoryVisualsParent.GetChild(i).transform.childCount - 1; k >= 0; k--)
            {
                Destroy(InventoryVisualsParent.GetChild(i).transform.GetChild(k).gameObject);
            }

            Dictionary<Vector2Int, InventoryGridCell> container = inventoryManager.inventories[i].GetInventoryContainer();
            foreach (KeyValuePair<string, List<Vector2Int>> kvp in inventoryManager.inventories[i].GetItemLocationTracker())
            {
                GameObject go = Instantiate(container[kvp.Value[0]].InventoryItem.ItemVisual, InventoryVisualsParent.GetChild(i).transform, false);
                go.GetComponentInChildren<TextMeshProUGUI>().text = GetItemAmountString(container[kvp.Value[0]].CurrentItemAmount);
                RectTransform rectItem = go.GetComponent<RectTransform>();

                int itemWidth = container[kvp.Value[0]].CurrentItemOrientation.x;
                int itemHeight = container[kvp.Value[0]].CurrentItemOrientation.y;

                rectItem.anchorMin = firstCellRectTransform[i].anchorMin;
                rectItem.anchorMax = firstCellRectTransform[i].anchorMax;

                if (itemWidth != container[kvp.Value[0]].InventoryItem.ItemSizeInInventory.x || itemHeight != container[kvp.Value[0]].InventoryItem.ItemSizeInInventory.y)
                    rectItem.rotation = RotateDrawnitem();

                Vector2 anchoredPosition = GetAnchoredPosition(kvp, i);
                rectItem.anchoredPosition = GetItemPosition(anchoredPosition, itemWidth, itemHeight, i);
            }
        }
    }

    private Vector2 GetAnchoredPosition(KeyValuePair<string, List<Vector2Int>> kvp, int i)
    {
        return new Vector2(firstCellRectTransform[i].anchoredPosition.x + kvp.Value[0].x * inventoryGridLayoutGroups[i].cellSize.x,
                           firstCellRectTransform[i].anchoredPosition.y + kvp.Value[0].y * inventoryGridLayoutGroups[i].cellSize.y);
    }

    private Vector2 GetItemPosition(Vector2 anchoredPosition, int itemWidth, int itemHeight, int i)
    {
        return anchoredPosition + new Vector2((inventoryGridLayoutGroups[i].cellSize.x * 0.5f * (itemWidth - 1)), (inventoryGridLayoutGroups[i].cellSize.y * 0.5f * (itemHeight - 1)));
    }

    private void AddEvent(GameObject cell, EventTriggerType eventTriggerType, UnityAction<BaseEventData> callback)
    {
        EventTrigger eventTrigger = cell.GetComponent<EventTrigger>();
        EventTrigger.Entry eventTriggerEntry = new EventTrigger.Entry();
        eventTriggerEntry.eventID = eventTriggerType;
        eventTriggerEntry.callback.AddListener(callback);
        eventTrigger.triggers.Add(eventTriggerEntry);
    }

    public void OnPointerEnter(GameObject cell)
    {
        currentHoveringCell = cell.GetComponent<GridCellUI>().cellPosition;
        currentHoveringInventoryId = cell.GetComponent<GridCellUI>().Id;
    }

    public void OnPointerExit(GameObject cell)
    {
        currentHoveringCell = new(-1, -1);
    }

    public void OnBeginDrag(GameObject cell)
    {
        GridCellUI selectedCell = cell.GetComponent<GridCellUI>();
        Inventory inventory = inventoryManager.inventories[selectedCell.Id];

        if (inventory.GetInventoryContainer()[selectedCell.cellPosition] != null)
        {
            inventory.NotifyMovingItem(selectedCell.cellPosition, selectedCell.Id);
            DrawInventory();
            bufferItemData = inventory.GetBufferItemData();

            itemBeingDragged = Instantiate(bufferItemData.gridCell.InventoryItem.ItemVisual, InventoryVisualsParent.GetChild(selectedCell.Id).transform, false);
            if (bufferItemData.shouldRotateOnDraw)
                itemBeingDragged.GetComponent<RectTransform>().rotation = RotateDrawnitem();

            itemBeingDragged.GetComponentInChildren<TextMeshProUGUI>().text = GetItemAmountString(bufferItemData.gridCell.CurrentItemAmount);

            PrepareDraggedObject(cell, selectedCell.cellPosition, bufferItemData.shouldRotateOnDraw);
        }
    }

    public void OnDrag(GameObject cell)
    {
        if (itemBeingDragged != null)
            itemBeingDragged.GetComponent<RectTransform>().position = input.GetMousePosition();
    }

    public void OnEndDrag(GameObject cell)
    {
        if (itemBeingDragged != null)
        {
            Inventory inventory = inventoryManager.inventories[currentHoveringInventoryId];

            if (!inventory.GetInventoryContainer().ContainsKey(currentHoveringCell))
            {
                inventory.NotifyFailedItemPlacement(bufferItemData.gridCell.CurrentItemAmount);
            }
            else
            {
                inventory.NotifyPlaceItem(currentHoveringCell, bufferItemData);
            }
            itemBeingDragged = null;
            DrawInventory();
        }
    }

    private void PrepareDraggedObject(GameObject cell, Vector2Int selectedCell, bool shouldRotate)
    {
        RectTransform rectCell = cell.GetComponent<RectTransform>();
        RectTransform rectImage = itemBeingDragged.GetComponent<RectTransform>();

        rectImage.anchorMax = rectCell.anchorMax;
        rectImage.anchorMin = rectCell.anchorMin;

        if (bufferItemData.gridCell.CurrentItemOrientation.x > 1 ||
            bufferItemData.gridCell.CurrentItemOrientation.y > 1)
        { rectImage.pivot = GetNewPivot(selectedCell, shouldRotate); }

        //rectImage.position = rectCell.position;
        //lerpToPivot = StartCoroutine(LerpToPivot());
        rectImage.position = input.GetMousePosition();
        SetAlpha();
    }

    private void SetAlpha()
    {
        Color color = itemBeingDragged.GetComponent<Image>().color;
        itemBeingDragged.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.75f);
    }

    private Vector2 GetNewPivot(Vector2Int selectedCell, bool shouldRotate)
    {
        Vector2 pivot = Vector2.zero;
        Vector2Int itemDefaultDimensions = bufferItemData.gridCell.InventoryItem.ItemSizeInInventory;

        Vector2Int selectedLocalCell = GetSelectedLocalCell(selectedCell, shouldRotate);

        if (selectedLocalCell.x > 0) { pivot.x = (1f / ((float)itemDefaultDimensions.x * 2f)) + ((1f / (float)itemDefaultDimensions.x) * selectedLocalCell.x); }
        else { pivot.x = (1f / ((float)itemDefaultDimensions.x * 2f)); }

        if (selectedLocalCell.y > 0) { pivot.y = (1f / ((float)itemDefaultDimensions.y * 2f)) + ((1f / (float)itemDefaultDimensions.y) * selectedLocalCell.y); }
        else { pivot.y = (1f / ((float)itemDefaultDimensions.y * 2f)); }

        return pivot;
    }

    private Vector2Int GetSelectedLocalCell(Vector2Int selectedCell, bool shouldRotate)
    {
        Vector2Int smallestXY = new(1000, 1000);
        foreach (Vector2Int cell in bufferItemData.cellsOccupied)
        {
            if (cell.x < smallestXY.x)
                smallestXY.x = cell.x;
            if (cell.y < smallestXY.y)
                smallestXY.y = cell.y;
        }

        Vector2Int selectedLocalCell = selectedCell - smallestXY;
        if (shouldRotate)
            selectedLocalCell = GetLocalCellOfItemDefaultOrientation(selectedLocalCell);

        return selectedLocalCell;
    }

    private Vector2Int GetLocalCellOfItemDefaultOrientation(Vector2Int oldLocalSelectedCell)
    {
        int x = oldLocalSelectedCell.y;
        int y = (bufferItemData.gridCell.CurrentItemOrientation.x - 1) - oldLocalSelectedCell.x;

        return new Vector2Int(x, y);
    }

    private void RotateDraggedItem()
    {
        RectTransform rectItem = itemBeingDragged.GetComponent<RectTransform>();

        if (rectItem.rotation.z > Quaternion.identity.z)
        {
            rotationCoroutine = StartCoroutine(StartRotating(rectItem.rotation, Quaternion.AngleAxis(0f, Vector3.forward)));
        }
        else
        {
            rotationCoroutine = StartCoroutine(StartRotating(rectItem.rotation, Quaternion.AngleAxis(90f, Vector3.forward)));
        }
    }

    private IEnumerator StartRotating(Quaternion startRotation, Quaternion finalRotation)
    {
        RectTransform rectItemBeingDragged = itemBeingDragged.GetComponent<RectTransform>();
        while (timeSinceLastRotation <= rotationDuration)
        {
            float t = timeSinceLastRotation / rotationDuration;
            rectItemBeingDragged.rotation = Quaternion.Lerp(startRotation, finalRotation, t);
            yield return new WaitForEndOfFrame();
        }
        rectItemBeingDragged.rotation = finalRotation;
    }

    private Quaternion RotateDrawnitem()
    {
        return Quaternion.AngleAxis(90f, Vector3.forward);
    }

    private string GetItemAmountString(int itemAmount)
    {
        return itemAmount == 1 ? "" : itemAmount.ToString();
    }

    //public void ClearLog()
    //{
    //    var assembly = Assembly.GetAssembly(typeof(Editor));
    //    var type = assembly.GetType("UnityEditor.LogEntries");
    //    var method = type.GetMethod("Clear");
    //    method.Invoke(new object(), null);
    //}
}
