using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New InventoryItemSO", menuName = "Inventory System/InventoryItemSO")]
public class InventoryItemSO : ScriptableObject
{
    [SerializeField] string itemName;
    [SerializeField][TextArea(10, 20)] string description;
    [SerializeField] GameObject itemVisualPrefab;
    [SerializeField] GameObject itemPhysicalPrefab;
    [SerializeField] int maximumStacks = 1;
    [SerializeField] Vector2Int itemSizeInInventory = new Vector2Int(1, 1);
    [SerializeField] bool canRotate = false;

    public string ItemName { get { return itemName; } private set { itemName = value; } }
    public string Description { get { return description; } private set { description = value; } }
    public GameObject ItemVisual { get { return itemVisualPrefab; } private set { itemVisualPrefab = value; } }
    public GameObject ItemPhysical { get => itemPhysicalPrefab; private set => itemPhysicalPrefab = value; }
    public int MaximumStacks { get { return maximumStacks; } private set { maximumStacks = value; } }
    public Vector2Int ItemSizeInInventory { get { return itemSizeInInventory; } private set { itemSizeInInventory = value; } }
    public bool CanRotate { get { return canRotate; } private set { canRotate = value; } }
}
