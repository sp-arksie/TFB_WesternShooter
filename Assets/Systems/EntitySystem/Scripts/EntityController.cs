using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows;

public class EntityController : MonoBehaviour, IAnimatableEntity
{
    [Header("Senses")]
    [SerializeField] internal EntitySight sight;

    [Header("Melee")]
    [SerializeField] float minimumMeleeRange = 0.5f;

    internal NavMeshAgent agent;
    HotBarManagerForEntity hotBarManager;

    BaseState currentState;

    Vector3 lastSeenTargetPosition = Vector3.zero;
    bool hasLastSeenTargetPosition = false;

    //float minimumMeleeRange = 1f;

    Vector3 smoothedLocalMovement = Vector3.zero;
    //bool isRunning = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        hotBarManager = GetComponent<HotBarManagerForEntity>();
    }

    private void Update()
    {
        UpdateAnimation(agent.velocity);
    }

    internal void SetOrMaintainState(BaseState stateToSetOrMaintain)
    {
        if (currentState != stateToSetOrMaintain)
        {
            if (currentState != null) { currentState.enabled = false; }
            currentState = stateToSetOrMaintain;
            currentState.enabled = true;
        }
    }

    internal void ForgetLastSeenTargetPosition() { hasLastSeenTargetPosition = false; }

    private void UpdateAnimation(Vector3 movementOnPlane)
    {
        Vector3 localMovementBeingApplied = transform.InverseTransformDirection(movementOnPlane);
        smoothedLocalMovement = localMovementBeingApplied.normalized;
    }

    #region Helper Functions

    internal bool GetHasLastSeenTargetPosition() { return hasLastSeenTargetPosition; }

    internal Vector3 GetLastSeenTargetPosition() { return lastSeenTargetPosition; }

    internal EntitySight.VisibleInMemory GetTarget() { return sight.GetCurrentlyVisiblesToEntity().Count > 0 ? sight.GetCurrentlyVisiblesToEntity()[0] : null; }

    internal float GetMinimumMeleeRange() { return minimumMeleeRange; }

    internal HotBarManagerForEntity GetHotBarManager() { return hotBarManager; }

    #endregion


    #region IAnimatableEntity
    bool IAnimatableEntity.GetIsCrouching()
    {
        return false;
    }

    bool IAnimatableEntity.GetIsGrounded()
    {
        return true;
    }

    bool IAnimatableEntity.GetIsRunning()
    {
        return false;
    }

    float IAnimatableEntity.GetXSpeed()
    {
        return smoothedLocalMovement.x;
    }

    float IAnimatableEntity.GetYSpeed()
    {
        return 0f;
    }

    float IAnimatableEntity.GetZSpeed()
    {
        return smoothedLocalMovement.z;
    }
    #endregion
}
