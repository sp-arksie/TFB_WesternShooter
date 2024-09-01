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
    [SerializeField] float minimumMeleeRange = 1f;

    internal NavMeshAgent agent;

    BaseState currentState;
    PatrolState patrolState;
    SeekState seekState;
    GotoLastTargetPositionState gotoLastTargetPositionState;
    MeleeAttackState meleeAttackState;

    Vector3 lastSeenTargetPosition = Vector3.zero;
    bool hasLastSeenTargetPosition = false;

    //float minimumMeleeRange = 1f;

    Vector3 smoothedLocalMovement = Vector3.zero;
    bool isRunning = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        patrolState =  GetComponent<PatrolState>();
        seekState = GetComponent<SeekState>();
        gotoLastTargetPositionState = GetComponent<GotoLastTargetPositionState>();
        meleeAttackState = GetComponent<MeleeAttackState>();
    }

    private void Update()
    {
        //List<EntityVisible> currentVisiblesToEntity = sight.GetCurrentlyVisiblesToEntity();

        //if (currentVisiblesToEntity.Count > 0)
        //{
        //    hasLastSeenTargetPosition = true;
        //    lastSeenTargetPosition = currentVisiblesToEntity[0].transform.position;
        //    if (Vector3.Distance(transform.position, currentVisiblesToEntity[0].transform.position) < minimumMeleeRange)
        //    {
        //        SetOrMaintainState(meleeAttackState);
        //    }
        //    else
        //    {
        //        SetOrMaintainState(seekState);
        //    }
        //}
        //else if (hasLastSeenTargetPosition)
        //{
        //    SetOrMaintainState(gotoLastTargetPositionState);
        //}
        //else
        //{
        //    SetOrMaintainState(patrolState);
        //}

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

    internal EntityVisible GetTarget() { return sight.GetCurrentlyVisiblesToEntity().Count > 0 ? sight.GetCurrentlyVisiblesToEntity()[0] : null; }

    internal float GetMinimumMeleeRange() { return minimumMeleeRange; }

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
