using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows;

public class EntityController : MonoBehaviour, IAnimatableEntity
{
    [SerializeField] float animationSmoothing = 50f;

    [Header("Senses")]
    [SerializeField] internal EntitySight sight;

    [Header("Melee")]
    [SerializeField] float minimumMeleeRange = 0.5f;

    [Header("Shooting")]
    [SerializeField] float shootingCooldownMin = 2f;
    [SerializeField] float shootingCooldownMax = 4f;
    public float ShootingCooldownMin { get => shootingCooldownMin; private set => shootingCooldownMin = value; }
    public float ShootingCooldownMax { get => shootingCooldownMax; private set => shootingCooldownMax = value; }

    [Header("Strafing")]
    [SerializeField] float maxLeftRightStrafeDistance = 4f;
    [SerializeField] float maxFrontBackStrafeDistance = 0.5f;
    [SerializeField] float strafeDestinationReachedThreshold = 0.3f;
    [SerializeField] float maxTimeToReachStrafeDestination = 4f;
    #region Strafing Properties
    public float MaxLeftRightStrafeDistance { get => maxLeftRightStrafeDistance; private set => maxLeftRightStrafeDistance = value; }
    public float MaxFrontBackStrafeDistance { get => maxFrontBackStrafeDistance; private set => maxFrontBackStrafeDistance = value; }
    public float StrafeDestinationReachedThreshold { get => strafeDestinationReachedThreshold; private set => strafeDestinationReachedThreshold = value; }
    public float NewStrafeDestinationTime { get; private set; }
    public float MaxTimeToReachStrafeDestination { get => maxTimeToReachStrafeDestination; private set => maxTimeToReachStrafeDestination = value; }
    public Vector3 CurrentStrafeValue { get; private set; } = Vector3.zero;
    #endregion

    [Header("Cover")]
    [SerializeField] LayerMask staticOccludersLayerMask = Physics.DefaultRaycastLayers;
    [SerializeField] int numberOfCollidersToSample = 10;
    [SerializeField] float coverCheckingAreaRadius = 10f;
    [Tooltip("-1 is most accurate and 1 is least")]
    [SerializeField] [Range(-1f, 1f)] float hidingAccuracy = 0f;
    [SerializeField] float coverDestinationReachedThreshold = 0.1f;
    #region Cover Properties
    public LayerMask StaticOccludersLayerMask { get => staticOccludersLayerMask; }
    public int NumberOfCollidersToSample { get => numberOfCollidersToSample; }
    public float CoverCheckingAreaRadius { get => coverCheckingAreaRadius; }
    public float HidingAccuracy { get => hidingAccuracy; }
    public float CoverDestinationReachedThreshold { get => coverDestinationReachedThreshold; }
    #endregion

    public HotBarManagerForEntity HotBarManager { get; private set; }
    public float LastQuickActionTime { get; private set; } = -10;
    public Vector3 CurrentCoverDestination { get; private set; }

    internal NavMeshAgent agent;
    BaseState currentState;

    Vector3 lastSeenTargetPosition = Vector3.zero;
    bool hasLastSeenTargetPosition = false;

    Vector3 smoothedLocalMovement = Vector3.zero;
    //bool isRunning = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        HotBarManager = GetComponent<HotBarManagerForEntity>();
    }

    private void OnEnable()
    {
        HotBarManager.OnQuickAction += SetLastQuickActionTime;
    }

    private void OnDisable()
    {
        HotBarManager.OnQuickAction -= SetLastQuickActionTime;
    }

    private void Update()
    {
        UpdateAnimation(agent.velocity);
    }

    private void SetLastQuickActionTime()
    {
        LastQuickActionTime = Time.time;
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

    Vector3 velocityLastFrame = Vector3.zero;
    Vector3 normalisedVelocityLastFrame = Vector3.zero;
    private void UpdateAnimation(Vector3 velocity)
    {
        //float speedPercent = velocity.magnitude / agent.speed;
        //Vector3 localMovementBeingApplied = transform.InverseTransformDirection(velocity);
        //Vector3 normalisedVelocity = new Vector3(
        //    localMovementBeingApplied.normalized.x * speedPercent, localMovementBeingApplied.normalized.y * speedPercent, localMovementBeingApplied.normalized.z * speedPercent);

        //smoothedLocalMovement = normalisedVelocity;

        float maxSpeedPercent = agent.velocity.magnitude / agent.speed;
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        Vector3 normalisedVelocity = new Vector3(
            localVelocity.normalized.x * maxSpeedPercent, localVelocity.normalized.y * maxSpeedPercent, localVelocity.normalized.z * maxSpeedPercent);
        normalisedVelocity = Vector3.Lerp(normalisedVelocityLastFrame, normalisedVelocity, Time.deltaTime * animationSmoothing);
        normalisedVelocityLastFrame = normalisedVelocity;

        smoothedLocalMovement = normalisedVelocity;

    }

    internal void OrientEntityToTarget()
    {
        Vector3 targetPosition = sight.GetCurrentlyVisiblesToEntity()[0].entityVisible.transform.position;

        Vector3 worldDirection = targetPosition - transform.position;
        worldDirection.y = 0f;
        Quaternion rotation = Quaternion.LookRotation(worldDirection, Vector3.up);

        transform.rotation = rotation;
    }

    #region Helper Functions

    internal bool GetHasLastSeenTargetPosition() { return hasLastSeenTargetPosition; }

    internal Vector3 GetLastSeenTargetPosition() { return lastSeenTargetPosition; }

    internal EntitySight.VisibleInMemory GetTarget() { return sight.GetCurrentlyVisiblesToEntity().Count > 0 ? sight.GetCurrentlyVisiblesToEntity()[0] : null; }

    internal float GetMinimumMeleeRange() { return minimumMeleeRange; }

    internal void NotifySetCoverDestination(Vector3 destination) { CurrentCoverDestination = destination; }

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
