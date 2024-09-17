using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    GameObject currentHoveringCell;
    Vector2Int currentHoveringCellPosition = Vector2IntExtensions.Empty;
    Vector2Int currentSelectedCellPosition = Vector2IntExtensions.Empty;
    int currentHoveringInventoryId = -1;

    InputManager input;
    InventoryManager inventoryManager;
    ItemInventoryMediator inventoryMediator;

    GraphicRaycaster graphicRaycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    List<RaycastResult> results = new();


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
        input = InputManager.Instance;
        inventoryManager = InventoryManager.Instance;
        inventoryMediator = ItemInventoryMediator.Instance;
        graphicRaycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
        inventoryGridLayoutGroups = new GridLayoutGroup[InventoryCellsParent.childCount];
        inventoryGridLayoutGroups = InventoryCellsParent.GetComponentsInChildren<GridLayoutGroup>(true);

        PrepareGridCells(true);
    }

    private void OnEnable()
    {
        input.GetMousePositionAction().performed += OnMouseMove;
        input.GetLeftClickAction().started += OnLeftClickPress;
        input.GetLeftClickAction().canceled += OnLeftClickRelease;
        input.GetRightClickAction().performed += OnRightClick;
        input.GetRotatedAction().performed += OnTryRotateDraggedItem;

        if (inventoryManager.inventories.Count > 1)
        {
            ClearGridCells();
            InventoryCellsParent.GetChild(1).gameObject.SetActive(true);
            InventoryVisualsParent.GetChild(1).gameObject.SetActive(true);
            PrepareGridCells(false);
        }

        StartCoroutine(DrawInventoryEndOfFrame());

        ContextMenu.OnEquipSelected += SendEquippedItemInfo;
        ContextMenu.OnUnequipSelected += SendUnequippedItemInfo;
        ContextMenu.OnContextMenuClose += ResetSelectedCell;

        inventoryManager.inventories[0].OnIdChange += SendIdChangeInfo;
    }

    private void OnDisable()
    {
        if (inventoryManager.inventories.Count > 1)
        {
            InventoryCellsParent.GetChild(1).gameObject.SetActive(false);
            InventoryVisualsParent.GetChild(1).gameObject.SetActive(false);
        }

        input.GetMousePositionAction().performed -= OnMouseMove;
        input.GetLeftClickAction().started -= OnLeftClickPress;
        input.GetLeftClickAction().canceled -= OnLeftClickRelease;
        input.GetRightClickAction().performed -= OnRightClick;
        input.GetRotatedAction().performed -= OnTryRotateDraggedItem;

        ContextMenu.OnEquipSelected -= SendEquippedItemInfo;
        ContextMenu.OnUnequipSelected -= SendUnequippedItemInfo;
        ContextMenu.OnContextMenuClose -= ResetSelectedCell;

        inventoryManager.inventories[0].OnIdChange -= SendIdChangeInfo;
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
                if (x == 0 && y == 0) firstCellRectTransform[i] = rectCell;
                go.GetComponentInChildren<TextMeshProUGUI>().text = $"{x}, {y}";
                GridCellUI gcui = go.GetComponent<GridCellUI>();
                gcui.SetLocationInGrid(x, y);
                gcui.SetId(i);
            }
        }
    }

    private void ClearGridCells()
    {
        Transform otherInventory = InventoryCellsParent.GetChild(1);
        for (int i = 0; i < inventoryGridLayoutGroups[1].transform.childCount; i++)
        {
            Destroy(inventoryGridLayoutGroups[1].transform.GetChild(i).gameObject);
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

            Dictionary<Vector2Int, InventoryGridCell> inventoryContainer = inventoryManager.inventories[i].inventoryContainer;
            foreach (KeyValuePair<string, List<Vector2Int>> item in inventoryManager.inventories[i].itemLocationTracker)
            {
                InventoryGridCell itemCell = inventoryContainer[item.Value[0]];
                GameObject go = Instantiate(itemCell.InventoryItem.ItemVisual, InventoryVisualsParent.GetChild(i).transform, false);
                go.GetComponentInChildren<TextMeshProUGUI>().text = GetItemAmountString(itemCell.CurrentItemAmount);
                RectTransform rectItem = go.GetComponent<RectTransform>();

                int itemWidth = itemCell.CurrentItemOrientation.x;
                int itemHeight = itemCell.CurrentItemOrientation.y;

                rectItem.anchorMin = firstCellRectTransform[i].anchorMin;
                rectItem.anchorMax = firstCellRectTransform[i].anchorMax;

                if (itemWidth != itemCell.InventoryItem.ItemSizeInInventory.x || itemHeight != itemCell.InventoryItem.ItemSizeInInventory.y)
                    rectItem.rotation = RotateDrawnitem();

                Vector2 anchoredPosition = GetAnchoredPosition(item, i);
                rectItem.anchoredPosition = GetItemPosition(anchoredPosition, itemWidth, itemHeight, i);
            }
        }
    }

    // Children of the GridLayoutGroup get their anchors set after the 1st frame.
    // Drawn items use the anchors to position the image, so need to delay to get the correct anchor.
    private IEnumerator DrawInventoryEndOfFrame()
    {
        yield return new WaitForEndOfFrame();

        DrawInventory();
    }

    private Quaternion RotateDrawnitem()
    {
        return Quaternion.AngleAxis(90f, Vector3.forward);
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

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        if (currentSelectedCellPosition == Vector2IntExtensions.Empty)
        {
            pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = input.GetMousePosition();

            results.Clear();
            graphicRaycaster.Raycast(pointerEventData, results);
            if (results.Count > 0)
            {
                GridCellUI gcui = results[0].gameObject.GetComponent<GridCellUI>();
                currentHoveringCellPosition = gcui.cellPosition;
                currentHoveringInventoryId = gcui.Id;
                currentHoveringCell = results[0].gameObject;
            }
            else
            {
                currentHoveringCellPosition = Vector2IntExtensions.Empty;
                currentHoveringInventoryId = -1;
                currentHoveringCell = null;
            }

            if (itemBeingDragged != null)
            {
                RectTransform rect = itemBeingDragged.GetComponent<RectTransform>();
                rect.position = input.GetMousePosition();
            }
        }
    }

    private void OnLeftClickPress(InputAction.CallbackContext context)
    {
        if (currentHoveringCell != null && currentSelectedCellPosition == Vector2IntExtensions.Empty)
        {
            GridCellUI selectedCell = currentHoveringCell.GetComponent<GridCellUI>();
            Inventory inventory = inventoryManager.inventories[selectedCell.Id];

            if (inventory.inventoryContainer[selectedCell.cellPosition] != null)
            {
                inventory.NotifyMovingItem(selectedCell.cellPosition, selectedCell.Id);
                DrawInventory();
                bufferItemData = inventory.GetBufferItemData();

                itemBeingDragged = Instantiate(bufferItemData.gridCell.InventoryItem.ItemVisual, InventoryVisualsParent.GetChild(selectedCell.Id).transform, false);
                if (bufferItemData.shouldRotateOnDraw)
                    itemBeingDragged.GetComponent<RectTransform>().rotation = RotateDrawnitem();

                itemBeingDragged.GetComponentInChildren<TextMeshProUGUI>().text = GetItemAmountString(bufferItemData.gridCell.CurrentItemAmount);

                PrepareDraggedObject(currentHoveringCell, selectedCell.cellPosition, bufferItemData.shouldRotateOnDraw);
            }
        }
    }

    private string GetItemAmountString(int itemAmount)
    {
        return itemAmount == 1 ? "" : itemAmount.ToString();
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

    private void SetAlpha()
    {
        Color color = itemBeingDragged.GetComponent<Image>().color;
        itemBeingDragged.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.75f);
    }

    private void OnLeftClickRelease(InputAction.CallbackContext context)
    {
        if (itemBeingDragged != null)
        {
            if(currentHoveringCellPosition != Vector2IntExtensions.Empty)
            {
                Inventory inventory = inventoryManager.inventories[currentHoveringInventoryId];
                inventory.NotifyPlaceItem(currentHoveringCellPosition, bufferItemData);
            }
            else
            {
                inventoryManager.inventories[bufferItemData.inventoryID].NotifyFailedItemPlacement(bufferItemData.gridCell.CurrentItemAmount);
            }
            itemBeingDragged = null;
            DrawInventory();
        }
    }

    private void OnTryRotateDraggedItem(InputAction.CallbackContext context)
    {
        if ((Time.time - timeSinceLastRotation) > rotationCooldown && itemBeingDragged != null && bufferItemData.gridCell.InventoryItem.CanRotate)
        {
            if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
            timeSinceLastRotation = Time.time;
            RotateDraggedItem();
            inventoryManager.inventories[bufferItemData.inventoryID].NotifyRotatedItem();
        }
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
        float dt = 0f;
        float t = 0f;

        while (t <= 1f)
        {
            t = dt / rotationDuration;
            rectItemBeingDragged.rotation = Quaternion.Lerp(startRotation, finalRotation, t);
            yield return new WaitForEndOfFrame();
            dt += Time.deltaTime;
        }
        rectItemBeingDragged.rotation = finalRotation;
    }

    private void OnRightClick(InputAction.CallbackContext context)
    {
        if(itemBeingDragged != null)
        {
            inventoryManager.inventories[bufferItemData.inventoryID].NotifyFailedItemPlacement(bufferItemData.gridCell.CurrentItemAmount);
        }

        if(currentHoveringInventoryId == 0 && inventoryManager.inventories[0].inventoryContainer[currentHoveringCellPosition] != null)
        {
            //input.GetMousePositionAction().performed -= OnMouseMove;

            currentSelectedCellPosition = currentHoveringCellPosition;
            string selectedItemName = inventoryManager.inventories[0].inventoryContainer[currentHoveringCellPosition].InventoryItem.name;
            ContextMenu.NotifyOpenContextMenu(selectedItemName);

            //input.GetMousePositionAction().performed += OnMouseMove;
        }
    }

    private void SendEquippedItemInfo()
    {
        // TODO: Check if already equipped etc

        Debug.Log("SendEquippedItemInfo");
        InventoryGridCell igc = inventoryManager.inventories[0].inventoryContainer[currentSelectedCellPosition];
        inventoryMediator.NotifyItemEquipped(igc);
    }

    private void SendUnequippedItemInfo()
    {
        InventoryGridCell igc = inventoryManager.inventories[0].inventoryContainer[currentSelectedCellPosition];
        inventoryMediator.NotifyItemUnequipped(igc);
    }

    private void ResetSelectedCell()
    {
        Debug.Log("ResetSelectedCell");
        currentSelectedCellPosition = Vector2IntExtensions.Empty;
    }

    private void SendIdChangeInfo(string oldId, string newId)
    {
        inventoryMediator.NotifyIdUpdate(oldId, newId);
    }

    //public void ClearLog()
    //{
    //    var assembly = Assembly.GetAssembly(typeof(Editor));
    //    var type = assembly.GetType("UnityEditor.LogEntries");
    //    var method = type.GetMethod("Clear");
    //    method.Invoke(new object(), null);
    //}
}
