using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows;

public class EntityController : MonoBehaviour, IAnimatableEntity
{
    [SerializeField] internal EntitySight sight;

    internal NavMeshAgent agent;

    BaseState currentState;
    PatrolState patrolState;
    SeekState seekState;
    GotoLastTargetPositionState gotoLastTargetPositionState;

    Vector3 lastSeenTargetPosition = Vector3.zero;
    bool hasLastSeenTargetPosition = false;

    Vector3 smoothedLocalMovement = Vector3.zero;
    bool isRunning = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        patrolState =  GetComponent<PatrolState>();
        seekState = GetComponent<SeekState>();
        gotoLastTargetPositionState = GetComponent<GotoLastTargetPositionState>();
    }

    private void Update()
    {
        List<EntityVisible> currentVisiblesToEntity = sight.GetCurrentlyVisiblesToEntity();

        if(currentVisiblesToEntity.Count > 0)
        {
            hasLastSeenTargetPosition = true;
            lastSeenTargetPosition = currentVisiblesToEntity[0].transform.position;
            if (currentState != seekState)
            {
                if (currentState != null) { currentState.enabled = false; }
                currentState = seekState;
                currentState.enabled = true;
            }
        }
        else
        {
            if(currentState != patrolState)
            {
                if(currentState != null) { currentState.enabled = false; }
                currentState = patrolState;
                currentState.enabled = true;
            }
        }

        UpdateAnimation(agent.velocity);
    }

    internal void ForgetLastSeenTargetPosition() { hasLastSeenTargetPosition = false; }

    internal Vector3 GetLastSeenTargetPosition() { return lastSeenTargetPosition; }

    private void UpdateAnimation(Vector3 movementOnPlane)
    {
        Vector3 localMovementBeingApplied = transform.InverseTransformDirection(movementOnPlane);
        smoothedLocalMovement = localMovementBeingApplied.normalized;
    }


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
