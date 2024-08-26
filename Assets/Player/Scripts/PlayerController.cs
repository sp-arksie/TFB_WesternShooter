using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

public class PlayerController : MonoBehaviour
{
    [Header("Locomotion")]
    [SerializeField] float jumpSpeed = 7f;
    [SerializeField] float minimumFallVelocity = -3f;
    [SerializeField] float gravityScaleMultiplier = 1f;
    [Space(10)]
    [SerializeField] float accelerationSpeed = 1f;
    [SerializeField] float baseSpeed = 5f;
    [SerializeField] float runSpeedMultiplier = 2f;
    [SerializeField] float crouchSpeedMultiplier = 0.5f;

    [Header("Orientation")]
    [SerializeField] float angularSpeed = 360f;

    [Header("Animation")]
    [SerializeField] float animationSmoothingRate = 1f;

    // Input
    InputManager input;

    // References
    Camera mainCamera;
    CharacterController characterController;
    Animator animator;

    // Movement
    const float gravity = -9.81f;
    float verticalVelocity = 0f;
    Vector3 previousXZMovementValue = Vector3.zero;
    bool playerBusy = false;

    // Animation
    Vector3 smoothedLocalMovementToApply = Vector3.zero;
    int xSpeedHash;
    int zSpeedHash;
    int ySpeedHash;
    int groundedHash;
    int crouchingHash;
    float normalisedVerticalSpeed = 0f;
    float oldNormalisedVerticalSpeed = 0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        animator = GetComponentInChildren<Animator>();
        input = InputManager.Instance;
    }

    private void Start()
    {
        xSpeedHash = Animator.StringToHash("XSpeed");
        zSpeedHash = Animator.StringToHash("ZSpeed");
        ySpeedHash = Animator.StringToHash("YSpeed");
        groundedHash = Animator.StringToHash("Grounded");
        crouchingHash = Animator.StringToHash("Crouching");
    }

    private void Update()
    {
        Vector3 movementOnXZPlane = UpdateMovement();
        UpdateOrientation();
        Vector3 verticalMovement = UpdateVerticalVelocity();

        UpdateAnimation(movementOnXZPlane);

        Vector3 movementToApply = (movementOnXZPlane * baseSpeed + verticalMovement) * Time.deltaTime;

        characterController.Move(movementToApply);
    }

    private Vector3 UpdateMovement()
    {
        Vector2 actionValue = input.GetPlayerMovement();
        Vector3 movementInput = new Vector3(actionValue.x, 0f, actionValue.y);

        Vector3 newMovement = mainCamera.transform.TransformDirection(movementInput);
        float magnitude = newMovement.magnitude;
        newMovement.y = 0f;
        newMovement = newMovement.normalized * magnitude;

        if(input.GetIsCrouching())
        {
            newMovement *= crouchSpeedMultiplier;
        }
        else if (input.GetIsRunning())
        {
            newMovement *= runSpeedMultiplier;
        }

        float t = Mathf.Min(Time.deltaTime, accelerationSpeed) / accelerationSpeed;
        Vector3 movementToApply = Vector3.Lerp(previousXZMovementValue, newMovement, t);
        previousXZMovementValue = movementToApply;

        return movementToApply;
    }

    private void UpdateOrientation()
    {
        Vector3 desiredDirection = mainCamera.transform.forward;
        desiredDirection.y = 0f;

        float signedAngularDistance = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);
        float angleToApply = angularSpeed * Time.deltaTime;

        angleToApply = Mathf.Sign(signedAngularDistance) * ( Mathf.Min(angleToApply, Mathf.Abs(signedAngularDistance)) );
        Quaternion rotationToApply = Quaternion.AngleAxis(angleToApply, Vector3.up);
        transform.rotation *= rotationToApply;

    }

    Vector3 UpdateVerticalVelocity()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = minimumFallVelocity;
        }

        verticalVelocity += gravity * gravityScaleMultiplier * Time.deltaTime;

        if (input.GetHasJumped() && characterController.isGrounded)
            verticalVelocity = jumpSpeed;

        return verticalVelocity * Vector3.up;
    }

    private void UpdateAnimation(Vector3 movementOnPlane)
    {
        Vector3 localMovementBeingApplied = transform.InverseTransformDirection(movementOnPlane);

        Vector3 direction = localMovementBeingApplied - smoothedLocalMovementToApply;
        float smoothingStep = animationSmoothingRate * Time.deltaTime;
        smoothingStep = Mathf.Min(direction.magnitude, smoothingStep);
        smoothedLocalMovementToApply += direction.normalized * smoothingStep;
        if (input.GetIsRunning())
        {
            float t = Mathf.Min(animationSmoothingRate, Time.deltaTime) / animationSmoothingRate;
            smoothedLocalMovementToApply = Vector3.Lerp(smoothedLocalMovementToApply, smoothedLocalMovementToApply * 2, t);
        }
        
        animator.SetBool(crouchingHash, input.GetIsCrouching());
        animator.SetFloat(xSpeedHash, smoothedLocalMovementToApply.x);
        animator.SetFloat(zSpeedHash, smoothedLocalMovementToApply.z);

        animator.SetBool(groundedHash, characterController.isGrounded);
        normalisedVerticalSpeed = Mathf.InverseLerp(jumpSpeed, -jumpSpeed, verticalVelocity);
        
        if(characterController.isGrounded)
        {
            animator.SetFloat(ySpeedHash, oldNormalisedVerticalSpeed);
        }
        else
        {
            animator.SetFloat(ySpeedHash, normalisedVerticalSpeed);
            oldNormalisedVerticalSpeed = normalisedVerticalSpeed;
        }
    }
}
