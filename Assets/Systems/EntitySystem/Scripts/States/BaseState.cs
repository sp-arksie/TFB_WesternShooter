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

    #region General
    protected NavMeshAgent GetAgent() { return entityController.agent; }
    protected void NotifyOrientEntityToTarget() { entityController.OrientEntityToTarget(); }
    #endregion

    #region Sight
    protected EntitySight GetSight() { return entityController.sight; }
    protected Vector3 GetLastSeenTargetPosition() { return entityController.GetLastSeenTargetPosition(); }
    protected void ForgetLastSeenTargetPosition() { entityController.ForgetLastSeenTargetPosition(); }
    #endregion

    #region Items
    protected HotBarManagerForEntity GetHotBarManager() { return entityController.HotBarManager; }
    protected bool GetCurrentItemBusy() { return entityController.HotBarManager.GetCurrentItemBusy(); }
    #endregion

    #region Cover
    protected int GetNumberOfCollidersToSample() { return entityController.NumberOfCollidersToSample; }
    protected float GetCoverCheckingAreaRadius() { return entityController.CoverCheckingAreaRadius; }
    protected LayerMask GetCoverObjectsLayerMask() { return entityController.StaticOccludersLayerMask; }
    protected float GetHidingAccuracy() { return entityController.HidingAccuracy; }
    protected void SetCoverDestination(Vector3 destination) { entityController.NotifySetCoverDestination(destination); }
    protected Vector3 GetCurrentCoverDestination() { return entityController.CurrentCoverDestination; }
    #endregion
}
