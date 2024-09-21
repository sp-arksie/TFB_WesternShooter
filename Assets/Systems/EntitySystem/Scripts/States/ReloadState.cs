using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadState : BaseState
{
    private void Update()
    {
        GetHotBarManager().DoReload();
    }
}
