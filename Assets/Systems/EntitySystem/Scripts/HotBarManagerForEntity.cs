using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotBarManagerForEntity : HotBarManager
{
    public List<ItemBase> itemTracker { get; private set; } = new();

    public ItemBase currentSelectedItem { get; private set; }

    public event Action OnQuickAction;

    protected override void Start()
    {
        base.Start();
        for(int i = 0; i < hotBarItems.childCount;  i++)
        {
            ItemBase item = hotBarItems.GetChild(i).GetComponent<ItemBase>();
            itemTracker.Add(item);
        }
        
        // TODO: have them start with index -1
        currentSelectedItem = hotBarItems.GetChild(0).GetComponent<ItemBase>();
    }

    internal void DoQuickAction()
    {
        QuickAction();
        OnQuickAction?.Invoke();
    }

    internal void DoChargeStart() { }

    internal void DoChargeRelease() { }

    internal void DoSelectItem(int index)
    {
        NotifySelectItem(index);
        if(index != -1)
            currentSelectedItem = hotBarItems.GetChild(index).GetComponent<ItemBase>();
        else
            currentSelectedItem = null;
    }

    internal void DoReload()
    {
        Reload();
    }

    #region Helper Functions
    internal bool GetCurrentItemBusy() { return currentItemBusy; }

    #endregion
}
