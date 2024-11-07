using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadState : BaseState
{
    private void OnEnable()
    {
        StopRun();
        SetIsMovingToCover(false);
        GetAgent().SetDestination(transform.position);
    }

    private void Update()
    {
        GetHotBarManager().DoReload();
    }
}
