using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInventoryHandler;

public class PlayerInventoryHandler : MonoBehaviour
{
    [SerializeField] Canvas inventoryCanvas;
    [SerializeField] Canvas[] otherCanvases;
    [SerializeField] CinemachineInputProvider cinemachineInputProvider;

    Inventory playerInventory;
    InventoryManager inventoryManager;
    InputManager input;
    ItemInventoryMediator inventoryMediator;

    bool shouldOpenInventory = false;

    #region DEBUG
    [Header("Debug")]
    [SerializeField] GameObject debugItemToEquip;
    [SerializeField] bool debugEquipItem = false;
    private void OnValidate()
    {
        if (debugEquipItem)
        {
            //iim.DebugNotifyItemEquipped("1234", debugItemToEquip, 1);
        }
    }
    #endregion

    private void Awake()
    {
        playerInventory = GetComponent<Inventory>();
        inventoryManager = InventoryManager.Instance;
        inventoryManager.AddOverlappingInventory(playerInventory);
        input = InputManager.Instance;
        inventoryMediator = ItemInventoryMediator.Instance;
    }

    private void OnEnable()
    {
        input.GetInventoryActivated().performed += HandleActionMaps;
    }

    private void OnDisable()
    {
        input.GetInventoryActivated().performed -= HandleActionMaps;
    }

    private void HandleActionMaps(InputAction.CallbackContext context)
    {
        shouldOpenInventory = ShouldOpenInventory();
        if (shouldOpenInventory)
            OpenInventory();
        else
            CloseInventory();
    }

    private bool ShouldOpenInventory()
    {
        if (shouldOpenInventory)
            return false;
        else
            return true;
    }

    private void OpenInventory()
    {
        // update ammo changes to inv
        playerInventory.NotifyUpdateProjectiles(inventoryMediator.projectileTracker);

        string[] actionMapsForInventory = new string[] { input.UiActionMap, input.InventoryActivatorActionMap };
        input.SetActionMaps(actionMapsForInventory, true);
        cinemachineInputProvider.enabled = false;

        foreach (Canvas c in otherCanvases) { c.gameObject.SetActive(false); }
        inventoryCanvas.gameObject.SetActive(true);
    }

    private void CloseInventory()
    {
        string[] actionMapsForInventory = new string[] { input.UiActionMap };
        input.SetActionMaps(actionMapsForInventory, false);
        cinemachineInputProvider.enabled = true;

        foreach (Canvas c in otherCanvases) { c.gameObject.SetActive(true); }
        inventoryCanvas.gameObject.SetActive(false);

        inventoryMediator.UpdateAmmoTracker(playerInventory);
    }

    //================================

    public void NotifyInventoryClosed()
    {
        // TODO: need to update equipped items also
    }
}
