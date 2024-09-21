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

    protected NavMeshAgent GetAgent() { return entityController.agent; }

    #region Sight
    protected EntitySight GetSight() { return entityController.sight; }
    protected Vector3 GetLastSeenTargetPosition() { return entityController.GetLastSeenTargetPosition(); }
    protected void ForgetLastSeenTargetPosition() { entityController.ForgetLastSeenTargetPosition(); }
    #endregion

    protected HotBarManagerForEntity GetHotBarManager() { return entityController.HotBarManager; }
    protected bool GetCurrentItemBusy() { return entityController.HotBarManager.GetCurrentItemBusy(); }
    protected void NotifyOrientEntityToTarget() { entityController.OrientEntityToTarget(); }

    #region Strafing
    //internal bool GetHasStrafePositionSet() { return entityController.HasStrafePositionSet; }
    internal Vector3 GetStrafeValue() { return entityController.CurrentStrafeValue; }
    internal float GetStrafeDestinationReachedThreshold() { return entityController.StrafeDestinationReachedThreshold; }
    internal void SetStrafeValue() { entityController.SetStrafeValue(); }
    internal float GetMaxTimeToReachStrafeDestination() { return entityController.MaxTimeToReachStrafeDestination; }
    internal float GetTimeOfLastStrafeSet() { return entityController.NewStrafeDestinationTime; }
    #endregion
}
