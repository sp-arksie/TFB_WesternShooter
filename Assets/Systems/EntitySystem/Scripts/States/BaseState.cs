using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;

public class BaseState : MonoBehaviour
{
    EntityController entityController;

    protected virtual void Awake()
    {
        entityController = gameObject.GetComponent<EntityController>();
    }


    protected EntitySight GetSight() { return entityController.sight; }
    protected NavMeshAgent GetAgent() { return entityController.agent; }
    protected Vector3 GetLastSeenTargetPosition() { return entityController.GetLastSeenTargetPosition(); }
    protected void ForgetLastSeenTargetPosition() { entityController.ForgetLastSeenTargetPosition(); }
    protected HotBarManagerForEntity GetHotBarManager() { return entityController.GetHotBarManager(); }
    protected bool GetCurrentItemBusy() { return entityController.GetHotBarManager().GetCurrentItemBusy(); }
}
