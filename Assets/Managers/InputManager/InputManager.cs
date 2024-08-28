using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    public static InputManager Instance
    {
        get { return instance; }
    }

    PlayerControls playerControls;
    List<InputActionMap> inputActionMaps = new();

    List<InputAction> hotBarSlots = new();

    string locomotionActionMap = "Locomotion";
    string aimActionMap = "Aim";
    string actionsActionMap = "Actions";
    string uiActionMap = "UI";

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        playerControls = new();

        inputActionMaps.Add( playerControls.asset.FindActionMap(locomotionActionMap) );
        inputActionMaps.Add( playerControls.asset.FindActionMap(aimActionMap) );
        inputActionMaps.Add( playerControls.asset.FindActionMap(aimActionMap) );
        inputActionMaps.Add( playerControls.asset.FindActionMap(uiActionMap) );
    }

    private void Start()
    {
        hotBarSlots.Add(playerControls.Actions.HotBarSlot1);
        hotBarSlots.Add(playerControls.Actions.HotBarSlot2);
        hotBarSlots.Add(playerControls.Actions.HotBarSlot3);
        hotBarSlots.Add(playerControls.Actions.HotBarSlot4);
        hotBarSlots.Add(playerControls.Actions.HotBarSlot5);
        hotBarSlots.Add(playerControls.Actions.HotBarSlot6);
        hotBarSlots.Add(playerControls.Actions.HotBarSlot7);
        hotBarSlots.Add(playerControls.Actions.HotBarSlot8);
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    #region LOCOMOTION
    
    public Vector2 GetPlayerMovement() { return playerControls.Locomotion.Movement.ReadValue<Vector2>(); }
    public bool GetIsRunning() { return playerControls.Locomotion.Run.IsPressed(); }
    public bool GetHasJumped() { return playerControls.Locomotion.Jump.WasPressedThisFrame(); }
    public bool GetIsCrouching() { return playerControls.Locomotion.Crouch.IsPressed(); }

    #endregion


    #region ACTIONS

    public InputAction GetUseAction() { return playerControls.Actions.Use; }
    public InputAction GetShootAction() { return playerControls.Actions.Shoot; }
    public InputAction GetAimAction() {  return playerControls.Actions.Aim; }
    public InputAction GetReloadAction() { return playerControls.Actions.Reload; }
    public InputAction GetScrollAction() { return playerControls.Actions.Scroll; }
    public List<InputAction> GetHotBarSlotActions() { return hotBarSlots; }
    #endregion

    #region UI

    public Vector2 GetMousePosition() { return playerControls.UI.MousePosition.ReadValue<Vector2>(); }
    public InputAction GetRotatedAction() { return playerControls.UI.Rotate; }

    #endregion

    private void RestrictSprint() { playerControls.Locomotion.Run.Disable(); }
    private void UnrestrictSprint() { playerControls.Locomotion.Run.Enable(); }
    private void SetUIActionMap()
    {
        foreach(InputActionMap iam in inputActionMaps)
        {
            if(iam.name != uiActionMap)
                iam.Disable();
            else
                iam.Enable();
        }
    }
    private void SetGameplayActionMap()
    {
        foreach (InputActionMap iam in inputActionMaps)
        {
            if (iam.name != uiActionMap)
                iam.Enable();
            else
                iam.Disable();
        }
    }
}
