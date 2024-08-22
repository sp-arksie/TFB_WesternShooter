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

    List<InputAction> hotBarSlots = new();

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        playerControls = new();
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


    #region POINTER AIM

    public Vector2 GetMouseDelta() { return playerControls.Aim.Mouse.ReadValue<Vector2>(); }

    #endregion


    #region ACTIONS

    public InputAction GetUseAction() { return playerControls.Actions.Use; }
    public InputAction GetShootAction() { return playerControls.Actions.Shoot; }
    public InputAction GetAimAction() {  return playerControls.Actions.Aim; }
    public InputAction GetScrollAction() { return playerControls.Actions.Scroll; }
    public List<InputAction> GetHotBarSlotActions() { return hotBarSlots; }
    //public InputAction GetHotBarSlot1Action() { return playerControls.Actions.HotBarSlot1; }
    //public InputAction GetHotBarSlot2Action() { return playerControls.Actions.HotBarSlot2; }
    //public InputAction GetHotBarSlot3Action() { return playerControls.Actions.HotBarSlot3; }
    //public InputAction GetHotBarSlot4Action() { return playerControls.Actions.HotBarSlot4; }
    //public InputAction GetHotBarSlot5Action() { return playerControls.Actions.HotBarSlot5; }
    //public InputAction GetHotBarSlot6Action() { return playerControls.Actions.HotBarSlot6; }
    //public InputAction GetHotBarSlot7Action() { return playerControls.Actions.HotBarSlot7; }
    //public InputAction GetHotBarSlot8Action() { return playerControls.Actions.HotBarSlot8; }

    #endregion
}
